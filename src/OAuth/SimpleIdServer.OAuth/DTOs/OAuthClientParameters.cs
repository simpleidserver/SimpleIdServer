// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.OAuth.DTOs
{
    public static class OAuthClientParameters
    {
        public const string ClientId = "client_id";
        public const string ClientSecret = "client_secret";
        public const string ClientIdIssuedAt = "client_id_issued_at";
        public const string ClientSecretExpiresAt = "client_secret_expires_at";
        public const string RegistrationAccessToken = "registration_access_token";
        public const string GrantTypes = "grant_types";
        public const string RedirectUris = "redirect_uris";
        public const string TokenEndpointAuthMethod = "token_endpoint_auth_method";
        public const string ResponseTypes = "response_types";
        public const string ClientName = "client_name";
        public const string ClientUri = "client_uri";
        public const string LogoUri = "logo_uri";
        public const string Scope = "scope";
        public const string Contacts = "contacts";
        public const string TosUri = "tos_uri";
        public const string PolicyUri = "policy_uri";
        public const string JwksUri = "jwks_uri";
        public const string Jwks = "jwks";
        public const string SoftwareId = "software_id";
        public const string SoftwareVersion = "software_version";
        public const string SoftwareStatement = "software_statement";
        public const string TokenSignedResponseAlg = "token_signed_response_alg";
        public const string TokenEncryptedResponseAlg = "token_encrypted_response_alg";
        public const string TokenEncryptedResponseEnc = "token_encrypted_response_enc";
        public const string TokenExpirationTimeInSeconds = "token_expiration_time_seconds";
        public const string RefreshTokenExpirationTimeInSeconds = "refresh_token_expiration_time_seconds";
        public const string RegistrationClientUri = "registration_client_uri";
    }
}
