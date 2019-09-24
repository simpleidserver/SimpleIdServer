// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OAuth.DTOs
{
    public static class RegisterRequestParameters
    {
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
    }
}