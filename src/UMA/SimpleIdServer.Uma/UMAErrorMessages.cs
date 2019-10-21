namespace SimpleIdServer.Uma
{
    public static class UMAErrorMessages
    {
        public const string BAD_TOKEN_FORMAT = "token format {0} is invalid";
        public const string INVALID_TICKET = "permission ticket is not correct";
        public const string MISSING_PARAMETER = "parameter {0} is missing";
        public const string BAD_CLAIM_TOKEN = "claim_token parameter is not a JWS token";
        public const string INVALID_RESOURCE_ID = "At least one of the provided resource identifiers was not found at the authorization server.";
        public const string REQUEST_NEEDINFO = "The authorization server needs additional information in order for a request to succeed, for example, a provided claim token was invalid or expired, or had an incorrect format, or additional claims are needed to complete the authorization assessment.";
        public const string REQUEST_DENIED = "The client is not authorized to have these permissions.";
        public const string INVALID_SCOPE = "At least one of the scopes included in the request does not match an available scope for any of the resources associated with requested permissions for the permission ticket provided by the client.";
    }
}
