using SimpleIdServer.Jwt;
using SimpleIdServer.Uma.Api.Token.Fetchers;

namespace SimpleIdServer.Uma
{
    public class UMAHostOptions
    {
        public UMAHostOptions()
        {
            SignInScheme = UMAConstants.SignInScheme;
            ChallengeAuthenticationScheme = UMAConstants.ChallengeAuthenticationScheme;
            ValidityPeriodPermissionTicketInSeconds = 5 * 60;
            OpenIdJsonWebKeySignature = new JsonWebKey();
            DefaultClaimTokenFormat = OpenIDClaimTokenFormat.NAME;
            OpenIdRedirectUrl = "https://openid.net/";
            RequestSubmittedInterval = 5;
        }

        /// <summary>
        /// Sign in scheme
        /// </summary>
        public string SignInScheme { get; set; }
        /// <summary>
        /// Challenge authentication scheme.
        /// </summary>
        public string ChallengeAuthenticationScheme { get; set; }
        /// <summary>
        /// The minimum amount of time in seconds that the client SHOULD wait between polling requests to the token endpoint. 
        /// </summary>
        public int RequestSubmittedInterval { get; set; }
        /// <summary>
        /// Get the default token claim format.
        /// </summary>
        public string DefaultClaimTokenFormat { get; set; }
        /// <summary>
        /// Validity of permission ticket in seconds.
        /// </summary>
        public double ValidityPeriodPermissionTicketInSeconds { get; set; }
        /// <summary>
        /// Claims interation endpoint URI to which to redirect the end-user requesting party at the authorization server.
        /// </summary>
        public string OpenIdRedirectUrl { get; set; }
        /// <summary>
        /// JSON Web Key Signature used to check the signature of received claim_token.
        /// </summary>
        public JsonWebKey OpenIdJsonWebKeySignature { get; set; }
    }
}
