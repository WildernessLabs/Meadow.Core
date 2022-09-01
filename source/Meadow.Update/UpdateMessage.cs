internal class UpdateMessage
{
    /*
{"mpakId":"75a71caf0ffc482487016ad1515bdbbb","mpakDownloadUrl":"https://www.meadowcloud.co/api/orgs/1430ba69051e486bb4eb287edf836fcb/packages/75a71caf0ffc482487016ad1515bdbbb/download","targetDevices":["*"],"publishedOn":"2022-08-25T04:45:23.1692833Z"}
    */
    public string MpakID { get; set; }
    public string MpakDownloadUrl { get; set; }
    public string[] TargetDevices { get; set; }
    public DateTime PublishedOn { get; set; }
}
