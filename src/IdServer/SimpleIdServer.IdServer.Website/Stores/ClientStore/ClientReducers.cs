// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    public static class ClientReducers
    {
        #region Search clients

        [ReducerMethod]
        public static SearchClientsState ReduceSearchClientsAction(SearchClientsState state, SearchClientsAction act) => new(isLoading: true, clients: new List<Client>());

        [ReducerMethod]
        public static SearchClientsState ReduceSearchClientsSuccessAction(SearchClientsState state, SearchClientsSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                Clients = act.Clients.Select(c => new SelectableClient(c)),
                Count = act.Clients.Count()
            };
        }

        [ReducerMethod]
        public static SearchClientsState ReduceAddClientSuccessAction(SearchClientsState state, AddClientSuccessAction act)
        {
            var clients = state.Clients?.ToList();
            if (clients == null) return state;
            var newClient = new Client { ClientId = act.ClientId, CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow };
            if(!string.IsNullOrWhiteSpace(act.ClientName))
                newClient.Translations.Add(new Translation
                {
                    Key = "client_name",
                    Language = act.Language,
                    Value = act.ClientName
                });
            clients.Add(new SelectableClient(newClient) { IsNew = true });            
            return state with
            {
                Clients = clients
            };
        }

        #endregion

        #region Add client

        [ReducerMethod]
        public static AddClientState ReduceAddSpaClientAction(AddClientState state, AddSpaClientAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddClientSuccessAction(AddClientState state, AddClientSuccessAction act) => new(isAdding: false, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddClientFailureAction(AddClientState state, AddClientFailureAction act) => new(isAdding: false, errorMessage: act.ErrorMessage);

        #endregion
    }
}
