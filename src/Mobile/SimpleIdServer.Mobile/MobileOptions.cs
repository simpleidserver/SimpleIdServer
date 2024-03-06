namespace SimpleIdServer.Mobile
{
    public class MobileOptions
    {
        public bool IgnoreHttps { get; set; } = true;
        public string IdServerUrl { get; set; } = "https://192.168.50.250:5001/master";
        public string WsServer { get; set; }
        public bool IsDev { get; set; } = false;
    }
}