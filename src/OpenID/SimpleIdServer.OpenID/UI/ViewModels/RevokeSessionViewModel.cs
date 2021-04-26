using SimpleIdServer.Jwt.Jws;

namespace SimpleIdServer.OpenID.UI.ViewModels
{
    public class RevokeSessionViewModel
    {
        public RevokeSessionViewModel(string revokeSessionCallbackUrl, JwsPayload idTokenHint, string frontChannelLogout)
        {
            RevokeSessionCallbackUrl = revokeSessionCallbackUrl;
            IdTokenHint = idTokenHint;
            FrontChannelLogout = frontChannelLogout;
        }

        public string RevokeSessionCallbackUrl { get; set; }
        public JwsPayload IdTokenHint { get; set; }
        public string FrontChannelLogout { get; set; }
    }
}
