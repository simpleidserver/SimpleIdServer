// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.CredentialIssuer
{
    public static class ErrorMessages
    {
        public const string CREDENTIAL_REQUEST_INVALID = "the incoming request is invalid";
        public const string MISSING_PARAMETER = "the parameter {0} is missing";
        public const string MISSING_OPENID_CREDENTIAL_FORMAT = "the authorization_details must contain a format";
        public const string EXISTING_CREDENTIAL_TEMPLATE = "Credential template with the type {0} already exists";
        public const string INVALID_ACCESS_TOKEN_SCOPE = "access token has an invalid scope";
        public const string INVALID_PROOF_SIG = "the proof signature is not correct";
        public const string INVALID_PROOF_JWT = "the proof is not a well formed JWT token";
        public const string INVALID_PROOF_JWT_TYP = "the proof typ must be equals to {0}";
        public const string INVALID_PROOF_C_NONCE = "the credential nonce (c_nonce) is not valid";
        public const string UNSUPPORTED_FORMAT = "the format {0} is not supported";
        public const string UNKNOWN_ACCESS_TOKEN = "either the access token has been revoked or is invalid";
        public const string UNKNOWN_CREDENTIAL_OFFER = "the credential offer doesn't exist";
        public const string UNKNOWN_CREDENTIAL_TEMPLATE = "the credential template {0} doesn't exist";
        public const string UNKNOWN_CREDENTIAL_DISPLAY_TEMPLATE = "the credential template display {0} doesn't exist";
        public const string UNKNOWN_CREDENTIAL_DISPLAY_PARAMETER = "the credential template parameter {0} doesn't exist";
        public const string UNKNOWN_WALLET_CLIENT_ID = "the wallet {0} doesn't exist";
        public const string UNKNOWN_PROOF_TYPE = "the proof type {0} is unknown";
        public const string UNKNOWN_USER = "the user {0} doesn't exist";
        public const string UNSUPPORTED_CREDENTIALS_FORMAT = "credential formats {0} are not supported";
        public const string MALFROMED_INCOMING_REQUEST = "the incoming request is malformed";
        public const string CREDOFFER_IS_INVALID = "credential offer is invalid";
        public const string CREDOFFER_IS_EXPIRED = "credential offer is expired";
        public const string UNAUTHORIZED_TO_ACCESS = "you are not authorized to access to {0}";
        public const string UNAUHTORIZED_TO_ACCESS_TO_CREDENTIAL = "you are not authorized to access to the credential";
        public const string NO_CREDENTIAL_FOUND = "no credential found";
        public const string USER_HAS_NO_DID = "user doesn't have a valid DID";
        public const string DID_METHOD_NOT_SUPPORTED = "DID method is not supported";
    }
}
