namespace Meadow.Update
{
    internal class UpdateMessage : UpdateInfo
    {
        public string MpakID
        {
            get => ID;
            set => ID = value;
        }
        public string MpakDownloadUrl { get; set; }
        public string[] TargetDevices { get; set; }
    }

    public class UpdateInfo
    {
        public DateTime PublishedOn { get; internal set; }
        public string ID { get; protected set; }
        public UpdateType UpdateType { get; internal set; }
        public string Version { get; internal set; }
        public long DownloadSize { get; internal set; }
        public string? Summary { get; internal set; }
        public string? Detail { get; internal set; }
        public bool Retrieved { get; internal set; }
        public bool Applied { get; internal set; }
        public bool DownloadHash { get; internal set; }
    }
}