public class UpdateInfo
{
    public string ID { get; internal set; }
    public UpdateType UpdateType { get; internal set; }
    public DateTime PublicationDate { get; internal set; }
    public string Version { get; internal set; }
    public long DownloadSize { get; internal set; }
    public string? Summary { get; internal set; }
    public string? Detail { get; internal set; }
    public bool Retrieved { get; internal set; }
    public bool Applied { get; internal set; }
    public bool DownloadHash { get; internal set; }
}
