// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Did.Ethr;
using SimpleIdServer.Did.Ethr.Models;
using SimpleIdServer.Did.Ethr.Services;
using SimpleIdServer.IdServer.Website.Resources;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.EthrNetworkStore
{
    public class EthrNetworkEffects
    {
        private readonly IIdentityDocumentConfigurationStore _identityDocumentConfigurationStore;
        private readonly ISmartContractServiceFactory _smartContractServiceFactory;

        public EthrNetworkEffects(IIdentityDocumentConfigurationStore identityDocumentConfigurationStore, ISmartContractServiceFactory smartContractServiceFactory)
        {
            _identityDocumentConfigurationStore = identityDocumentConfigurationStore;
            _smartContractServiceFactory = smartContractServiceFactory;
        }

        [EffectMethod]
        public async Task Handle(SearchEthrNetworksAction action, IDispatcher dispatcher)
        {
            IQueryable<NetworkConfiguration> query = _identityDocumentConfigurationStore.Query();
            if (!string.IsNullOrWhiteSpace(action.Filter))
                query = query.Where(SanitizeExpression(action.Filter));

            if (!string.IsNullOrWhiteSpace(action.OrderBy))
                query = query.OrderBy(SanitizeExpression(action.OrderBy));

            var nb = query.Count();
            var networks = await query.ToListAsync(CancellationToken.None);
            dispatcher.Dispatch(new SearchEthrNetworksSuccessAction { Networks = networks, Count = nb });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(AddEthrNetworkAction action, IDispatcher dispatcher)
        {
            var exists = await _identityDocumentConfigurationStore.Query().AnyAsync(c => c.Name == action.Name);
            if (exists)
            {
                dispatcher.Dispatch(new AddEthrNetworkFailureAction { ErrorMessage = string.Format(Global.EthrNetworkExists, action.Name) });
                return;
            }

            var network = new NetworkConfiguration { Name = action.Name, RpcUrl = action.RpcUrl, PrivateAccountKey = action.PrivateAccountKey, CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow };
            _identityDocumentConfigurationStore.Add(network);
            await _identityDocumentConfigurationStore.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddEthrNetworkSuccessAction { Name = action.Name, RpcUrl = action.RpcUrl, PrivateAccountKey = action.PrivateAccountKey });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedEthrContractAction action, IDispatcher dispatcher)
        {
            var ethrContracts = await _identityDocumentConfigurationStore.Query().Where(c => action.Names.Contains(c.Name)).ToListAsync();
            foreach(var ethrContract in ethrContracts) _identityDocumentConfigurationStore.Remove(ethrContract);
            await _identityDocumentConfigurationStore.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedEthrContractSuccessAction { Names = action.Names });
        }

        [EffectMethod]
        public async Task Handle(DeployEthrContractAction action, IDispatcher dispatcher)
        {
            var accountService = _smartContractServiceFactory.Build();
            try
            {
                var networkConfiguration = await _identityDocumentConfigurationStore.Query().SingleAsync(c => c.Name == action.Name);
                var result = await accountService.UseAccount(networkConfiguration.PrivateAccountKey).UseNetwork(action.Name).DeployContractAndGetService();
                networkConfiguration.UpdateDateTime = DateTime.UtcNow;
                networkConfiguration.ContractAdr = result.ContractHandler.ContractAddress;
                await _identityDocumentConfigurationStore.SaveChanges(CancellationToken.None);
                dispatcher.Dispatch(new DeployEthrContractSuccessAction { Name = networkConfiguration.Name, ContractAdr = networkConfiguration.ContractAdr });
            }
            catch(Exception ex)
            {
                dispatcher.Dispatch(new DeployEthrContractFailureAction { ErrorMessage = ex.Message });
            }
        }
    }

    public class SearchEthrNetworksAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public string? ScopeName { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchEthrNetworksSuccessAction
    {
        public IEnumerable<NetworkConfiguration> Networks { get; set; } = new List<NetworkConfiguration>();
        public int Count { get; set; }
    }

    public class AddEthrNetworkAction
    {
        public string Name { get; set; }
        public string RpcUrl { get; set; }
        public string PrivateAccountKey { get; set; }
    }

    public class AddEthrNetworkSuccessAction
    {
        public string Name { get; set; }
        public string RpcUrl { get; set; }
        public string PrivateAccountKey { get; set; }
    }

    public class AddEthrNetworkFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class DeployEthrContractAction
    {
        public string Name { get; set; }
    }

    public class DeployEthrContractSuccessAction
    {
        public string Name { get; set; }
        public string ContractAdr { get; set; }
    }

    public class DeployEthrContractFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class RemoveSelectedEthrContractAction
    {
        public IEnumerable<string> Names { get; set; }
    }

    public class RemoveSelectedEthrContractSuccessAction
    {
        public IEnumerable<string> Names { get; set; }
    }

    public class ToggleEthrContractAction
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class SelectOneEthrContractAction
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ToggleAllEthrContractAction
    {
        public bool IsSelected { get; set; }
    }
}
