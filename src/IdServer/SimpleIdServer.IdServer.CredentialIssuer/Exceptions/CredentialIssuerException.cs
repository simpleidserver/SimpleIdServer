// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Identity.Client;
using System;

namespace SimpleIdServer.CredentialIssuer.Exceptions
{
    public class CredentialIssuerException : Exception
    {
        public CredentialIssuerException(string errorCode, string errorMessage) : base(errorMessage)
        {
            ErrorCode = errorCode;
        }

        public string ErrorCode { get; private set; }
    }
}
