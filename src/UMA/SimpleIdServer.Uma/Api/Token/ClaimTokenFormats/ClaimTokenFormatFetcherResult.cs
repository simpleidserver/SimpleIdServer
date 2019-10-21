using SimpleIdServer.Jwt.Jws;

namespace SimpleIdServer.Uma.Api.Token.Fetchers
{
    public class ClaimTokenFormatFetcherResult
    {
        public ClaimTokenFormatFetcherResult(string subject, JwsPayload payload)
        {
            Subject = subject;
            Payload = payload;
        }

        public string Subject { get; set; }
        public JwsPayload Payload { get; set; }
    }
}
