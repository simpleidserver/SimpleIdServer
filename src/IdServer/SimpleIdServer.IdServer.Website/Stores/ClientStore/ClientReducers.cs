// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.DPoP;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers.Models;
using SimpleIdServer.IdServer.Saml.Idp.Extensions;
using SimpleIdServer.IdServer.Website.Stores.ScopeStore;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    public static class ClientReducers
    {
        #region SearchClientsState

        [ReducerMethod]
        public static SearchClientsState ReduceSearchClientsAction(SearchClientsState state, SearchClientsAction act) => new(isLoading: true, clients: new List<Domains.Client>());

        [ReducerMethod]
        public static SearchClientsState ReduceSearchClientsSuccessAction(SearchClientsState state, SearchClientsSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                Clients = act.Clients.Select(c => new SelectableClient(c)),
                Count = act.Count
            };
        }

        [ReducerMethod]
        public static SearchClientsState ReduceUpdateClientDetailsAction(SearchClientsState state, UpdateClientDetailsAction act)
        {
            var clients = state.Clients;
            var client = clients?.SingleOrDefault(c => c.Value.Id == act.Id);
            if (client != null) client.Value.UpdateClientName(act.ClientName);
            return state with
            {
                Clients = clients
            };
        }

        [ReducerMethod]
        public static SearchClientsState ReduceAddClientSuccessAction(SearchClientsState state, AddClientSuccessAction act)
        {
            var clients = state.Clients?.ToList();
            if (clients == null) return state;
            var newClient = new Domains.Client { Id = act.Id, ClientId = act.ClientId, CreateDateTime = DateTime.Now, UpdateDateTime = DateTime.Now, ClientType = act.ClientType };
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
            clients = clients.Where(c => !act.Ids.Contains(c.Value.Id)).ToList();
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
        public static AddClientState ReduceAddHighlySecuredWebsiteApplicationAction(AddClientState state, AddHighlySecuredWebsiteApplicationAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddHighlySecuredWebsiteApplicationWithGrantMgtSupportAction(AddClientState state, AddHighlySecuredWebsiteApplicationWithGrantMgtSupportAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddMobileApplicationAction(AddClientState state, AddMobileApplicationAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddWsFederationApplicationAction(AddClientState state, AddWsFederationApplicationAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddDeviceApplicationAction(AddClientState state, AddExternalDeviceApplicationAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddClientSuccessAction(AddClientState state, AddClientSuccessAction act) => new(isAdding: false, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceAddClientFailureAction(AddClientState state, AddClientFailureAction act) => new(isAdding: false, errorMessage: act.ErrorMessage);

        [ReducerMethod]
        public static AddClientState ReduceAddSamlSpApplicationAction(AddClientState state, AddSamlSpApplicationAction act) => new(isAdding: true, errorMessage: null);

        [ReducerMethod]
        public static AddClientState ReduceStartAddClientAction(AddClientState state, StartAddClientAction act)
        {
            return state with
            {
                ErrorMessage = null
            };
        }

        #endregion

        #region UpdateClientState

        [ReducerMethod]
        public static UpdateClientState ReduceStartGenerateClientKeyAction(UpdateClientState state, StartGenerateClientKeyAction act)
        {
            return new UpdateClientState(false)
            {
                ErrorMessage = null
            };
        }

        [ReducerMethod]
        public static UpdateClientState ReduceUpdateClientDetailsAction(UpdateClientState state, UpdateClientDetailsAction act) => new(isUpdating: true);

        [ReducerMethod]
        public static UpdateClientState ReduceUpdateClientDetailsSuccessAction(UpdateClientState state, UpdateClientDetailsSuccessAction act) => new(isUpdating: false);

        [ReducerMethod]
        public static UpdateClientState ReduceUpdateClientDetailsFailureAction(UpdateClientState state, UpdateClientDetailsFailureAction act) => new(isUpdating: false);

        [ReducerMethod]
        public static UpdateClientState ReduceUpdateAdvancedClientSettingsFailureAction(UpdateClientState state, UpdateAdvancedClientSettingsFailureAction act) => new(isUpdating: false);

        [ReducerMethod]
        public static UpdateClientState ReduceAddClientScopesAction(UpdateClientState state, AddClientScopesAction act) => new(isUpdating: true);

        [ReducerMethod]
        public static UpdateClientState ReduceAddClientScopesSuccessAction(UpdateClientState state, AddClientScopesSuccessAction act) => new(isUpdating: false);

        [ReducerMethod]
        public static UpdateClientState ReduceGenerateSigKeySuccessAction(UpdateClientState state, GenerateSigKeySuccessAction act) => state with
        {
            ErrorMessage = null
        };

        [ReducerMethod]
        public static UpdateClientState ReduceGenerateEncKeySuccessAction(UpdateClientState state, GenerateEncKeySuccessAction act) => state with
        {
            ErrorMessage = null
        };


        [ReducerMethod]
        public static UpdateClientState ReduceGenerateSigKeyFailureAction(UpdateClientState state, GenerateKeyFailureAction act) => state with
        {
            ErrorMessage = act.ErrorMessage
        };

        [ReducerMethod]
        public static UpdateClientState ReduceUpdateClientCredentialsAction(UpdateClientState state, UpdateClientCredentialsAction act) => state with
        {
            IsUpdating = true
        };

        [ReducerMethod]
        public static UpdateClientState ReduceUpdateClientCredentialsSuccessAction(UpdateClientState state, UpdateClientCredentialsSuccessAction act) => state with
        {
            IsUpdating = false
        };

        [ReducerMethod]
        public static UpdateClientState ReduceUpdateAdvancedClientSettingsAction(UpdateClientState state, UpdateAdvancedClientSettingsAction act) => state with
        {
            IsUpdating = true
        };

        [ReducerMethod]
        public static UpdateClientState ReduceUpdateAdvancedClientSettingsSuccessAction(UpdateClientState state, UpdateAdvancedClientSettingsSuccessAction act) => state with
        {
            IsUpdating = false
        };

        #endregion

        #region ClientState

        [ReducerMethod]
        public static ClientState ReduceGetClientAction(ClientState state, GetClientAction act) => state with
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
            client.IsRedirectUrlCaseSensitive = act.IsRedirectUrlCaseSensitive;
            client.RedirectToRevokeSessionUI = act.RedirectToRevokeSessionUI;
            client.DefaultAcrValues = act.DefaultAcrValues;
            client.IsPublic = act.IsPublic;
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
            if (act.IsDeviceGrantTypeEnabled)
                grantTypes.Add(DeviceCodeHandler.GRANT_TYPE);
            if (act.IsTokenExchangeEnabled)
                grantTypes.Add(TokenExchangeHandler.GRANT_TYPE);
            client.GrantTypes = grantTypes;
            client.IsConsentDisabled = !act.IsConsentEnabled;
            client.TokenExchangeType = act.TokenExchangeType;
            client.SetSaml2SpMetadataUrl(act.MetadataUrl);
            client.SetUseAcsArtifact(act.UseAcs);
            client.AccessTokenType = act.AccessTokenType;
            client.Parameters = client.Parameters;
            return state with
            {
                Client = client
            };
        }

        [ReducerMethod]
        public static ClientState ReduceUpdateJWKSUrlSuccessAction(ClientState state, UpdateJWKSUrlSuccessAction act)
        {
            var client = state.Client;
            client.JwksUri = act.JWKSUrl;
            return state with
            {
                Client = client
            };
        }

        [ReducerMethod]
        public static ClientState ReduceUpdateClientCredentialsSuccessAction(ClientState state, UpdateClientCredentialsSuccessAction act)
        {
            var client = state.Client;
            client.TokenEndPointAuthMethod = act.AuthMethod;
            if (client.TokenEndPointAuthMethod == OAuthClientSecretPostAuthenticationHandler.AUTH_METHOD || client.TokenEndPointAuthMethod == OAuthClientSecretBasicAuthenticationHandler.AUTH_METHOD)
                client.ClientSecret = act.ClientSecret;
            else if (client.TokenEndPointAuthMethod == OAuthClientTlsClientAuthenticationHandler.AUTH_METHOD)
            {
                client.TlsClientAuthSubjectDN = act.TlsClientAuthSubjectDN;
                client.TlsClientAuthSanDNS = act.TlsClientAuthSanDNS;
                client.TlsClientAuthSanEmail = act.TlsClientAuthSanEmail;
                client.TlsClientAuthSanIP = act.TlsClientAuthSanIP;
            }

            client.UpdateDateTime = DateTime.Now;
            return state with
            {
                Client = client
            };
        }

        [ReducerMethod]
        public static ClientState ReduceUpdateAdvancedClientSettingsSuccessAction(ClientState state, UpdateAdvancedClientSettingsSuccessAction act)
        {
            var client = state.Client;
            client.IdTokenSignedResponseAlg = act.IdTokenSignedResponseAlg;
            client.TokenSignedResponseAlg = act.TokenSignedResponseAlg;
            client.AuthorizationSignedResponseAlg = act.AuthorizationSignedResponseAlg;
            client.AuthorizationDataTypes = act.AuthorizationDataTypes?.ToList();
            client.ResponseTypes = act.ResponseTypes?.ToList();
            client.DPOPBoundAccessTokens = act.IsDPoPRequired;
            client.IsDPOPNonceRequired = act.IsDPoPNonceRequired;
            client.DPOPNonceLifetimeInSeconds = act.DPOPNonceLifetimeInSeconds;
            client.TokenExpirationTimeInSeconds = act.TokenExpirationTimeInSeconds;
            client.UserCookieExpirationTimeInSeconds = act.UserCookieExpirationTimeInSeconds;
            client.AuthorizationCodeExpirationInSeconds = act.AuthorizationCodeExpirationInSeconds;
            client.DeviceCodeExpirationInSeconds = act.DeviceCodeExpirationInSeconds;
            client.DeviceCodePollingInterval = act.DeviceCodePollingInterval;
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
        public static ClientScopesState ReduceGetClientSuccessAction(ClientScopesState state, GetClientSuccessAction act)
        {
            var scopes = act.Client.Scopes.Where(s => s.Type == ScopeTypes.IDENTITY || s.Type == ScopeTypes.APIRESOURCE).ToList();
            return state with
            {
                IsLoading = false,
                Count = scopes.Count(),
                Scopes = scopes.Select(s => new SelectableClientScope(s)).ToList()
            };
        }

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

        [ReducerMethod]
        public static ClientScopesState ReduceSearchScopesAction(ClientScopesState state, SearchScopesAction act) => state with
        {
            IsEditableScopesLoading = true
        };

        [ReducerMethod]
        public static ClientScopesState ReduceSearchScopesSuccessAction(ClientScopesState state, SearchScopesSuccessAction act)
        {
            var result = act.Scopes.OrderBy(s => s.Name).Select(s => new EditableClientScope(s)
            {
                IsPresent = state.Scopes.Any(sc => sc.Value.Name == s.Name)
            }).ToList();
            return state with
            {
                EditableScopes = result,
                EditableScopesCount = act.Count,
                IsEditableScopesLoading = false
            };
        }

        [ReducerMethod]
        public static ClientScopesState ReduceAddClientScopesSuccessAction(ClientScopesState state, AddClientScopesSuccessAction act)
        {
            var scopes = state.Scopes.ToList();
            var newScopes = act.Scopes.ToList().Select(s => new SelectableClientScope(s)
            {
                IsNew = true
            });
            foreach (var newScope in newScopes)
                scopes.Add(newScope);
            return state with
            {
                Scopes = scopes,
                Count = scopes.Count
            };
        }

        [ReducerMethod]
        public static ClientScopesState ReduceToggleEditableClientScopeSelectionAction(ClientScopesState state, ToggleEditableClientScopeSelectionAction act)
        {
            var lst = state.EditableScopes.ToList();
            var scope = lst.Single(s => s.Value.Name == act.ScopeName);
            scope.IsSelected = act.IsSelected;
            return state with
            {
                EditableScopes = lst
            };
        }

        [ReducerMethod]
        public static ClientScopesState ReduceToggleAllEditableClientScopeSelectionAction(ClientScopesState state, ToggleAllEditableClientScopeSelectionAction act)
        {
            var lst = state.EditableScopes.ToList();
            foreach (var record in lst)
                record.IsSelected = act.IsSelected;
            return state with
            {
                EditableScopes = lst
            };
        }

        #endregion

        #region ClientKeysState

        [ReducerMethod]
        public static ClientKeysState ReduceGetClientAction(ClientKeysState state, GetClientAction act) => state with
        {
            IsLoading = true
        };

        [ReducerMethod]
        public static ClientKeysState ReduceGetClientSuccessAction(ClientKeysState state, GetClientSuccessAction act) => state with
        {
            IsLoading = false,
            Count = act.Client.SerializedJsonWebKeys.Count(),
            Keys = act.Client.SerializedJsonWebKeys.Select(s => new SelectableClientKey(s)).ToList()
        };

        [ReducerMethod]
        public static ClientKeysState ReduceToggleAllClientKeySelectionAction(ClientKeysState state, ToggleAllClientKeySelectionAction act)
        {
            var scopes = state.Keys.ToList();
            foreach (var scope in scopes)
                scope.IsSelected = act.IsSelected;
            return state with
            {
                Keys = scopes
            };
        }

        [ReducerMethod]
        public static ClientKeysState ReduceToggleClientKeySelectionAction(ClientKeysState state, ToggleClientKeySelectionAction act)
        {
            var keys = state.Keys.ToList();
            keys.First(s => s.Value.Kid == act.KeyId).IsSelected = act.IsSelected;
            return state with
            {
                Keys = keys
            };
        }

        [ReducerMethod]
        public static ClientKeysState ReduceAddSigKeySuccessAction(ClientKeysState state, AddSigKeySuccessAction act)
        {
            var keys = state.Keys.ToList();
            var jsonWebKey = act.Credentials.SerializePublicJWK();
            var newKey = new ClientJsonWebKey
            {
                Alg = act.Alg,
                Kid = act.KeyId,
                KeyType = act.KeyType,
                Usage = DefaultTokenSecurityAlgs.JwkUsages.Sig,
                SerializedJsonWebKey = JsonWebKeySerializer.Write(jsonWebKey)
            };
            keys.Add(new SelectableClientKey(newKey) { IsNew = true, Value = newKey });
            return state with
            {
                Keys = keys,
                Count = keys.Count
            };
        }

        [ReducerMethod]
        public static ClientKeysState ReduceAddEncKeySuccessAction(ClientKeysState state, AddEncKeySuccessAction act)
        {
            var keys = state.Keys.ToList();
            var jsonWebKey = act.Credentials.SerializePublicJWK();
            var newKey = new ClientJsonWebKey
            {
                Alg = act.Alg,
                Kid = act.KeyId,
                KeyType = act.KeyType,
                Usage = DefaultTokenSecurityAlgs.JwkUsages.Enc,
                SerializedJsonWebKey = JsonWebKeySerializer.Write(jsonWebKey)
            };
            keys.Add(new SelectableClientKey(newKey) { IsNew = true, Value = newKey });
            return state with
            {
                Keys = keys,
                Count = keys.Count
            };
        }

        [ReducerMethod]
        public static ClientKeysState ReduceRemoveSelectedClientKeysAction(ClientKeysState state, RemoveSelectedClientKeysAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static ClientKeysState ReduceRemoveSelectedClientKeysSuccessAction(ClientKeysState state, RemoveSelectedClientKeysSuccessAction act)
        {
            var keys = state.Keys.ToList();
            keys = keys.Where(j => !act.KeyIds.Contains(j.Value.Kid)).ToList();
            return state with
            {
                Keys = keys,
                Count = keys.Count,
                IsLoading = false
            };
        }

        #endregion

        #region ClientRolesState

        [ReducerMethod]
        public static ClientRolesState ReduceGetClientAction(ClientRolesState state, GetClientAction act) => new(new List<Scope>(), true);

        [ReducerMethod]
        public static ClientRolesState ReduceGetClientSuccessAction(ClientRolesState state, GetClientSuccessAction act) => new(act.Client.Scopes.Where(s => s.Type == ScopeTypes.ROLE), false);

        [ReducerMethod]
        public static ClientRolesState ReduceAddClientRoleAction(ClientRolesState state, AddClientRoleAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static ClientRolesState ReduceClientRoleFailureAction(ClientRolesState state, AddClientRoleFailureAction act)
        {
            return state with
            {
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static ClientRolesState ReduceAddClientRoleSuccessAction(ClientRolesState state, AddClientRoleSuccessAction act)
        {
            var roles = state.Roles.ToList();
            roles.Add(new SelectableClientRole(act.Scope)
            {
                IsNew = true
            });
            return state with
            {
                IsLoading = false,
                Roles = roles,
                Nb = roles.Count()
            };
        }

        [ReducerMethod]
        public static ClientRolesState ReduceRemoveSelectedScopesAction(ClientRolesState state, RemoveSelectedScopesAction act)
        {
            if (!act.IsRole) return state;
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static ClientRolesState ReduceRemoveSelectedScopesSuccessAction(ClientRolesState state, RemoveSelectedScopesSuccessAction act)
        {
            if (!act.IsRole) return state;
            var roles = state.Roles.ToList();
            roles = roles.Where(r => !act.ScopeIds.Contains(r.Value.Id)).ToList();
            return state with
            {
                IsLoading = false,
                Roles = roles,
                Nb = roles.Count()
            };
        }

        [ReducerMethod]
        public static ClientRolesState ReduceToggleAllClientRolesAction(ClientRolesState state, ToggleAllClientRolesAction act)
        {
            var roles = state.Roles.ToList();
            foreach(var role in roles)
                role.IsSelected = act.IsSelected;
            return state with
            {
                Roles = roles
            };
        }

        [ReducerMethod]
        public static ClientRolesState ReduceToggleClientRoleAction(ClientRolesState state, ToggleClientRoleAction act)
        {
            var roles = state.Roles.ToList();
            var role = roles.Single(r => r.Value.Id == act.RoleId);
            role.IsSelected = act.IsSelected;
            return state with
            {
                Roles = roles
            };
        }

        #endregion
    }
}
