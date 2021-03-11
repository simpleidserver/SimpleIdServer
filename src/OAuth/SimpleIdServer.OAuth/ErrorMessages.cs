// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OAuth
{
    public class ErrorMessages
    {
        public const string UNKNOWN_CLIENT = "unknown client {0}";
        public const string UNKNOWN_AUTH_METHOD = "unknown authentication method : {0}";
        public const string UNKNOWN_TOKEN_TYPE_HINT = "unknown token type hint : {0}";
        public const string UNAUTHORIZED_CLIENT = "unauthorized client";
        public const string NO_CLIENT_SECRET = "no client secret";
        public const string DUPLICATE_SCOPES = "duplicate scopes : {0}";
        public const string INVALID_SCOPES = "invalid scopes : {0}";
        public const string BAD_CODE_VERIFIER = "code_verifier is invalid";
        public const string BAD_CODE_CHALLENGE_METHOD = "transform algorithm {0} is not supported";
        public const string BAD_SOFTWARE_STATEMENT_SIGNATURE = "software statement signature is invalid";
        public const string BAD_GRANT_TYPE = "bad grant type";
        public const string BAD_JWS_SOFTWARE_STATEMENT = "software statement is not a JWS token";
        public const string BAD_ISSUER_SOFTWARE_STATEMENT = "software statement issuer is not trusted";
        public const string BAD_USER_CREDENTIAL = "bad user credential";
        public const string BAD_CLIENT_CREDENTIAL = "bad client credential";
        public const string BAD_CLIENT_GRANT_TYPE = "grant type {0} not supported by the client";
        public const string BAD_CLIENT_ASSERTION_DECRYPTION = "bad client assertion decryption";
        public const string BAD_CLIENT_ASSERTION_FORMAT = "bad client assertion format";
        public const string BAD_CLIENT_ASSERTION_SIGNATURE = "bad client assertion signature";
        public const string BAD_CLIENT_ASSERTION_ISSUER = "bad client assertion issuer";
        public const string BAD_RESPONSE_TYPES = "response types {0} are not supported";
        public const string BAD_RESPONSE_TYPES_CLIENT = "response types {0} are not supported by the client";
        public const string BAD_ACCESS_TOKEN = "access token is not correct";
        public const string BAD_REDIRECT_URI = "redirect uri {0} is not correct";
        public const string BAD_CLIENT_URI = "client uri {0} is not correct";
        public const string BAD_LOGO_URI = "logo uri {0} is not correct";
        public const string BAD_POLICY_URI = "policy uri {0} is not correct";
        public const string BAD_TOS_URI = "tos uri {0} is not correct";
        public const string BAD_JWKS_URI = "jwks uri {0} is not correct";
        public const string BAD_TOKEN = "bad token";
        public const string BAD_CLIENT_ASSERTION_AUDIENCES = "bad client assertion audiences";
        public const string BAD_CLIENT_ASSERTION_EXPIRED = "bad client assertion expired";
        public const string BAD_REFRESH_TOKEN = "bad refresh token";
        public const string BAD_AUTHORIZATION_CODE = "bad authorization code";
        public const string BAD_RESPONSE_TYPE = "response type must equals to 'code'";
        public const string BAD_RESPONSE_MODE = "response mode {0} is not supported";
        public const string MISSING_PARAMETER = "missing parameter {0}";
        public const string MISSING_RESPONSE_TYPES = "missing response types {0}";
        public const string MISSING_RESPONSE_TYPE = "valid response type must be passed for the grant type {0}";
        public const string MISSING_ACCESS_TOKEN = "access token is missing";
        public const string UNSUPPORTED_TOKEN_ENCRYPTED_RESPONSE_ALG = "token_encrypted_response_alg is not supported";
        public const string UNSUPPORTED_TOKEN_ENCRYPTED_RESPONSE_ENC = "token_encrypted_response_enc is not supported";
        public const string UNSUPPORTED_TOKEN_SIGNED_RESPONSE_ALG = "token_signed_response_alg is not supported";
        public const string UNSUPPORTED_SCOPES = "scopes {0} are not supported";
        public const string UNSUPPORTED_GRANT_TYPE = "grant type {0} is not supported";
        public const string UNSUPPORTED_GRANT_TYPES = "grant types {0} are not supported";
        public const string REFRESH_TOKEN_NOT_ISSUED_BY_CLIENT = "refresh token has not been issued by the client";
        public const string NO_REGISTERED_REDIRECTURI = "no redirect uri has been registered";
        public const string DUPLICATE_JWKS = "jwks and jwks_uri parameters cannot be passed at the same time";
        public const string LOGIN_IS_REQUIRED = "login is required";
        public const string AUTHORIZATION_CODE_ALREADY_USED = "authorization code has already been used, all tokens previously issued have been revoked";
        public const string ACCESS_TOKEN_REJECTED = "access token has been rejected";
        public const string REFRESH_TOKEN_IS_EXPIRED = "refresh token is expired";
        public const string ACCESS_TOKEN_VALID_CLIENT = "access token can be used for the client '{0}' and not for '{1}'";
        public const string CLIENT_IDENTIFIER_MUST_BE_IDENTICAL = "client identifier must be identical";
        public const string CLIENT_SECRET_MUST_BE_IDENTICAL = "client secret must be identical";
        public const string NO_CLIENT_CERTIFICATE = "no client certificate";
        public const string CERTIFICATE_SUBJECT_INVALID = "certificate subject is invalid";
        public const string CERTIFICATE_SAN_DNS_INVALID = "certificate san DNS is invalid";
        public const string CERTIFICATE_SAN_EMAIL_INVALID = "certificate san EMAIL is invalid";
        public const string CERTIFICATE_SAN_IP_INVALID = "certificate san IP is invalid";
        public const string REQUEST_OBJECT_IS_EXPIRED = "request object is expired";
    }
}