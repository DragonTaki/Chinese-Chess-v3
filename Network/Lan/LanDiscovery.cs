/* ----- ----- ----- ----- */
// LanDiscovery.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Chinese_Chess_v3.Network.Protocol;

namespace Chinese_Chess_v3.Network.Lan
{
    /// <summary>Broadcast discovery on the local subnet; no accounts or external server are involved.</summary>
    public sealed class LanDiscovery : IDisposable
    {
        private UdpClient _responder;
        private CancellationTokenSource _lifetime;

        public event Action<Exception> Failed;

        private sealed record DiscoveryPacket
        {
            public string Protocol { get; init; }
            public string Nonce { get; init; }
            public string RoomId { get; init; }
            public string Nickname { get; init; }
            public int Port { get; init; }
        }

        /// <summary>Advertises a waiting room. Call StopAdvertising when the room fills or starts.</summary>
        public void Advertise(string roomId, string nickname, int port)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                throw new ArgumentException("A room identifier is required.", nameof(roomId));
            nickname = LanProtocol.ValidateNickname(nickname);
            if (port < LanProtocol.FirstGamePort || port > LanProtocol.LastGamePort)
                throw new ArgumentOutOfRangeException(nameof(port));
            StopAdvertising();
            var responder = new UdpClient(new IPEndPoint(IPAddress.Any, LanProtocol.DiscoveryPort));
            _responder = responder;
            _lifetime = new CancellationTokenSource();
            _ = RespondAsync(responder, roomId, nickname, port, _lifetime.Token);
        }

        /// <summary>Finds nearby waiting rooms during a bounded search window.</summary>
        public static async Task<IReadOnlyList<LanHost>> FindAsync(TimeSpan duration, CancellationToken token = default, IPAddress address = null)
        {
            if (duration <= TimeSpan.Zero || duration > TimeSpan.FromSeconds(10))
                throw new ArgumentOutOfRangeException(nameof(duration));
            using var client = new UdpClient(new IPEndPoint(IPAddress.Any, 0)) { EnableBroadcast = true };
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
            timeout.CancelAfter(duration);
            string nonce = Guid.NewGuid().ToString("N");
            byte[] query = Encoding.UTF8.GetBytes(ProtocolJson.Serialize(new DiscoveryPacket
            {
                Protocol = LanProtocol.Identity,
                Nonce = nonce,
            }));
            await client.SendAsync(query, new IPEndPoint(address ?? IPAddress.Broadcast, LanProtocol.DiscoveryPort), token);
            var rooms = new Dictionary<string, LanHost>();
            try
            {
                while (true)
                {
                    UdpReceiveResult result = await client.ReceiveAsync(timeout.Token);
                    if (result.Buffer.Length > 2048 || !ProtocolJson.TryDeserialize(Encoding.UTF8.GetString(result.Buffer), out DiscoveryPacket reply)
                        || reply.Protocol != LanProtocol.Identity || reply.Nonce != nonce || string.IsNullOrWhiteSpace(reply.RoomId)
                        || string.IsNullOrWhiteSpace(reply.Nickname) || reply.Nickname.Length > 32
                        || reply.Port < LanProtocol.FirstGamePort || reply.Port > LanProtocol.LastGamePort)
                        continue;
                    var endpoint = new IPEndPoint(result.RemoteEndPoint.Address, reply.Port);
                    rooms[endpoint + "/" + reply.RoomId] = new LanHost
                    {
                        RoomId = reply.RoomId,
                        Nickname = reply.Nickname,
                        Endpoint = endpoint,
                    };
                }
            }
            catch (OperationCanceledException) when (!token.IsCancellationRequested) { }
            token.ThrowIfCancellationRequested();
            return new List<LanHost>(rooms.Values);
        }

        public void StopAdvertising()
        {
            _lifetime?.Cancel();
            _responder?.Dispose();
            _lifetime?.Dispose();
            _lifetime = null;
            _responder = null;
        }

        public void Dispose() => StopAdvertising();

        private async Task RespondAsync(UdpClient client, string roomId, string nickname, int port, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    UdpReceiveResult received = await client.ReceiveAsync(token);
                    if (received.Buffer.Length > 2048 || !ProtocolJson.TryDeserialize(Encoding.UTF8.GetString(received.Buffer), out DiscoveryPacket query)
                        || query.Protocol != LanProtocol.Identity || query.Nonce?.Length != 32 || query.RoomId != null)
                        continue;
                    byte[] reply = Encoding.UTF8.GetBytes(ProtocolJson.Serialize(new DiscoveryPacket
                    {
                        Protocol = LanProtocol.Identity,
                        Nonce = query.Nonce,
                        RoomId = roomId,
                        Nickname = nickname,
                        Port = port,
                    }));
                    await client.SendAsync(reply, received.RemoteEndPoint, token);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
            catch (ObjectDisposedException) when (token.IsCancellationRequested) { }
            catch (SocketException) when (token.IsCancellationRequested) { }
            catch (SocketException ex) { Failed?.Invoke(ex); }
        }
    }
}
