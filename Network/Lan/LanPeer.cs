/* ----- ----- ----- ----- */
// LanPeer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Chinese_Chess_v3.Network.Protocol;

using NetClient;

namespace Chinese_Chess_v3.Network.Lan
{
    /// <summary>One plain TCP peer for a LAN room. Events run on the networking thread.</summary>
    public sealed class LanPeer : IDisposable
    {
        private readonly object _gate = new();
        private CancellationTokenSource _lifetime;
        private TcpListener _listener;
        private TcpClient _client;
        private Channel<PendingLine> _outgoing;
        private int _generation;

        private sealed record PendingLine(string Text, TaskCompletionSource<bool> Sent = null);

        public event Action Connected;
        public event Action<LanMessage> Received;
        public event Action Disconnected;
        public event Action<Exception> Failed;

        public int Port { get; private set; }
        public bool IsConnected { get { lock (_gate) return _client != null; } }

        /// <summary>Listens on the default port or one of the bounded fallback ports.</summary>
        public void Host()
        {
            Stop();
            TcpListener listener = null;
            for (int port = LanProtocol.FirstGamePort; port <= LanProtocol.LastGamePort; port++)
            {
                listener = new TcpListener(IPAddress.Any, port);
                try { listener.Start(1); Port = port; break; }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    listener.Stop();
                    listener = null;
                }
                catch { listener.Stop(); throw; }
            }
            if (listener == null)
                throw new IOException("Every LAN room port is already in use.");
            _listener = listener;
            _lifetime = new CancellationTokenSource();
            _ = AcceptAsync(listener, _generation, _lifetime.Token);
        }

        /// <summary>Connects to the selected discovery result; a failed or cancelled attempt releases its socket.</summary>
        public async Task JoinAsync(IPEndPoint endpoint, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(endpoint);
            Stop();
            var lifetime = new CancellationTokenSource();
            _lifetime = lifetime;
            CancellationToken lifetimeToken = lifetime.Token;
            int generation = _generation;
            using var connect = CancellationTokenSource.CreateLinkedTokenSource(token, lifetimeToken);
            connect.CancelAfter(TimeSpan.FromSeconds(5));
            var client = new TcpClient(endpoint.AddressFamily);
            try
            {
                await client.ConnectAsync(endpoint.Address, endpoint.Port, connect.Token);
                connect.Token.ThrowIfCancellationRequested();
                Task run = RunAsync(client, generation, lifetimeToken);
                _ = run;
            }
            catch { client.Dispose(); throw; }
        }

        /// <summary>Queues a message without blocking the UI. A stalled peer is disconnected instead of growing the queue.</summary>
        public bool Send(LanMessage message)
        {
            ArgumentNullException.ThrowIfNull(message);
            lock (_gate)
            {
                if (_client == null || _outgoing == null)
                    return false;
                if (_outgoing.Writer.TryWrite(new PendingLine(ProtocolJson.Serialize(message))))
                    return true;
                _client.Dispose();
                return false;
            }
        }

        public void Stop()
        {
            lock (_gate)
                _generation++;
            _lifetime?.Cancel();
            _listener?.Stop();
            lock (_gate)
            {
                _client?.Dispose();
                _outgoing?.Writer.TryComplete();
                _client = null;
                _outgoing = null;
            }
            _lifetime?.Dispose();
            _lifetime = null;
            _listener = null;
            Port = 0;
        }

        public void Dispose() => Stop();

        /// <summary>Waits for queued packets to be written before an explicit leave closes the socket.</summary>
        public async Task<bool> FlushAsync(CancellationToken token)
        {
            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (_gate)
            {
                if (_client == null || _outgoing == null || !_outgoing.Writer.TryWrite(new PendingLine(null, completion)))
                    return false;
            }
            return await completion.Task.WaitAsync(token);
        }

        /// <summary>Closes the current guest while leaving the hosting listener available.</summary>
        public void DisconnectPeer()
        {
            lock (_gate)
                _client?.Dispose();
        }

        private async Task AcceptAsync(TcpListener listener, int generation, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync(token);
                    await RunAsync(client, generation, token);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
            catch (ObjectDisposedException) when (token.IsCancellationRequested) { }
            catch (SocketException) when (token.IsCancellationRequested) { }
            catch (Exception ex) { Failed?.Invoke(ex); }
        }

        private async Task RunAsync(TcpClient client, int generation, CancellationToken lifetime)
        {
            using (client)
            using (var connection = CancellationTokenSource.CreateLinkedTokenSource(lifetime))
            {
                var outgoing = Channel.CreateBounded<PendingLine>(256);
                client.NoDelay = true;
                client.SendTimeout = 5000;
                lock (_gate)
                {
                    if (generation != _generation || lifetime.IsCancellationRequested)
                        return;
                    _client = client;
                    _outgoing = outgoing;
                }
                var framing = new LineFraming(client.GetStream());
                Task writer = WriteAsync(framing, outgoing.Reader, connection);
                Task heartbeat = HeartbeatAsync(outgoing.Writer, connection.Token);
                try
                {
                    lock (_gate)
                    {
                        if (generation != _generation)
                            return;
                        Connected?.Invoke();
                    }
                    while (!connection.IsCancellationRequested)
                    {
                        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(connection.Token);
                        timeout.CancelAfter(TimeSpan.FromSeconds(10));
                        string line = await framing.ReadLineAsync(timeout.Token);
                        if (line == null)
                            break;
                        if (line.Length > 65536 || !ProtocolJson.TryDeserialize(line, out LanMessage message)
                            || string.IsNullOrWhiteSpace(message.Type))
                            throw new IOException("Invalid LAN message.");
                        if (message.Type != "Heartbeat")
                        {
                            lock (_gate)
                            {
                                if (generation == _generation)
                                    Received?.Invoke(message);
                            }
                        }
                    }
                }
                catch (OperationCanceledException) when (lifetime.IsCancellationRequested) { }
                catch (Exception ex) when (ex is IOException or SocketException or JsonException or OperationCanceledException or ObjectDisposedException)
                {
                    lock (_gate)
                    {
                        if (generation == _generation && !lifetime.IsCancellationRequested)
                            Failed?.Invoke(ex);
                    }
                }
                finally
                {
                    connection.Cancel();
                    client.Dispose();
                    outgoing.Writer.TryComplete();
                    try { await Task.WhenAll(writer, heartbeat); }
                    catch (Exception ex) when (ex is IOException or SocketException or OperationCanceledException or ObjectDisposedException) { }
                    bool current;
                    lock (_gate)
                    {
                        current = ReferenceEquals(_client, client);
                        if (current) { _client = null; _outgoing = null; }
                    }
                    lock (_gate)
                    {
                        if (current && generation == _generation && !lifetime.IsCancellationRequested)
                            Disconnected?.Invoke();
                    }
                }
            }
        }

        private static async Task WriteAsync(LineFraming framing, ChannelReader<PendingLine> reader, CancellationTokenSource connection)
        {
            try
            {
                await foreach (PendingLine line in reader.ReadAllAsync(connection.Token))
                {
                    if (line.Text != null)
                        framing.WriteLine(line.Text);
                    line.Sent?.TrySetResult(true);
                }
            }
            catch
            {
                connection.Cancel();
                throw;
            }
            finally
            {
                while (reader.TryRead(out var pending))
                    pending.Sent?.TrySetCanceled();
            }
        }

        private static async Task HeartbeatAsync(ChannelWriter<PendingLine> writer, CancellationToken token)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            string message = ProtocolJson.Serialize(LanMessage.Create("Heartbeat", new { }));
            while (await timer.WaitForNextTickAsync(token))
                await writer.WriteAsync(new PendingLine(message), token);
        }
    }
}
