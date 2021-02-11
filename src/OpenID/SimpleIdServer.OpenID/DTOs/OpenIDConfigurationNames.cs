// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OpenID.DTOs
{
    public static class OpenIDConfigurationNames
    {
        public const string UserInfoEndpoint = "userinfo_endpoint";
        public const string RequestParameterSupported = "request_parameter_supported";
        public const string RequestUriParameterSupported = "request_uri_parameter_supported";
        public const string RequestObjectSigningAlgValuesSupported = "request_object_signing_alg_values_supported";
        public const string RequestObjectEncryptionAlgValuesSupported = "request_object_encryption_alg_values_supported";
        public const string RequestObjectEncryptionEncValuesSupported = "request_object_encryption_enc_values_supported";
        public const string SubjectTypesSupported = "subject_types_supported";
        public const string CheckSessionIframe = "check_session_iframe";
        public const string EndSessionEndpoint = "check_session_endpoint";
        public const string AcrValuesSupported = "acr_values_supported";
        public const string IdTokenSigningAlgValuesSupported = "id_token_signing_alg_values_supported";
        public const string IdTokenEncryptionAlgValuesSupported = "id_token_encryption_alg_values_supported";
        public const string IdTokenEncryptionEncValuesSupported = "id_token_encryption_enc_values_supported";
        public const string UserInfoSigningAlgValuesSupported = "userinfo_signing_alg_values_supported";
        public const string UserInfoEncryptionAlgValuesSupported = "userinfo_encryption_alg_values_supported";
        public const string UserInfoEncryptionEncValuesSupported = "userinfo_encryption_enc_values_supported";
        public const string ClaimsSupported = "claims_supported";
    }
}