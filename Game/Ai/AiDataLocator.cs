/* ----- ----- ----- ----- */
// AiDataLocator.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Text.Json;

namespace Chinese_Chess_v3.Game.Ai;

public enum AiDataKind { Engine, Data }

/// <summary>A discovered engine or unrecognized data file. Discovery never starts a process.</summary>
public sealed record AiDataEntry(AiDataKind Kind, string Name, string Path, IReadOnlyList<string>? Args = null);

/// <summary>Discovers user-supplied engine manifests, engine programs and other files.</summary>
public sealed class AiDataLocator
{
    private readonly string _folder;
    public AiDataLocator(string folder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        _folder = Path.GetFullPath(folder);
    }

    public IReadOnlyList<AiDataEntry> Enumerate()
    {
        var entries = new List<AiDataEntry>();
        try { Directory.CreateDirectory(_folder); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return entries; }
        string[] files;
        try { files = Directory.GetFiles(_folder); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return entries; }
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (string file in files.OrderBy(f => f, StringComparer.Ordinal))
        {
            try
            {
                using var stream = File.OpenRead(file);
                if (!file.EndsWith(".engine.json", StringComparison.OrdinalIgnoreCase)) continue;
                using var document = JsonDocument.Parse(stream);
                var root = document.RootElement;
                string? name = root.GetProperty("name").GetString(), path = root.GetProperty("path").GetString();
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(path)) continue;
                string full = Path.GetFullPath(path, _folder);
                using var engine = File.OpenRead(full);
                string[] args = root.TryGetProperty("args", out var list) ?
                    list.EnumerateArray().Select(a => a.GetString() ?? throw new JsonException("Invalid argument.")).ToArray() : Array.Empty<string>();
                entries.Add(new(AiDataKind.Engine, name, full, args));
                seen.Add(full);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or KeyNotFoundException or InvalidOperationException or ArgumentException) { }
        }
        foreach (string file in files.OrderBy(f => f, StringComparer.Ordinal))
        {
            if (seen.Contains(file) || file.EndsWith(".engine.json", StringComparison.OrdinalIgnoreCase)) continue;
            try
            {
                using var stream = File.OpenRead(file);
                string name = Path.GetFileName(file);
                bool pattern = name.Contains("pikafish", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("elephanteye", StringComparison.OrdinalIgnoreCase) || name.Contains("engine", StringComparison.OrdinalIgnoreCase);
                bool executable = file.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                    (!OperatingSystem.IsWindows() && (File.GetUnixFileMode(file) &
                        (UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute)) != 0);
                entries.Add(new(pattern && executable ? AiDataKind.Engine : AiDataKind.Data, name, file));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { }
        }
        return entries.ToArray();
    }
}

/// <summary>Constructs engines without coupling callers to their implementation.</summary>
public static class EngineFactory
{
    public static IChessEngine Create(string spec)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(spec);
        if (spec.StartsWith("builtin:", StringComparison.OrdinalIgnoreCase) &&
            Enum.TryParse<EngineLevel>(spec[8..], true, out var level) && Enum.IsDefined(level))
            return BuiltInEngine.Create(level);
        if (spec.StartsWith("uci:", StringComparison.OrdinalIgnoreCase) && spec.Length > 4)
            return new UciEngine(spec[4..]);
        throw new ArgumentException("Expected builtin:<level> or uci:<path>.", nameof(spec));
    }

    public static IChessEngine Create(AiDataEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (entry.Kind != AiDataKind.Engine) throw new ArgumentException("The entry is not an engine.", nameof(entry));
        return new UciEngine(entry.Path, entry.Args);
    }
}
