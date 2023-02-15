// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    public static class ClientReducers
    {
        #region SearchClientsState

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
            var newClient = new Client { ClientId = act.ClientId, CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow, ClientType = act.ClientType };
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
                Clients = clients,
                Count = clients.Count()
            };
        }

        [ReducerMethod]
        public static SearchClientsState ReduceRemoveSelectedClientsAction(SearchClientsState state, RemoveSelectedClientsAction act) => state with
        {
            IsLoading = true
        };

        [ReducerMethod]
        public static SearchClientsState ReduceRemoveSelectedClientsSuccessAction(SearchClientsState state, RemoveSelectedClientsSuccessAction act)
        {
            var clients = state.Clients?.ToList();
            if (clients == null) return state;
            clients = clients.Where(c => !act.ClientIds.Contains(c.Value.ClientId)).ToList();
            return state with
            {
                Clients = clients,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static SearchClientsState ReduceToggleClientSelectionAction(SearchClientsState state, ToggleClientSelectionAction act)
        {
            var clients = state.Clients?.ToList();
            if (clients == null) return state;
            var selectedClient = clients.Single(c => c.Value.ClientId == act.ClientId);
            selectedClient.IsSelected = act.IsSelected;
            return state with
            {
                Clients = clients
            };
        }

        [ReducerMethod]
        public static SearchClientsState ReduceToggleAllClientSelectionAction(SearchClientsState state, ToggleAllClientSelectionAction act)
        {
            var clients = state.Clients?.ToList();
            if (clients == null) return state;
            foreach (var client in clients) client.IsSelected = act.IsSelected;
            return state with
            {
                Clients = clients
            };
        }

        #endregion

        #region AddClientState

        [ReducerMethod]
        public static AddClientState ReduceAddSpaClientAction(AddClientState state, AddSpaClientAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddWebsiteApplicationAction(AddClientState state, AddWebsiteApplicationAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddMobileApplicationAction(AddClientState state, AddMobileApplicationAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddWsFederationApplicationAction(AddClientState state, AddWsFederationApplicationAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddDeviceApplicationAction(AddClientState state, AddDeviceApplicationAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddClientSuccessAction(AddClientState state, AddClientSuccessAction act) => new(isAdding: false, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddClientFailureAction(AddClientState state, AddClientFailureAction act) => new(isAdding: false, errorMessage: act.ErrorMessage);

        #endregion

        #region UpdateClientState

        [ReducerMethod]
        public static UpdateClientState ReduceUpdateClientDetailsAction(UpdateClientState state, UpdateClientDetailsAction act) => new(isUpdating: true);

        [ReducerMethod]
        public static UpdateClientState ReduceUpdateClientDetailsSuccessAction(UpdateClientState state, UpdateClientDetailsSuccessAction act) => new(isUpdating: false);

        #endregion

        #region ClientState

        [ReducerMethod]
        public static ClientState ReduceGetScopeAction(ClientState state, GetClientAction act) => state with
        {
            IsLoading = true
        };

        [ReducerMethod]
        public static ClientState ReduceGetClientSuccessAction(ClientState state, GetClientSuccessAction act) => state with
        {
            IsLoading = false,
            Client = act.Client
        };

        [ReducerMethod]
        public static ClientState ReduceUpdateClientDetailsSuccessAction(ClientState state, UpdateClientDetailsSuccessAction act)
        {
            var client = state.Client;
            client.RedirectionUrls = act.RedirectionUrls.Split(';');
            client.UpdateClientName(act.ClientName);
            client.PostLogoutRedirectUris = act.PostLogoutRedirectUris.Split(';');
            client.FrontChannelLogoutSessionRequired = act.FrontChannelLogoutSessionRequired;
            client.FrontChannelLogoutUri = act.FrontChannelLogoutUri;
            client.BackChannelLogoutUri = act.BackChannelLogoutUri;
            client.BackChannelLogoutSessionRequired = act.BackChannelLogoutSessionRequired;
            var grantTypes = new List<string>();
            if (act.IsClientCredentialsGrantTypeEnabled)
                grantTypes.Add(ClientCredentialsHandler.GRANT_TYPE);
            if (act.IsPasswordGrantTypeEnabled)
                grantTypes.Add(PasswordHandler.GRANT_TYPE);
            if (act.IsRefreshTokenGrantTypeEnabled)
                grantTypes.Add(RefreshTokenHandler.GRANT_TYPE);
            if (act.IsAuthorizationCodeGrantTypeEnabled)
                grantTypes.Add(AuthorizationCodeHandler.GRANT_TYPE);
            if (act.IsCIBAGrantTypeEnabled)
                grantTypes.Add(CIBAHandler.GRANT_TYPE);
            if (act.IsUMAGrantTypeEnabled)
                grantTypes.Add(UmaTicketHandler.GRANT_TYPE);
            client.GrantTypes = grantTypes;
            client.IsConsentDisabled = !act.IsConsentEnabled;
            return state with
            {
                Client = client
            };
        }

        #endregion

        #region ClientScopesState

        [ReducerMethod]
        public static ClientScopesState ReduceGetClientAction(ClientScopesState state, GetClientAction act) => state with
        {
            IsLoading = true
        };

        [ReducerMethod]
        public static ClientScopesState ReduceGetClientSuccessAction(ClientScopesState state, GetClientSuccessAction act) => state with
        {
            IsLoading = false,
            Count = act.Client.Scopes.Count(),
            Scopes = act.Client.Scopes.Select(s => new SelectableClientScope(s)).ToList()
        };

        [ReducerMethod]
        public static ClientScopesState ReduceToggleAllClientScopeSelectionAction(ClientScopesState state, ToggleAllClientScopeSelectionAction act)
        {
            var scopes = state.Scopes.ToList();
            foreach (var scope in scopes)
                scope.IsSelected = act.IsSelected;
            return state with
            {
                Scopes = scopes
            };
        }

        [ReducerMethod]
        public static ClientScopesState ReduceToggleClientScopeSelectionAction(ClientScopesState state, ToggleClientScopeSelectionAction act)
        {
            var scopes = state.Scopes.ToList();
            scopes.First(s => s.Value.Name == act.ScopeName).IsSelected = act.IsSelected;
            return state with
            {
                Scopes = scopes
            };
        }

        [ReducerMethod]
        public static ClientScopesState ReduceRemoveSelectedClientScopesSuccessAction(ClientScopesState state, RemoveSelectedClientScopesSuccessAction act)
        {
            var scopes = state.Scopes.ToList();
            scopes = scopes.Where(s => !act.ScopeNames.Contains(s.Value.Name)).ToList();
            return state with
            {
                Scopes = scopes
            };
        }

        #endregion
    }
}
