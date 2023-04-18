// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.DTOs
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
        public const string EndSessionEndpoint = "end_session_endpoint";
        public const string AcrValuesSupported = "acr_values_supported";
        public const string IdTokenSigningAlgValuesSupported = "id_token_signing_alg_values_supported";
        public const string IdTokenEncryptionAlgValuesSupported = "id_token_encryption_alg_values_supported";
        public const string IdTokenEncryptionEncValuesSupported = "id_token_encryption_enc_values_supported";
        public const string UserInfoSigningAlgValuesSupported = "userinfo_signing_alg_values_supported";
        public const string UserInfoEncryptionAlgValuesSupported = "userinfo_encryption_alg_values_supported";
        public const string UserInfoEncryptionEncValuesSupported = "userinfo_encryption_enc_values_supported";
        public const string ClaimsSupported = "claims_supported";
        public const string ClaimsParameterSupported = "claims_parameter_supported";
        public const string BackchannelTokenDeliveryModesSupported = "backchannel_token_delivery_modes_supported";
        public const string BackchannelAuthenticationEndpoint = "backchannel_authentication_endpoint";
        public const string BackchannelAuthenticationRequestSigningAlgValues = "backchannel_authentication_request_signing_alg_values_supported";
        public const string BackchannelUserCodeParameterSupported = "backchannel_user_code_parameter_supported";
        public const string FrontChannelLogoutSupported = "frontchannel_logout_supported";
        public const string FrontChannelLogoutSessionSupported = "frontchannel_logout_session_supported";
        public const string BackchannelLogoutSupported = "backchannel_logout_supported";
        public const string BackchannelLogoutSessionSupported = "backchannel_logout_session_supported";
        public const string GrantManagementActionRequired = "grant_management_action_required";
        public const string GrantManagementEndpoint = "grant_management_endpoint";
        public const string GrantManagementActionsSupported = "grant_management_actions_supported";
        public const string AuthorizationSigningAlgValuesSupported = "authorization_signing_alg_values_supported";
        public const string AuthorizationEncryptionAlgValuesSupported = "authorization_encryption_alg_values_supported";
        public const string AuthorizationEncryptionEncValuesSupported = "authorization_encryption_enc_values_supported";
        public const string MtlsEndpointAliases = "mtls_endpoint_aliases";
        public const string PushedAuthorizationRequestEndpoint = "pushed_authorization_request_endpoint";
        public const string RequirePushedAuthorizationRequests = "require_pushed_authorization_requests";
        public const string AuthorizationDetailsSupported = "authorization_details_supported";
        public const string CredentialOfferEndpoint = "credential_offer_endpoint";
    }
}
