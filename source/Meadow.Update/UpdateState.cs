namespace Meadow.Update
{
    public enum UpdateState
    {
        Dead,
        Disconnected,
        Connecting,
        Connected,
        Idle,
        UpdateAvailable,
        DownloadingFile,
        UpdateInProgress
    }
}
