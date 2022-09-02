using Meadow;
using System.Collections;
using System.Text.Json;

public class UpdateStore : IEnumerable<UpdateInfo>
{
    private List<UpdateInfo> _updates = new List<UpdateInfo>();
    private DirectoryInfo _storeDirectory;

    internal UpdateStore(string dataDirectory)
    {
        // each update is a subdirectory of the store
        var di = new DirectoryInfo(dataDirectory);
        if (!di.Exists)
        {
            di.Create();
        }
        else
        {
            // load from persistence
            foreach(var d in di.GetDirectories())
            {
                // load the update info
                var infoFile = d.GetFiles("info.json").FirstOrDefault();
                if(infoFile == null)
                {
                    // not a valid update
                    // TODO: should we delete this folder?
                    Resolver.Log.Warn($"Invalid Update: {d.Name}");
                    continue;
                }
                UpdateInfo? info;
                try
                {
                    var json = File.ReadAllText(infoFile.FullName);
                    info = JsonSerializer.Deserialize<UpdateInfo>(json);

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
            }
        }
    }

    internal void Add(UpdateInfo info)
    {
        _updates.Add(info);
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
