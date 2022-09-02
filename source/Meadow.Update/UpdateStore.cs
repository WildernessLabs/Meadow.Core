﻿using Meadow;
using System.Collections;
using System.Text.Json;

public class UpdateStore : IEnumerable<UpdateInfo>
{
    private const string UpdateInfoFileName = "info.json";

    private List<UpdateMessage> _updates = new List<UpdateMessage>();
    private DirectoryInfo _storeDirectory;

    internal UpdateStore(string dataDirectory)
    {
        // each update is a subdirectory of the store
        _storeDirectory = new DirectoryInfo(dataDirectory);
        if (!_storeDirectory.Exists)
        {
            _storeDirectory.Create();
        }
        else
        {
            // load from persistence
            foreach(var d in _storeDirectory.GetDirectories())
            {
                // load the update info
                var infoFile = d.GetFiles(UpdateInfoFileName).FirstOrDefault();
                if(infoFile == null)
                {
                    // not a valid update
                    // TODO: should we delete this folder?
                    Resolver.Log.Warn($"Invalid Update: {d.Name}");
                    continue;
                }
                UpdateMessage? info;
                try
                {
                    var json = File.ReadAllText(infoFile.FullName);
                    info = JsonSerializer.Deserialize<UpdateMessage>(json);

                    if (info == null)
                    {
                        Resolver.Log.Warn($"Invalid update json for {d.Name}");
                        continue;
                    }
                }
                catch(Exception ex)
                {
                    Resolver.Log.Warn($"Error getting update info for {d.Name}: {ex.Message}");
                    continue;
                }

                // has this update already been applied?
                var zipInfo = d.GetFiles("*.zip").FirstOrDefault();

                if (info.Applied)
                {
                    // it's been applied.  Make sure no binary is still hanging around consuming space
                    if (zipInfo != null)
                    {
                        try
                        {
                            zipInfo.Delete();
                        }
                        catch(Exception ex)
                        {
                            Resolver.Log.Warn($"Error deleting update binary for {d.Name}: {ex.Message}");
                        }
                    }
                }
                else
                {
                    info.Retrieved = false;

                    // has this update already been retieved?  double check, don't just believe the info
                    // we have valid info, have we downloaded?
                    if (zipInfo != null)
                    {
                        // TODO: check the download hash to make sure it's good?

                        info.Retrieved = true;
                    }
                }

                _updates.Add(info);
            }
        }
    }

    internal void Add(UpdateMessage info)
    {
        // create a folder
        var di = _storeDirectory.CreateSubdirectory(info.ID);

        // persist this update
        info.Retrieved = false;
        info.Applied = false;
        var json = JsonSerializer.Serialize(info);

        var dest = Path.Combine(di.FullName, UpdateInfoFileName);
        File.WriteAllText(dest, json);

        _updates.Add(info);
    }

    public void Clear()
    {
        _updates.Clear();
        foreach (var d in _storeDirectory.EnumerateDirectories())
        {
            foreach(var file in d.EnumerateFiles())
            {
                file.Delete();
            }
            d.Delete();
        }
    }

    internal bool TryGetMessage(string id, out UpdateMessage? message)
    {
        message = _updates.FirstOrDefault(m => m.MpakID == id);
        return message != null;
    }

    internal FileStream GetUpdateFileStream(string updateID)
    {
        // make sure the update folder exists
        var dest = Path.Combine(_storeDirectory.FullName, updateID);
        var di = new DirectoryInfo(dest);
        if(!di.Exists)
        {
            di.Create();
        }

        var fi = new FileInfo(Path.Combine(dest, $"{updateID}.zip"));
        if(fi.Exists)
        {
            fi.Delete();
        }

        return fi.Create();
    }

    public IEnumerator<UpdateInfo> GetEnumerator()
    {
        return _updates.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
