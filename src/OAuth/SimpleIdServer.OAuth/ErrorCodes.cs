// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OAuth
{
    public static class ErrorCodes
    {
        public static string INVALID_REQUEST = "invalid_request";
        public static string INVALID_CLIENT = "invalid_client";
        public static string INVALID_REDIRECT_URI = "invalid_redirect_uri";
        public static string INVALID_CLIENT_METADATA = "invalid_client_metadata";
        public static string INVALID_SOFTWARE_STATEMENT = "invalid_software_statement";
        public static string INVALID_CLIENT_AUTH = "invalid_client_auth";
        public static string INVALID_GRANT = "invalid_grant";
        public static string INVALID_TOKEN = "invalid_token";
        public static string INVALID_SCOPE = "invalid_scope";
        public static string INVALID_REQUEST_OBJECT = "invalid_request_object";
        public static string UNSUPPORTED_GRANT_TYPE = "unsupported_grant_type";
        public static string UNSUPPORTED_TOKEN_TYPE = "unsupported_token_type";
        public static string UNSUPPORTED_RESPONSE_MODE = "unsupported_response_mode";
        public static string UNSUPPORTED_RESPONSE_TYPE = "unsupported_response_type";
        public static string LOGIN_REQUIRED = "login_required";
        public static string ACCESS_DENIED = "access_denied";
    }
}
