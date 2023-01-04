// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.DTOs
{
    public static class OAuthConfigurationNames
    {
        public const string Issuer = "issuer";
        public const string AuthorizationEndpoint = "authorization_endpoint";
        public const string TokenEndpoint = "token_endpoint";
        public const string JwksUri = "jwks_uri";
        public const string RegistrationEndpoint = "registration_endpoint";
        public const string ScopesSupported = "scopes_supported";
        public const string ResponseTypesSupported = "response_types_supported";
        public const string ResponseModesSupported = "response_modes_supported";
        public const string GrantTypesSupported = "grant_types_supported";
        public const string TokenEndpointAuthMethodsSupported = "token_endpoint_auth_methods_supported";
        public const string TokenEndpointAuthSigningAlgValuesSupported = "token_endpoint_auth_signing_alg_values_supported";
        public const string ServiceDocumentation = "service_documentation";
        public const string UiLocalesSupported = "ui_locales_supported";
        public const string OpPolicyUri = "op_policy_uri";
        public const string OpTosUri = "op_tos_uri";
        public const string RevocationEndpoint = "revocation_endpoint";
        public const string RevocationEndpointAuthMethodsSupported = "revocation_endpoint_auth_methods_supported";
        public const string RevocationEndpointAuthSigningAlgValuesSupported = "revocation_endpoint_auth_signing_alg_values_supported";
        public const string IntrospectionEndpoint = "introspection_endpoint";
        public const string IntrospectionEndpointAuthMethodsSupported = "introspection_endpoint_auth_methods_supported";
        public const string IntrospectionEndpointAuthSigningAlgValuesSupported = "introspection_endpoint_auth_signing_alg_values_supported";
        public const string CodeChallengeMethodsSupported = "code_challenge_methods_supported";
        /// <summary>
        /// Boolean value indicating server support for mutual-TLS client certificate-bound access tokens.
        /// </summary>
        public const string TlsClientCertificateBoundAccessTokens = "tls_client_certificate_bound_access_tokens";
        public const string MtlsEndpointAliases = "mtls_endpoint_aliases";
    }
}