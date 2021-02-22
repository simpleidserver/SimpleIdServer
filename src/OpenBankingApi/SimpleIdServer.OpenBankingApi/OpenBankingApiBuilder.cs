// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.OpenBankingApi.Domains.Account;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenBankingApi.Persistences.InMemory;
using System.Collections.Concurrent;

namespace SimpleIdServer.OpenBankingApi
{
    public class OpenBankingApiBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public OpenBankingApiBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public IServiceCollection ServiceCollection { get => _serviceCollection; }

        public OpenBankingApiBuilder AddAccounts(ConcurrentBag<AccountAggregate> accounts)
        {
            _serviceCollection.AddSingleton<IAccountRepository>(new InMemoryAccountRepository(accounts));
            return this;
        }
    }
}
