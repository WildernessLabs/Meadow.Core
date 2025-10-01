using Meadow.Foundation.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Meadow;

/// <summary>
/// Crash-safe, priority-based telemetry store using multi-file persistence
/// Prioritizes errors (priority 0) over general telemetry, with FIFO within each priority
/// </summary>
internal class PriorityTelemetryStore : IMeadowCloudTelemetryStore, IDisposable
{
    private const int DefaultMaxRecordsPerFile = 50;
    private const int WriteBufferSize = 512;

    private readonly string _baseDirectory;
    private readonly int _maxRecordsPerFile;
    private readonly object _lock = new object();
    private readonly Dictionary<int, FileStream?> _writeStreams = new Dictionary<int, FileStream?>();
    private readonly Dictionary<int, int> _currentFileCounts = new Dictionary<int, int>();
    private long _receptionCounter = 0;
    private bool _disposed = false;

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return CountByPriority().Values.Sum();
            }
        }
    }

    public PriorityTelemetryStore(string baseDirectory, int maxRecordsPerFile = DefaultMaxRecordsPerFile)
    {
        _baseDirectory = baseDirectory ?? throw new ArgumentNullException(nameof(baseDirectory));
        _maxRecordsPerFile = maxRecordsPerFile > 0 ? maxRecordsPerFile : DefaultMaxRecordsPerFile;

        InitializeDirectories();
        RecoverReceptionCounter();
    }

    private void InitializeDirectories()
    {
        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
        }

        // Create priority directories (0-3 typical, but support more)
        for (int i = 0; i <= 3; i++)
        {
            var priorityDir = GetPriorityDirectory(i);
            if (!Directory.Exists(priorityDir))
            {
                Directory.CreateDirectory(priorityDir);
            }
        }
    }

    private void RecoverReceptionCounter()
    {
        // Scan all existing files to find the highest reception sequence number
        // This ensures new entries have higher sequence numbers than persisted ones
        long maxSequence = 0;

        var priorityDirs = Directory.GetDirectories(_baseDirectory)
            .Where(d => Path.GetFileName(d).StartsWith("p"));

        foreach (var priorityDir in priorityDirs)
        {
            var files = Directory.GetFiles(priorityDir, "*.dat");

            foreach (var file in files)
            {
                var fileMaxSequence = GetMaxSequenceFromFile(file);
                if (fileMaxSequence > maxSequence)
                {
                    maxSequence = fileMaxSequence;
                }
            }
        }

        _receptionCounter = maxSequence;

        if (_receptionCounter > 0)
        {
            Resolver.Log.Info($"Recovered reception counter: {_receptionCounter}");
        }
    }

    private long GetMaxSequenceFromFile(string filePath)
    {
        long maxSequence = 0;

        try
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, WriteBufferSize))
            {
                while (stream.Position < stream.Length)
                {
                    // Read length prefix
                    var lengthBytes = new byte[4];
                    if (stream.Read(lengthBytes, 0, 4) != 4)
                        break;

                    var dataLength = BitConverter.ToInt32(lengthBytes, 0);
                    if (dataLength <= 0 || dataLength > 1024 * 1024)
                        break;

                    // Read and deserialize entry
                    var data = new byte[dataLength];
                    if (stream.Read(data, 0, dataLength) != dataLength)
                        break;

                    try
                    {
                        var json = Encoding.UTF8.GetString(data);
                        var entry = MicroJson.Deserialize<TelemetryEntry>(json);
                        if (entry.ReceptionSequence > maxSequence)
                        {
                            maxSequence = entry.ReceptionSequence;
                        }
                    }
                    catch
                    {
                        // Skip corrupted entry
                        continue;
                    }
                }
            }
        }
        catch
        {
            // Return what we found so far
        }

        return maxSequence;
    }

    private string GetPriorityDirectory(int priority)
    {
        return Path.Combine(_baseDirectory, $"p{priority}");
    }

    public void Enqueue(object item, string endpoint, int priority = 3)
    {
        if (item == null) return;

        lock (_lock)
        {
            var receptionSeq = ++_receptionCounter;
            var entry = new TelemetryEntry(item, endpoint, priority, receptionSeq);

            WriteEntryToFile(entry, priority);
        }
    }

    private void WriteEntryToFile(TelemetryEntry entry, int priority)
    {
        var priorityDir = GetPriorityDirectory(priority);

        // Ensure directory exists for dynamic priorities
        if (!Directory.Exists(priorityDir))
        {
            Directory.CreateDirectory(priorityDir);
        }

        // Get or create write stream for this priority
        if (!_writeStreams.ContainsKey(priority) || _writeStreams[priority] == null)
        {
            OpenNewFileForPriority(priority);
        }

        // Check if we need to rotate to a new file
        if (_currentFileCounts.ContainsKey(priority) && _currentFileCounts[priority] >= _maxRecordsPerFile)
        {
            CloseWriteStream(priority);
            OpenNewFileForPriority(priority);
        }

        var stream = _writeStreams[priority];
        if (stream == null) return;

        try
        {
            // Serialize with MicroJson
            var json = MicroJson.Serialize(entry);
            var data = Encoding.UTF8.GetBytes(json);

            // Write length prefix (4 bytes)
            var lengthBytes = BitConverter.GetBytes(data.Length);
            stream.Write(lengthBytes, 0, 4);

            // Write serialized data
            stream.Write(data, 0, data.Length);

            // Force to flash for crash safety
            stream.Flush(true);

            // Update count
            if (!_currentFileCounts.ContainsKey(priority))
            {
                _currentFileCounts[priority] = 0;
            }
            _currentFileCounts[priority]++;
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Failed to write telemetry entry: {ex.Message}");
        }
    }

    private void OpenNewFileForPriority(int priority)
    {
        var priorityDir = GetPriorityDirectory(priority);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var filePath = Path.Combine(priorityDir, $"{timestamp}.dat");

        // Ensure unique filename
        int suffix = 0;
        while (File.Exists(filePath))
        {
            filePath = Path.Combine(priorityDir, $"{timestamp}_{++suffix}.dat");
        }

        var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, WriteBufferSize);
        _writeStreams[priority] = stream;
        _currentFileCounts[priority] = 0;
    }

    private void CloseWriteStream(int priority)
    {
        if (_writeStreams.ContainsKey(priority) && _writeStreams[priority] != null)
        {
            _writeStreams[priority]?.Flush(true);
            _writeStreams[priority]?.Dispose();
            _writeStreams[priority] = null;
        }
    }

    public CloudTelemetryItem? Peek()
    {
        lock (_lock)
        {
            var entry = GetNextEntry(remove: false);
            return entry != null ? new CloudTelemetryItem(entry.Item, entry.EndPoint, entry.Priority)
            {
                ReceptionSequence = entry.ReceptionSequence
            } : null;
        }
    }

    public CloudTelemetryItem? Dequeue()
    {
        lock (_lock)
        {
            var entry = GetNextEntry(remove: true);
            return entry != null ? new CloudTelemetryItem(entry.Item, entry.EndPoint, entry.Priority)
            {
                ReceptionSequence = entry.ReceptionSequence
            } : null;
        }
    }

    private TelemetryEntry? GetNextEntry(bool remove)
    {
        // Get all priority directories sorted by priority (0 first)
        var priorityDirs = Directory.GetDirectories(_baseDirectory)
            .Where(d => Path.GetFileName(d).StartsWith("p"))
            .OrderBy(d =>
            {
                var priorityStr = Path.GetFileName(d).Substring(1);
                return int.TryParse(priorityStr, out var p) ? p : int.MaxValue;
            })
            .ToList();

        foreach (var priorityDir in priorityDirs)
        {
            var files = Directory.GetFiles(priorityDir, "*.dat")
                .OrderBy(f => f) // Files are named by timestamp, so this gives FIFO
                .ToList();

            foreach (var file in files)
            {
                var entry = ReadFirstEntryFromFile(file, remove);
                if (entry != null)
                {
                    return entry;
                }
            }
        }

        return null;
    }

    private TelemetryEntry? ReadFirstEntryFromFile(string filePath, bool remove)
    {
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, WriteBufferSize))
            {
                if (stream.Length < 4)
                {
                    // Empty or corrupted file, delete it
                    stream.Close();
                    File.Delete(filePath);
                    return null;
                }

                // Read length prefix
                var lengthBytes = new byte[4];
                if (stream.Read(lengthBytes, 0, 4) != 4)
                {
                    stream.Close();
                    File.Delete(filePath);
                    return null;
                }

                var dataLength = BitConverter.ToInt32(lengthBytes, 0);
                if (dataLength <= 0 || dataLength > 1024 * 1024) // Sanity check: max 1MB per record
                {
                    stream.Close();
                    File.Delete(filePath);
                    return null;
                }

                // Read serialized data
                var data = new byte[dataLength];
                var bytesRead = stream.Read(data, 0, dataLength);
                if (bytesRead != dataLength)
                {
                    // Incomplete record, likely corruption
                    stream.Close();
                    File.Delete(filePath);
                    return null;
                }

                var json = Encoding.UTF8.GetString(data);
                var entry = MicroJson.Deserialize<TelemetryEntry>(json);

                if (remove)
                {
                    // Check if there's more data in the file
                    var hasMoreData = stream.Position < stream.Length;
                    stream.Close();

                    if (hasMoreData)
                    {
                        // Remove this entry from the file by rewriting the rest
                        RewriteFileWithoutFirstEntry(filePath);
                    }
                    else
                    {
                        // File only had this one entry, delete it
                        File.Delete(filePath);
                    }
                }

                return entry;
            }
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Failed to read telemetry file {filePath}: {ex.Message}");

            // Delete corrupted file
            try { File.Delete(filePath); } catch { }

            return null;
        }
    }

    private void RewriteFileWithoutFirstEntry(string filePath)
    {
        var tempPath = filePath + ".tmp";

        try
        {
            using (var readStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, WriteBufferSize))
            using (var writeStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, WriteBufferSize))
            {
                // Skip first entry
                var lengthBytes = new byte[4];
                if (readStream.Read(lengthBytes, 0, 4) == 4)
                {
                    var dataLength = BitConverter.ToInt32(lengthBytes, 0);
                    if (dataLength > 0 && dataLength < 1024 * 1024)
                    {
                        readStream.Seek(dataLength, SeekOrigin.Current);

                        // Copy remaining data
                        var buffer = new byte[WriteBufferSize];
                        int bytesRead;
                        while ((bytesRead = readStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writeStream.Write(buffer, 0, bytesRead);
                        }

                        writeStream.Flush(true);
                    }
                }
            }

            // Replace original with temp
            File.Delete(filePath);
            File.Move(tempPath, filePath);
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Failed to rewrite telemetry file {filePath}: {ex.Message}");

            // Clean up temp file
            try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { }
        }
    }

    public Dictionary<int, int> CountByPriority()
    {
        lock (_lock)
        {
            var counts = new Dictionary<int, int>();

            var priorityDirs = Directory.GetDirectories(_baseDirectory)
                .Where(d => Path.GetFileName(d).StartsWith("p"));

            foreach (var priorityDir in priorityDirs)
            {
                var priorityStr = Path.GetFileName(priorityDir).Substring(1);
                if (!int.TryParse(priorityStr, out var priority))
                    continue;

                var files = Directory.GetFiles(priorityDir, "*.dat");
                var totalCount = 0;

                foreach (var file in files)
                {
                    totalCount += CountEntriesInFile(file);
                }

                if (totalCount > 0)
                {
                    counts[priority] = totalCount;
                }
            }

            return counts;
        }
    }

    private int CountEntriesInFile(string filePath)
    {
        var count = 0;

        try
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, WriteBufferSize))
            {
                while (stream.Position < stream.Length)
                {
                    // Read length prefix
                    var lengthBytes = new byte[4];
                    if (stream.Read(lengthBytes, 0, 4) != 4)
                        break;

                    var dataLength = BitConverter.ToInt32(lengthBytes, 0);
                    if (dataLength <= 0 || dataLength > 1024 * 1024)
                        break;

                    // Skip the data
                    stream.Seek(dataLength, SeekOrigin.Current);
                    count++;
                }
            }
        }
        catch
        {
            // Return count of readable entries
        }

        return count;
    }

    public void Dispose()
    {
        if (_disposed) return;

        lock (_lock)
        {
            // Close all write streams
            foreach (var priority in _writeStreams.Keys.ToList())
            {
                CloseWriteStream(priority);
            }

            _disposed = true;
        }
    }
}
