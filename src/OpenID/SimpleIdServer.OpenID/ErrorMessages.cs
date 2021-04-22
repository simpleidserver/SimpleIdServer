// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OpenID
{
    public static class ErrorMessages
    {
        public const string OPENID_SCOPE_MISSING = "openid scope is missing";
        public const string POST_LOGOUT_REDIRECT_URI_IS_INVALID = "post_logout_redirect_uri {0} is invalid";
        public const string INVALID_IDTOKENHINT = "id_token_hint is invalid";
        public const string INVALID_POST_LOGOUT_REDIRECT_URI = "post_logout_redirect_uri parameter is invalid";
        public const string INVALID_SUBJECT_IDTOKENHINT = "subject contained in id_token_hint is invalid";
        public const string INVALID_AUDIENCE_IDTOKENHINT = "audience contained in id_token_hint is invalid";
        public const string INVALID_CLAIMS = "claims {0} are invalid";
        public const string INVALID_REQUEST_PARAMETER = "request parameter is invalid";
        public const string INVALID_REQUEST_URI_PARAMETER = "request uri parameter is invalid";
        public const string INVALID_JWS_REQUEST_PARAMETER = "request parameter is not a valid JWS token";
        public const string INVALID_SIGNATURE_ALG = "the signature algorithm is invalid";
        public const string INVALID_JWE_REQUEST_PARAMETER = "request parameter is not a valid JWE token";
        public const string INVALID_RESPONSE_TYPE_CLAIM = "the response type claim is invalid";
        public const string INVALID_CLIENT_ID_CLAIM = "the client identifier claim is invalid";
        public const string INVALID_ISSUER_CLAIM = "the issuer claim is invalid";
        public const string INVALID_APPLICATION_TYPE = "application type is invalid";
        public const string INVALID_SECTOR_IDENTIFIER_URI = "sector_identifier_uri is not a valid URI";
        public const string INVALID_INITIATE_LOGIN_URI = "initiate_login_uri is not a valid URI";
        public const string INVALID_HTTPS_SECTOR_IDENTIFIER_URI = "sector_identifier_uri doesn't contain https scheme";
        public const string INVALID_HTTPS_INITIATE_LOGIN_URI = "initiate_login_uri doesn't contain https scheme";
        public const string INVALID_HTTPS_REDIRECT_URI = "redirect_uri does not contain https scheme";
        public const string INVALID_HTTPS_BC_CLIENT_NOTIFICATION_EDP = "client notification endpoint doesn't contain https scheme";
        public const string INVALID_LOCALHOST_REDIRECT_URI = "redirect_uri must not contain localhost";
        public const string INVALID_BC_DELIVERY_MODE = "invalid back channel delivery mode";
        public const string INVALID_BC_CLIENT_NOTIFICATION_EDP = "invalid back channel client notification endpoint";
        public const string INVALID_AUDIENCE = "invalid audiences";
        public const string INVALID_SUBJECT_TYPE = "subject_type is invalid";
        public const string INVALID_AUTH_REQUEST_ID = "auth_req_id doesn't exist";
        public const string NO_ESSENTIAL_ACR_IS_SUPPORTED = "no essential acr is supported";
        public const string UNSUPPORTED_IDTOKEN_SIGNED_RESPONSE_ALG = "id_token_signed_response_alg is not supported";
        public const string UNSUPPORTED_IDTOKEN_ENCRYPTED_RESPONSE_ALG = "id_token_encrypted_response_alg is not supported";
        public const string UNSUPPORTED_IDTOKEN_ENCRYPTED_RESPONSE_ENC = "id_token_encrypted_response_enc is not supported";
        public const string UNSUPPORTED_USERINFO_SIGNED_RESPONSE_ALG = "userinfo_signed_response_alg is not supported";
        public const string UNSUPPORTED_USERINFO_ENCRYPTED_RESPONSE_ALG = "userinfo_encrypted_response_alg is not supported";
        public const string UNSUPPORTED_USERINFO_ENCRYPTED_RESPONSE_ENC = "userinfo_encrypted_response_enc is not supported";
        public const string UNSUPPORTED_REQUEST_OBJECT_SIGNING_ALG = "request_object_signing_alg is not supported";
        public const string UNSUPPORTED_REQUEST_OBJECT_ENCRYPTION_ALG = "request_object_encryption_alg is not supported";
        public const string UNSUPPORTED_REQUEST_OBJECT_ENCRYPTION_ENC = "request_object_encryption_enc is not supported";
        public const string UNSUPPORTED_BC_AUTHENTICATION_REQUEST_SIGNING_ALG = "bc_authentication_request_signing_alg is not supported";
        public const string MISSING_ID_TOKEN_HINT = "id_token_hint parameter is missing";
        public const string MISSING_TOKEN = "missing token";
        public const string MISSING_ISSUER_CLAIM = "the issuer claim is missing";
        public const string MISSING_AUD_CLAIM = "the audience claim is missing";
        public const string MISSING_RESPONSE_TYPE_CLAIM = "the response type claim is missing";
        public const string MISSING_CLIENT_ID_CLAIM = "the client identifier claim is missing";
        public const string NO_CONSENT = "no consent has been accepted";
        public const string CONTENT_TYPE_NOT_SUPPORTED = "the content-type is not correct";
        public const string ACCESS_REVOKED_BY_RESOURCE_OWNER = "access has been revoked by the resource owner";
        public const string CLIENT_NOTIFICATION_TOKEN_MUST_NOT_EXCEED_1024 = "client_notification_token must not exceed 1024 characters";
        public const string CLIENT_NOTIFICATION_TOKEN_MUST_CONTAIN_AT_LEAST_128_BYTES = "client_notification_token must contains at least 128 bytes";
        public const string BINDING_MESSAGE_MUST_NOT_EXCEED = "binding_message must not exceed {0} characters";
        public const string REQUESTED_EXPIRY_MUST_BE_POSITIVE = "requested_expiry must be positive";
        public const string LOGIN_HINT_TOKEN_IS_EXPIRED = "login_hint_token has expired";
        public const string UNKNOWN_USER = "unknown user '{0}'";
        public const string UNKNOWN_PERMISSIONS = "the permissions {0} don't exist";
        public const string ONE_HINT_MUST_BE_PASSED = "only one hint can be passed in the request";
        public const string AUTH_REQUEST_NOT_CONFIRMED = "the authentication request '{0}' has not been confirmed";
        public const string AUTH_REQUEST_NOTIFIED = "the authentication request '{0}' has already been notified to the client";
        public const string AUTH_REQUEST_NOT_NOTIFIED = "the authentication request '{0}' has already been confirmed";
        public const string AUTH_REQUEST_EXPIRED = "the authentication request '{0}' is expired";
        public const string AUTH_REQUEST_NOT_AUTHORIZED_TO_REJECT = "you're not authorized to reject the authorization request";
        public const string AUTH_REQUEST_REJECTED = "the authentication request '{0}' is rejected";
        public const string AUTH_REQUEST_SENT = "the authentication request '{0}' is finished";
        public const string AUTH_REQUEST_NO_AUDIENCE = "the request doesn't contain audience";
        public const string AUTH_REQUEST_BAD_AUDIENCE = "the request doesn't contain correct audience";
        public const string AUTH_REQUEST_NO_ISSUER = "the request doesn't contain issuer";
        public const string AUTH_REQUEST_BAD_ISSUER = "the request doesn't contain correct issuer";
        public const string AUTH_REQUEST_NO_EXPIRATION = "the request doesn't contain expiration time";
        public const string AUTH_REQUEST_IS_EXPIRED = "the request is expired";
        public const string AUTH_REQUEST_MAXIMUM_LIFETIME = "the maximum lifetime of the request is '{0}' seconds";
        public const string AUTH_REQUEST_NO_IAT = "the request doesn't contain iat";
        public const string AUTH_REQUEST_NO_NBF = "the request doesn't contain nbf";
        public const string AUTH_REQUEST_BAD_NBF = "the request cannot be received before '{0}'";
        public const string AUTH_REQUEST_NO_JTI = "the request doesn't contain jti";
        public const string AUTH_REQUEST_ALG_NOT_VALID = "the request must be signed with '{0}' algorithm";
        public const string AUTH_REQUEST_CLIENT_NOT_AUTHORIZED = "the client is not authorized to use the auth_req_id";
        public const string TOO_MANY_AUTH_REQUEST = "too many authentication request : {0}";
        public const string ONLY_PINGORPUSH_MODE_CAN_BE_USED = "only ping or push mode can be used to get tokens";
    }
}