namespace SimpleIdServer.MobileApp
{
    public static class Constants
    {
        public static readonly string[] Scopes = new string[] { "openid", "profile" };
        public static readonly string AuthStateKey = "authState";
        public static string AuthServiceDiscoveryKey = "authServiceDiscovery";
        public static readonly string RedirectUri = "com.companyname.simpleidserver.mobileapp:/oauth2redirect";
        public static readonly string ClientId = "native";
        public static readonly string BaseUrl = "https://simpleidserver.northeurope.cloudapp.azure.com/openbanking";
        public static readonly string DiscoveryEndpoint = $"{BaseUrl}/.well-known/openid-configuration";
        public static readonly string ConfirmAuthReqId = $"{BaseUrl}/bc-authorize/confirm";
        public static readonly string RejectAuthReqId = $"{BaseUrl}/bc-authorize/reject";
    }
}
