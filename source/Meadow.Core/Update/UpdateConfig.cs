namespace Meadow.Update
{
    public class UpdateConfig
    {
        public string UpdateServer { get; set; } = "localhost"; // = "20.253.228.77";
        public int UpdatePort { get; set; } = 1883;
        public string ClientID { get; set; } = "simple_client";
        public string RootTopic { get; set; } = "Meadow.OtA";
        public int CloudConnectRetrySeconds { get; set; } = 15;
    }
}