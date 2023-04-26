// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Key
{
    public class IdentityDocumentExtractor : IIdentityDocumentExtractor
    {
        private string _publicKeyFormat;

        public IdentityDocumentExtractor()
        {
            _publicKeyFormat = SimpleIdServer.Did.Constants.VerificationMethodTypes.Ed25519VerificationKey2020;
        }

        public string Type => Constants.Type;

        public Task<IdentityDocument> Extract(string id, CancellationToken cancellationToken)
        {
            var did = IdentityDocumentIdentifierParser.InternalParse(id);
            var builder = KeyIdentityDocumentBuilder.NewKey(did.Identifier);
            var sigKey = SignatureKeyFactory.Build(did.PublicKey);
            builder.AddVerificationMethod(sigKey, _publicKeyFormat, KeyPurposes.VerificationKey);
            builder.AddContext(Constants.TypeToContextUrl[_publicKeyFormat]);
            return Task.FromResult(builder.Build());
        }
    }
}