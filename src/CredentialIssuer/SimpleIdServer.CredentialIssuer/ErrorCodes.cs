// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.CredentialIssuer
{
    public static class ErrorCodes
    {
        public const string INVALID_CREDENTIAL_REQUEST = "invalid_credential_request";
        public const string INVALID_CREDENTIAL_OFFER_REQUEST = "invalid_credential_offer_request";
        public const string INVALID_REQUEST = "invalid_request";
        public const string INVALID_TOKEN = "invalid_token";
        public const string INVALID_PROOF = "invalid_proof";
        public const string INVALID_CREDOFFER = "invalid_credoffer";
        public const string UNAUTHORIZED = "unauthorized";
        public const string UNSUPPORTED_CREDENTIAL_FORMAT = "unsupported_credential_format";
        public const string UNSUPPORTED_CREDENTIAL_TYPE = "unsupported_credential_type";
        public const string NOT_FOUND = "not_found";
    }
}
