// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Key
{
    public class IdentityDocumentExtractor : IIdentityDocumentExtractor
    {
        private readonly DidKeyOptions _options;

        public IdentityDocumentExtractor(DidKeyOptions options)
        {
            _options = options;
        }

        public string Type => Constants.Type;

        public Task<IdentityDocument> Extract(string id, CancellationToken cancellationToken)
        {
            var did = IdentityDocumentIdentifierParser.InternalParse(id);
            if (!Constants.TypeToContextUrl.ContainsKey(_options.PublicKeyFormat)) throw new InvalidOperationException($"the public key format {_options.PublicKeyFormat} is not supported");
            var builder = KeyIdentityDocumentBuilder.NewKey(did.Identifier);
            var sigKey = SignatureKeyFactory.Build(did.PublicKey);
            builder.AddVerificationMethod(sigKey, _options.PublicKeyFormat, KeyPurposes.VerificationKey);
            builder.AddContext(Constants.TypeToContextUrl[_options.PublicKeyFormat]);
            return Task.FromResult(builder.Build());
        }
    }
}