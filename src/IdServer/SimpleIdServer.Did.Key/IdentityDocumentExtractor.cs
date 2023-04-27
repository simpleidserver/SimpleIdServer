// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var verificationMethod = ExtractValidationMethod(id, sigKey);
            builder.AddVerificationMethod(verificationMethod, _options.PublicKeyFormat, KeyPurposes.VerificationKey);
            builder.AddContext(Constants.TypeToContextUrl[_options.PublicKeyFormat]);
            return Task.FromResult(builder.Build());
        }

        IdentityDocumentVerificationMethod ExtractValidationMethod(string id, ISignatureKey signatureKey)
        {
            var publicKeyPayload = signatureKey.GetPublicKey(true);
            var kvp = SignatureKeyFactory.AlgToMulticodec.First(kvp => kvp.Value == signatureKey.Name);
            var payload = new List<byte>();
            payload.AddRange(kvp.Key.Item2);
            payload.AddRange(publicKeyPayload);
            var publicKeyMultibase = $"z{Encoding.Base58Encoding.Encode(payload.ToArray())}";
            var verificationMethodId = $"{id}#{publicKeyMultibase}";
            var identityVerificationMethod = new IdentityDocumentVerificationMethod
            {
                Id = verificationMethodId,
                Controller = id,
                Type = _options.PublicKeyFormat
            };
            if(_options.PublicKeyFormat == Did.Constants.VerificationMethodTypes.Ed25519VerificationKey2020)
                identityVerificationMethod.AdditionalParameters.Add(Constants.AdditionalVerificationMethodFields.PublicKeyMultibase, publicKeyMultibase);

            if (_options.PublicKeyFormat == Did.Constants.VerificationMethodTypes.JsonWebKey2020)
                identityVerificationMethod.PublicKeyJwk = signatureKey.GetPublicKeyJwk();

            return identityVerificationMethod;
        }
    }
}