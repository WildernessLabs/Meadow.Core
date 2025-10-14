using Meadow.Cloud;
using System.Reflection;

namespace Core.Unit.Tests;

/// <summary>
/// Tests for SimpleStringPool power-loss recovery and corruption handling
/// These tests use reflection to access internal SimpleStringPool class
/// </summary>
public class StringPoolRecoveryTests
{
    private string GetTestDirectory()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "meadow-test-" + Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        return testDir;
    }

    private void CleanupTestDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch
            {
                // Best effort cleanup
            }
        }
    }

    private object CreateStringPool(string filePath)
    {
        // Use reflection to create SimpleStringPool (it's internal)
        var assembly = typeof(TalusTelemetryStore).Assembly;
        var poolType = assembly.GetType("Meadow.Cloud.SimpleStringPool");
        if (poolType == null)
        {
            throw new InvalidOperationException("SimpleStringPool type not found");
        }

        return Activator.CreateInstance(poolType, new object[] { filePath })
            ?? throw new InvalidOperationException("Failed to create SimpleStringPool");
    }

    private int CallAdd(object pool, string value)
    {
        var method = pool.GetType().GetMethod("Add");
        if (method == null) throw new InvalidOperationException("Add method not found");
        return (int)method.Invoke(pool, new object[] { value })!;
    }

    private string? CallGet(object pool, int id)
    {
        var method = pool.GetType().GetMethod("Get");
        if (method == null) throw new InvalidOperationException("Get method not found");
        return (string?)method.Invoke(pool, new object[] { id });
    }

    private void CallDispose(object pool)
    {
        var method = pool.GetType().GetMethod("Dispose");
        if (method == null) throw new InvalidOperationException("Dispose method not found");
        method.Invoke(pool, null);
    }

    [Fact]
    public void StringPoolRecoversFromChecksumMismatch()
    {
        var testDir = GetTestDirectory();
        var poolPath = Path.Combine(testDir, "test.pool");

        try
        {
            // Create pool and add some strings
            var pool1 = CreateStringPool(poolPath);
            var id1 = CallAdd(pool1, "test string 1");
            var id2 = CallAdd(pool1, "test string 2");
            CallDispose(pool1);

            // Verify file was created with checksum
            Assert.True(File.Exists(poolPath));
            var lines = File.ReadAllLines(poolPath);
            Assert.True(lines[0].StartsWith("#CHECKSUM:"), "First line should be checksum");

            // Corrupt the file by modifying data but not checksum
            lines[2] = "1|corrupted data|1";
            File.WriteAllLines(poolPath, lines);

            // Try to load - should detect corruption
            var pool2 = CreateStringPool(poolPath);

            // If no backup exists, pool should start fresh
            // Verify it starts with clean state
            var retrieved = CallGet(pool2, id1);
            Assert.Null(retrieved); // Should not find corrupted data

            CallDispose(pool2);
        }
        finally
        {
            CleanupTestDirectory(testDir);
        }
    }

    [Fact]
    public void StringPoolUsesBackupWhenMainFileCorrupted()
    {
        var testDir = GetTestDirectory();
        var poolPath = Path.Combine(testDir, "test.pool");
        var backupPath = poolPath + ".bak";

        try
        {
            // Create pool with data
            var pool1 = CreateStringPool(poolPath);
            var id1 = CallAdd(pool1, "important string");
            CallDispose(pool1);

            // Create a valid backup manually
            File.Copy(poolPath, backupPath, true);

            // Corrupt main file
            File.WriteAllText(poolPath, "garbage data\nmore garbage");

            // Open pool - should recover from backup
            var pool2 = CreateStringPool(poolPath);

            // Should have recovered data from backup
            var retrieved = CallGet(pool2, id1);
            Assert.Equal("important string", retrieved);

            CallDispose(pool2);

            // Main file should be restored from backup
            Assert.True(File.Exists(poolPath));
        }
        finally
        {
            CleanupTestDirectory(testDir);
        }
    }

    [Fact]
    public void StringPoolHandlesPartialWrite()
    {
        var testDir = GetTestDirectory();
        var poolPath = Path.Combine(testDir, "test.pool");

        try
        {
            // Create pool with multiple strings
            var pool1 = CreateStringPool(poolPath);
            var id1 = CallAdd(pool1, "string 1");
            var id2 = CallAdd(pool1, "string 2");
            var id3 = CallAdd(pool1, "string 3");
            CallDispose(pool1);

            // Simulate partial write by truncating file
            var fileInfo = new FileInfo(poolPath);
            var originalLength = fileInfo.Length;

            using (var stream = new FileStream(poolPath, FileMode.Open, FileAccess.Write))
            {
                // Truncate to 60% - simulates power loss during write
                stream.SetLength(originalLength * 6 / 10);
            }

            // Reopen - should handle gracefully
            var pool2 = CreateStringPool(poolPath);

            // Pool should start fresh or skip corrupted entries
            // Either way, it shouldn't crash
            CallDispose(pool2);
        }
        finally
        {
            CleanupTestDirectory(testDir);
        }
    }

    [Fact]
    public void StringPoolSkipsCorruptedEntriesInValidFile()
    {
        var testDir = GetTestDirectory();
        var poolPath = Path.Combine(testDir, "test.pool");

        try
        {
            // Create a file with mixed valid and invalid entries (no checksum - old format)
            var lines = new[]
            {
                "1|valid string 1|1",
                "invalid line without pipes",
                "2|valid string 2|1",
                "3|another|bad|pipe|count|1",
                "4|valid string 3|1"
            };
            File.WriteAllLines(poolPath, lines);

            // Load pool - should skip invalid entries
            var pool = CreateStringPool(poolPath);

            // Should have loaded valid entries
            var val1 = CallGet(pool, 1);
            var val2 = CallGet(pool, 2);
            var val4 = CallGet(pool, 4);

            Assert.Equal("valid string 1", val1);
            Assert.Equal("valid string 2", val2);
            Assert.Equal("valid string 3", val4);

            CallDispose(pool);
        }
        finally
        {
            CleanupTestDirectory(testDir);
        }
    }

    [Fact]
    public void StringPoolCreatesBackupOnSuccessfulWrite()
    {
        var testDir = GetTestDirectory();
        var poolPath = Path.Combine(testDir, "test.pool");
        var backupPath = poolPath + ".bak";

        try
        {
            // Create pool and add data
            var pool1 = CreateStringPool(poolPath);
            CallAdd(pool1, "first write");
            CallDispose(pool1);

            // No backup yet (first write)
            var hasBackup = File.Exists(backupPath);

            // Add more data
            var pool2 = CreateStringPool(poolPath);
            CallAdd(pool2, "second write");
            CallDispose(pool2);

            // Now backup should exist
            Assert.True(File.Exists(backupPath), "Backup file should be created after second write");

            // Backup should contain valid data
            var backupLines = File.ReadAllLines(backupPath);
            Assert.True(backupLines.Length > 0);
        }
        finally
        {
            CleanupTestDirectory(testDir);
        }
    }

    [Fact]
    public void StringPoolSurvivesPowerLossDuringWrite()
    {
        var testDir = GetTestDirectory();
        var poolPath = Path.Combine(testDir, "test.pool");
        var tempPath = poolPath + ".tmp";

        try
        {
            // Create pool with initial data
            var pool1 = CreateStringPool(poolPath);
            var id1 = CallAdd(pool1, "original data");
            CallDispose(pool1);

            // Simulate power loss during write by creating a corrupt .tmp file
            File.WriteAllText(tempPath, "incomplete write data\n");

            // Reopen pool - should ignore incomplete .tmp file and use main file
            var pool2 = CreateStringPool(poolPath);

            // Should still have original data
            var retrieved = CallGet(pool2, id1);
            Assert.Equal("original data", retrieved);

            CallDispose(pool2);
        }
        finally
        {
            CleanupTestDirectory(testDir);
        }
    }

    [Fact]
    public void StringPoolHandlesEmptyFile()
    {
        var testDir = GetTestDirectory();
        var poolPath = Path.Combine(testDir, "test.pool");

        try
        {
            // Create empty file
            File.WriteAllText(poolPath, "");

            // Should handle gracefully and start fresh
            var pool = CreateStringPool(poolPath);

            // Should be able to add new data
            var id = CallAdd(pool, "new data");
            var retrieved = CallGet(pool, id);

            Assert.Equal("new data", retrieved);

            CallDispose(pool);
        }
        finally
        {
            CleanupTestDirectory(testDir);
        }
    }

    [Fact]
    public void StringPoolHandlesMissingFile()
    {
        var testDir = GetTestDirectory();
        var poolPath = Path.Combine(testDir, "nonexistent.pool");

        try
        {
            // Create pool with non-existent file
            var pool = CreateStringPool(poolPath);

            // Should start fresh
            var id = CallAdd(pool, "first entry");
            var retrieved = CallGet(pool, id);

            Assert.Equal("first entry", retrieved);

            CallDispose(pool);
        }
        finally
        {
            CleanupTestDirectory(testDir);
        }
    }

    [Fact]
    public void StringPoolMaintainsChecksumIntegrity()
    {
        var testDir = GetTestDirectory();
        var poolPath = Path.Combine(testDir, "test.pool");

        try
        {
            // Create pool with data
            var pool1 = CreateStringPool(poolPath);
            CallAdd(pool1, "test string");
            CallDispose(pool1);

            // Read file and verify checksum format
            var lines = File.ReadAllLines(poolPath);

            Assert.True(lines.Length >= 2, "File should have checksum + at least one data line");
            Assert.True(lines[0].StartsWith("#CHECKSUM:"), "First line should be checksum");

            // Parse checksum value
            var checksumLine = lines[0].Substring(10);
            Assert.True(int.TryParse(checksumLine, out var checksum), "Checksum should be valid integer");

            // Reopen - should validate checksum successfully
            var pool2 = CreateStringPool(poolPath);
            CallDispose(pool2);
        }
        finally
        {
            CleanupTestDirectory(testDir);
        }
    }
}
