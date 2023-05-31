// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Key
{
    public class DIDKeyGenerator : IDIDGenerator
    {
        public string Method => Constants.Type;

        public Task<DIDGenerationResult> Generate(Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            var generateKey = SignatureKeyBuilder.NewES256K();
            var identityDocument = KeyIdentityDocumentBuilder.NewKey(generateKey)
                .AddVerificationMethod(generateKey, Did.Constants.VerificationMethodTypes.Ed25519VerificationKey2020)
                .Build();
            return Task.FromResult(DIDGenerationResult.Ok(identityDocument.Id, generateKey.PrivateKey.ToHex()));
        }
    }
}
