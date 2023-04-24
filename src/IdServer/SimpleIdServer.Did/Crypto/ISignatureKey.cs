// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Crypto
{
    public interface ISignatureKey
    {
        string Name { get; }
        bool Check(string content, string signature);
        string Sign(string content);
        IdentityDocumentVerificationMethod ExtractVerificationMethodWithPublicKey();
    }
}