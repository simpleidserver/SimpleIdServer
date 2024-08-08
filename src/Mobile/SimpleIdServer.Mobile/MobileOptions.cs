namespace SimpleIdServer.Mobile
{
    public class MobileOptions
    {
        public const string LocalhostIp = "openid.simpleidserver.com";
        public bool IgnoreHttps { get; set; } = true;
        public string IdServerUrl { get; set; } = $"https://{LocalhostIp}/master";
        // public const string LocalhostIp = "192.168.50.125";
        // public string IdServerUrl { get; set; } = $"https://{LocalhostIp}:5001/master";
        public string WsServer { get; set; }
        public bool IsDev { get; set; } = true;
        public string ClientId { get; set; } = "walletClient";
        public string ClientSecret = "password";
        public string VcVersion { get; set; } = SupportedVcVersions.ESBI;
    }
}