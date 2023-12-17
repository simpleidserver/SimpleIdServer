// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Ethr.Services;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Store;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Ethr
{
    public class DIDEthrGenerator : IDIDGenerator
    {
        private readonly IIdentityDocumentConfigurationStore _identityDocumentConfigurationStore;
        private readonly IDIDRegistryServiceFactory _didRegistryServiceFactory;

        public DIDEthrGenerator(IIdentityDocumentConfigurationStore identityDocumentConfigurationStore, IDIDRegistryServiceFactory didRegistryServiceFactory)
        {
            _identityDocumentConfigurationStore = identityDocumentConfigurationStore;
            _didRegistryServiceFactory = didRegistryServiceFactory;
        }

        public string Method { get; } = Constants.Type;

        public async Task<DIDGenerationResult> Generate(Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            const string publicKeyName = "publicKey";
            const string networkName = "network";
            if (!parameters.ContainsKey(publicKeyName)) return DIDGenerationResult.Nok(string.Format(ErrorMessages.MISSING_PARAMETER, publicKeyName));
            if (!parameters.ContainsKey(networkName)) return DIDGenerationResult.Nok(string.Format(ErrorMessages.MISSING_PARAMETER, networkName));
            var publicKey = parameters[publicKeyName];
            var network = await _identityDocumentConfigurationStore.Get(parameters[networkName], cancellationToken);
            var key = SignatureKeyBuilder.NewES256K();
            var did = $"did:{Constants.Type}:{networkName}:{publicKey}";
            var identityDocument = IdentityDocumentBuilder.New(did)
                // .AddVerificationMethod(key, Did.Constants.VerificationMethodTypes.Secp256k1VerificationKey2018)
                .Build();
            var sync = new IdentityDocumentSynchronizer(_didRegistryServiceFactory);
            await sync.Sync(identityDocument, publicKey, network);
            var hex = key.PrivateKey.ToHex();
            return DIDGenerationResult.Ok(did, hex);
        }
    }
}
