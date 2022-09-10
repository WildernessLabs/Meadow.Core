using System;

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
        public DateTime PublishedOn { get; set; }
        public string ID { get; protected set; }
        public UpdateType UpdateType { get; set; }
        public string Version { get; set; }
        public long DownloadSize { get; set; }
        public string? Summary { get; set; }
        public string? Detail { get; set; }
        public bool Retrieved { get; set; }
        public bool Applied { get; set; }
        public string DownloadHash { get; set; }
    }
}