// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.Did.Ethr.Models;

namespace SimpleIdServer.IdServer.Website.Stores.EthrNetworkStore
{
    public static class EthrNetworkReducers
    {
        #region SearchEthrNetworksState

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceSearchEthrNetworksAction(SearchEthrNetworksState state, SearchEthrNetworksAction act) => new(true, new List<NetworkConfiguration>(), 0);

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceSearchEthrNetworksSuccessAction(SearchEthrNetworksState state, SearchEthrNetworksSuccessAction act) => new(false, act.Networks, act.Count);

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceAddEthrNetworkAction(SearchEthrNetworksState state, AddEthrNetworkAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceAddEthrNetworkFailureAction(SearchEthrNetworksState state, AddEthrNetworkFailureAction act)
        {
            return state with
            {
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceRemoveSelectedEthrContractAction(SearchEthrNetworksState state, RemoveSelectedEthrContractAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceRemoveSelectedEthrContractSuccessAction(SearchEthrNetworksState state, RemoveSelectedEthrContractSuccessAction act)
        {
            var networks = state.Networks.Where(n => !act.Names.Contains(n.Value.Name));
            return state with
            {
                IsLoading = false,
                Networks = networks,
                Count = networks.Count()
            };
        }

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceAddEthrNetworkSuccessAction(SearchEthrNetworksState state, AddEthrNetworkSuccessAction act)
        {
            var result = state.Networks.ToList();
            result.Add(new SelectableEthrNetwork(new NetworkConfiguration { CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow, RpcUrl = act.RpcUrl, Name = act.Name })
            {
                IsNew = true
            });
            return state with
            {
                IsLoading = false,
                Networks = result,
                Count = result.Count
            };
        }

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceDeployEthrContractAction(SearchEthrNetworksState state, DeployEthrContractAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceDeployEthrContractFailureAction(SearchEthrNetworksState state, DeployEthrContractFailureAction act)
        {
            return state with
            {
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceDeployEthrContractSuccessAction(SearchEthrNetworksState state, DeployEthrContractSuccessAction act)
        {
            var results = state.Networks.ToList();
            var result = results.Single(r => r.Value.Name == act.Name);
            result.Value.ContractAdr = act.ContractAdr;
            result.Value.UpdateDateTime = DateTime.UtcNow;
            return state with
            {
                IsLoading = false,
                Networks = results,
                Count = results.Count
            };
        }

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceToggleEthrContractAction(SearchEthrNetworksState state, ToggleEthrContractAction act)
        {
            var results = state.Networks.ToList();
            results.First(r => r.Value.Name == act.Name).IsSelected = act.IsSelected;
            return state with
            {
                Networks = results
            };
        }

        [ReducerMethod]
        public static SearchEthrNetworksState ReduceToggleAllEthrContractAction(SearchEthrNetworksState state, ToggleAllEthrContractAction act)
        {
            var results = state.Networks.ToList();
            foreach (var result in results) result.IsSelected = act.IsSelected;
            return state with
            {
                Networks = results
            };
        }

        #endregion

        #region UpdateEthrNetworkState

        [ReducerMethod]
        public static UpdateEthrNetworkState ReduceAddEthrNetworkAction(UpdateEthrNetworkState state, AddEthrNetworkAction act)
        {
            return state with
            {
                IsUpdating = true
            };
        }

        [ReducerMethod]
        public static UpdateEthrNetworkState ReduceAddEthrNetworkFailureAction(UpdateEthrNetworkState state, AddEthrNetworkFailureAction act)
        {
            return state with
            {
                IsUpdating = false,
                ErrorMessage = act.ErrorMessage
            };
        }

        [ReducerMethod]
        public static UpdateEthrNetworkState ReduceAddEthrNetworkSuccessAction(UpdateEthrNetworkState state, AddEthrNetworkSuccessAction act)
        {
            return state with
            {
                IsUpdating = false,
                ErrorMessage = null
            };
        }

        [ReducerMethod]
        public static UpdateEthrNetworkState ReduceDeployEthrContractAction(UpdateEthrNetworkState state, DeployEthrContractAction act)
        {
            return state with
            {
                IsUpdating = true
            };
        }

        [ReducerMethod]
        public static UpdateEthrNetworkState ReduceDeployEthrContractFailureAction(UpdateEthrNetworkState state, DeployEthrContractFailureAction act)
        {
            return state with
            {
                IsUpdating = false,
                ErrorMessage = act.ErrorMessage
            };
        }

        [ReducerMethod]
        public static UpdateEthrNetworkState ReduceDeployEthrContractSuccessAction(UpdateEthrNetworkState state, DeployEthrContractSuccessAction act)
        {
            return state with
            {
                IsUpdating = false,
                ErrorMessage = null
            };
        }

        #endregion
    }
}
