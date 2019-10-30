// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OpenID
{
    public static class ErrorMessages
    {
        public const string OPENID_SCOPE_MISSING = "openid scope is missing";
        public const string INVALID_IDTOKENHINT = "id_token_hint is invalid";
        public const string INVALID_POST_LOGOUT_REDIRECT_URI = "post_logout_redirect_uri parameter is invalid";
        public const string INVALID_SUBJECT_IDTOKENHINT = "subject contained in id_token_hint is invalid";
        public const string INVALID_AUDIENCE_IDTOKENHINT = "audience contained in id_token_hint is invalid";
        public const string INVALID_CLAIMS = "claims {0} are invalid";
        public const string POST_LOGOUT_REDIRECT_URI_IS_INVALID = "post_logout_redirect_uri {0} is invalid";
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
        public const string INVALID_HTTPS_SECTOR_IDENTIFIER_URI = "sector_identifier_uri doesn't contain https scheme";
        public const string INVALID_HTTPS_REDIRECT_URI = "redirect_uri does not contain https scheme";
        public const string INVALID_LOCALHOST_REDIRECT_URI = "redirect_uri must not contain localhost";
        public const string INVALID_SUBJECT_TYPE = "subject_type is invalid";
        public const string UNSUPPORTED_IDTOKEN_SIGNED_RESPONSE_ALG = "id_token_signed_response_alg is not supported";
        public const string UNSUPPORTED_IDTOKEN_ENCRYPTED_RESPONSE_ALG = "id_token_encrypted_response_alg is not supported";
        public const string UNSUPPORTED_IDTOKEN_ENCRYPTED_RESPONSE_ENC = "id_token_encrypted_response_enc is not supported";
        public const string UNSUPPORTED_USERINFO_SIGNED_RESPONSE_ALG = "userinfo_signed_response_alg is not supported";
        public const string UNSUPPORTED_USERINFO_ENCRYPTED_RESPONSE_ALG = "userinfo_encrypted_response_alg is not supported";
        public const string UNSUPPORTED_USERINFO_ENCRYPTED_RESPONSE_ENC = "userinfo_encrypted_response_enc is not supported";
        public const string UNSUPPORTED_REQUEST_OBJECT_SIGNING_ALG = "request_object_signing_alg is not supported";
        public const string UNSUPPORTED_REQUEST_OBJECT_ENCRYPTION_ALG = "request_object_encryption_alg is not supported";
        public const string UNSUPPORTED_REQUEST_OBJECT_ENCRYPTION_ENC = "request_object_encryption_enc is not supported";
        public const string MISSING_ID_TOKEN_HINT = "id_token_hint parameter is missing";
        public const string MISSING_TOKEN = "missing token";
        public const string MISSING_ISSUER_CLAIM = "the issuer claim is missing";
        public const string MISSING_AUD_CLAIM = "the audience claim is missing";
        public const string MISSING_RESPONSE_TYPE_CLAIM = "the response type claim is missing";
        public const string MISSING_CLIENT_ID_CLAIM = "the client identifier claim is missing";
        public const string INVALID_AUDIENCE = "invalid audiences";
        public const string NO_CONSENT = "no consent has been accepted";
    }
}