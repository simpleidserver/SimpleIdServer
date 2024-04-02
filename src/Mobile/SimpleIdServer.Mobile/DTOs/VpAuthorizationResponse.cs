namespace SimpleIdServer.Mobile.DTOs
{
    public class VpAuthorizationResponse
    {
        public string VpToken { get; set; }
        public string State { get; set; }
        public string PresentationSubmission { get; set; }

        public Dictionary<string, string> ToQueries()
            => new Dictionary<string, string>
            {
                { "vp_token", VpToken },
                { "state", State },
                { "presentation_submission", PresentationSubmission }
            };
    }
}