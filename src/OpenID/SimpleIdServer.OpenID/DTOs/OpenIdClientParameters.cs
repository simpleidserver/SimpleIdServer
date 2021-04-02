// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OpenID.DTOs
{
    public static class OpenIdClientParameters
    {
        public const string ApplicationType = "application_type";
        public const string SectorIdentifierUri = "sector_identifier_uri";
        public const string SubjectType = "subject_type";
        public const string IdTokenSignedResponseAlg = "id_token_signed_response_alg";
        public const string IdTokenEncryptedResponseAlg = "id_token_encrypted_response_alg";
        public const string IdTokenEncryptedResponseEnc = "id_token_encrypted_response_enc";
        public const string UserInfoSignedResponseAlg = "userinfo_signed_response_alg";
        public const string UserInfoEncryptedResponseAlg = "userinfo_encrypted_response_alg";
        public const string UserInfoEncryptedResponseEnc = "userinfo_encrypted_response_enc";
        public const string RequestObjectSigningAlg = "request_object_signing_alg";
        public const string RequestObjectEncryptionAlg = "request_object_encryption_alg";
        public const string RequestObjectEncryptionEnc = "request_object_encryption_enc";
        public const string DefaultMaxAge = "default_max_age";
        public const string RequireAuthTime = "require_auth_time";
        public const string DefaultAcrValues = "default_acr_values";
        public const string PostLogoutRedirectUris = "post_logout_redirect_uris";
        public const string InitiateLoginUri = "initiate_login_uri";
        /// <summary>
        /// One of the following values: poll, ping or push.
        /// </summary>
        public const string BCTokenDeliveryMode = "backchannel_token_delivery_mode";
        /// <summary>
        /// This is the endpoint to which the OP will post a notification after a successful or failed end-user authentication.
        /// </summary>
        public const string BCClientNotificationEndpoint = "backchannel_client_notification_endpoint";
        /// <summary>
        /// The JWS algorithm alg value that the Client will use for signing authentication request.
        /// When omitted, the Client will not send signed authentication requests.
        /// </summary>ù
        public const string BCAuthenticationRequestSigningAlg = "backchannel_authentication_request_signing_alg";
        /// <summary>
        /// Boolean value specifying whether the Client supports the user_code parameter. 
        /// </summary>
        public const string BCUserCodeParameter = "backchannel_user_code_parameter";
    }
}
