using SimpleIdServer.Jwt.Jws;

namespace SimpleIdServer.OpenID.UI.ViewModels
{
    public class RevokeSessionViewModel
    {
        public RevokeSessionViewModel(string revokeSessionCallbackUrl, JwsPayload idTokenHint)
        {
            RevokeSessionCallbackUrl = revokeSessionCallbackUrl;
            IdTokenHint = idTokenHint;
        }

        public string RevokeSessionCallbackUrl { get; set; }
        public JwsPayload IdTokenHint { get; set; }
    }
}
