// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Did.Store;

namespace SimpleIdServer.Did.Ethr.Services
{
    public interface ISmartContractServiceFactory
    {
        SmartContractService Build();
    }

    public class SmartContractServiceFactory : ISmartContractServiceFactory
    {
        private readonly IIdentityDocumentConfigurationStore _store;
        private readonly IOptions<DidEthrOptions> _options;

        public SmartContractServiceFactory(IIdentityDocumentConfigurationStore store, IOptions<DidEthrOptions> options)
        {
            _store = store;
            _options = options;
        }

        public SmartContractService Build() => new SmartContractService(_store, _options);
    }
}
