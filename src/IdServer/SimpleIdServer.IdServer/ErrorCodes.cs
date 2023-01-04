// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer
{
    public static class ErrorCodes
    {
        public const string INVALID_REQUEST = "invalid_request";
        public const string INVALID_CLIENT = "invalid_client";
        public const string INVALID_REDIRECT_URI = "invalid_redirect_uri";
        public const string INVALID_CLIENT_METADATA = "invalid_client_metadata";
        public const string INVALID_SOFTWARE_STATEMENT = "invalid_software_statement";
        public const string INVALID_GRANT = "invalid_grant";
        public const string INVALID_TOKEN = "invalid_token";
        public const string INVALID_SCOPE = "invalid_scope";
        public const string INVALID_REQUEST_OBJECT = "invalid_request_object";
        public const string UNSUPPORTED_GRANT_TYPE = "unsupported_grant_type";
        public const string UNSUPPORTED_TOKEN_TYPE = "unsupported_token_type";
        public const string UNSUPPORTED_RESPONSE_MODE = "unsupported_response_mode";
        public const string UNSUPPORTED_RESPONSE_TYPE = "unsupported_response_type";
        public const string LOGIN_REQUIRED = "login_required";
        public const string ACCESS_DENIED = "access_denied";
        public const string INVALID_BINDING_MESSAGE = "invalid_binding_message";
        public const string EXPIRED_LOGIN_HINT_TOKEN = "expired_login_hint_token";
        public const string UNKNOWN_USER_ID = "unknown_user_id";
        public const string AUTHORIZATION_PENDING = "authorization_pending";
        public const string EXPIRED_TOKEN = "expired_token";
        public const string SLOW_DOWN = "slow_down";
    }
}
