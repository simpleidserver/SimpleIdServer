namespace SimpleIdServer.MobileApp
{
    public static class Constants
    {
        public static readonly string[] Scopes = new string[] { "openid", "profile" };
        public static readonly string AuthStateKey = "authState";
        public static string AuthServiceDiscoveryKey = "authServiceDiscovery";
        public static readonly string RedirectUri = "com.companyname.simpleidserver.mobileapp:/oauth2redirect";
        public static readonly string ClientId = "native";
        public static readonly string BaseUrl = "https://192.168.1.63:60010";
        public static readonly string DiscoveryEndpoint = $"{BaseUrl}/.well-known/openid-configuration";
        public static readonly string ConfirmAuthReqId = $"{BaseUrl}/bc-authorize/confirm";
    }
}
