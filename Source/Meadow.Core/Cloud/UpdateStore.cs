using System;
using System.IO;
using System.IO.Hashing;

namespace Meadow.Update;

using static Resolver;

/// <summary>
/// A local persistent store for holding and accounting for Meadow update data
/// </summary>
internal class UpdateStore
{
    private const string UpdateInfoFileName = "info.json";
    private const string MpakPartialFileName = "mpak.partial";
    private const string MpakFileName = "mpak.zip";

    private readonly DirectoryInfo _storeDirectory;
    private readonly string manifest_path;
    private readonly string mpak_partial_path;
    private readonly string mpak_path;

    private FileStream? mpak_stream;

    internal enum States { Empty, Manifest, Mpak };

    internal UpdateMessage? Manifest;
    internal string MpakPath
    {
        get
        {
            if (State >= States.Mpak)
                return mpak_path;
            else throw new InvalidOperationException("Mpak not available.");
        }
    }

    internal States State
    {
        get
        {
            var state = States.Empty;
            try
            {
                if (File.Exists(manifest_path))
                {
                    var json = File.ReadAllText(manifest_path);
                    Manifest = JsonSerializer.Deserialize<UpdateMessage>(json);
                    state = States.Manifest;

                    if (File.Exists(mpak_path))
                    {
                        state = States.Mpak;
                    }
                }
            }
            catch (Exception e)
            {
                Clear();
                Log.Error(e, "update store");
            }
            return state;
        }
        set
        {
            if (value == States.Empty)
                Clear();
            else
                throw new InvalidOperationException();
        }
    }

    internal UpdateStore(string path)
    {
        _storeDirectory = new DirectoryInfo(path);
        if (!_storeDirectory.Exists)
            _storeDirectory.Create();

        manifest_path = Path.Join(path, UpdateInfoFileName);
        mpak_partial_path = Path.Join(path, MpakPartialFileName);
        mpak_path = Path.Join(path, MpakFileName);
    }

    /// <summary>
    /// Adds a manifest to the update store.
    /// </summary>
    /// <param name="manifest_path">The path to the manifest file.</param>
    public void AddManifest(string manifest_path)
    {
        Log.Debug("AddManifest()", "update store");
        if (State != States.Empty)
            throw new Exception("Cannot add a manifest, store already includes an update");

        Log.Debug($"{Path.GetFullPath(manifest_path)} -> {this.manifest_path}");

        File.Move(manifest_path, this.manifest_path);
    }

    /// <summary>
    /// Calculates the CRC32 hash of a file
    /// </summary>
    /// <param name="file">The file to hash</param>
    private string GetFileHash(string file)
    {
        var crc32 = new Crc32();
        using var fs = File.OpenRead(file);
        crc32.Append(fs);

        var checkSum = crc32.GetCurrentHash();
        Array.Reverse(checkSum); // make big endian
        return BitConverter.ToString(checkSum).Replace("-", "").ToLower();
    }

    /// <summary>
    /// Deletes all local update archives and information
    /// </summary>
    private void Clear()
    {
        MeadowOS.DeleteDirectoryContents(_storeDirectory, deleteDirectory: false);
    }

    internal FileStream StartMpak()
    {
        Log.Debug("StartMpak()", "update store");
        if (State != States.Manifest)
            throw new Exception("Cannot add a MPAK, no manifest in store");

        var fi = new FileInfo(mpak_partial_path);
        if (fi.Exists)
        {
            // Continue the download, instead of discarding the downloaded bytes
            Log.Debug($"Resuming download from existing file: {fi.FullName}. Appending to continue.");
            mpak_stream = fi.Open(FileMode.Append, FileAccess.Write);
        }
        else
        {
            mpak_stream = fi.Create();
        }
        return mpak_stream;
    }

    internal void DeleteMpak()
    {
        Log.Debug("ResetMpak()", "update store");
        if (State != States.Manifest)
            throw new Exception("Cannot delete MPAK, no manifest in store");

        mpak_stream?.Dispose();
        mpak_stream = null;

        File.Delete(mpak_partial_path);
        File.Delete(mpak_path);
    }

    internal bool ValidateMpak(string hashAlgorithm, string expectedHash, out string actualHash)
    {
        Log.Debug("ValidateMpak()", "update store");
        if (State != States.Manifest)
        {
            throw new Exception("Cannot validate a MPAK, no manifest in store");
        }

        if (!string.Equals(hashAlgorithm, "meadowCrc", StringComparison.OrdinalIgnoreCase))
        {
            throw new MpakValidationFailedException($"Hash algorithm '{hashAlgorithm}' is not supported.");
        }

        mpak_stream?.Dispose();
        mpak_stream = null;

        var mpakFilePath = 
              File.Exists(mpak_partial_path) ? mpak_partial_path 
            : File.Exists(mpak_path) ? mpak_path 
            : throw new FileNotFoundException("Cannot validate a MPAK, no mpak in store");

        actualHash = GetFileHash(mpakFilePath);

        return string.Equals(actualHash, expectedHash, StringComparison.Ordinal);
    }

    internal void CompleteMpak()
    {
        Log.Debug("CompleteMpak()", "update store");
        if (State != States.Manifest)
            throw new Exception("Cannot add a MPAK, no manifest in store");

        mpak_stream?.Dispose();
        mpak_stream = null;

        File.Copy(mpak_partial_path, mpak_path);
        File.Delete(mpak_partial_path);
    }
}