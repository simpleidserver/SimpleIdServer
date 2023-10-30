// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Saml.Idp.Extensions;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Website.Resources;
using SimpleIdServer.IdServer.WsFederation;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    public class ClientEffects
    {
        private readonly IDbContextFactory<StoreDbContext> _factory;
        private readonly ProtectedSessionStorage _sessionStorage;

        public ClientEffects(IDbContextFactory<StoreDbContext> factory, ProtectedSessionStorage sessionStorage)
        {
            _factory = factory;
            _sessionStorage = sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(SearchClientsAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                IQueryable<Client> query = dbContext.Clients.Include(c => c.Translations).Include(c => c.Realms).Include(c => c.Scopes).Where(c => c.Realms.Any(r => r.Name == realm)).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(action.Filter))
                    query = query.Where(SanitizeExpression(action.Filter));

                if (!string.IsNullOrWhiteSpace(action.OrderBy))
                    query = query.OrderBy(SanitizeExpression(action.OrderBy));

                var nb = query.Count();
                var clients = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
                dispatcher.Dispatch(new SearchClientsSuccessAction { Clients = clients, Count = nb });
            }

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(GetAllClientsAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                IQueryable<Client> query = dbContext.Clients.Include(c => c.Translations).Include(c => c.Realms).Include(c => c.Scopes).Where(c => c.Realms.Any(r => r.Name == realm)).AsNoTracking();
                var nb = query.Count();
                var clients = await query.ToListAsync(CancellationToken.None);
                dispatcher.Dispatch(new SearchClientsSuccessAction { Clients = clients, Count = nb });
            }
        }

        [EffectMethod]
        public async Task Handle(AddSpaClientAction action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientId, action.RedirectionUrls, dispatcher)) return;
            using (var dbContext = _factory.CreateDbContext())
            {
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var scopes = await dbContext.Scopes.Include(s => s.Realms).Where(s => (s.Name == Constants.StandardScopes.OpenIdScope.Name || s.Name == Constants.StandardScopes.Profile.Name) && s.Realms.Any(r => r.Name == realm)).ToListAsync(CancellationToken.None);
                var newClientBuilder = ClientBuilder.BuildUserAgentClient(action.ClientId, Guid.NewGuid().ToString(), activeRealm, action.RedirectionUrls.ToArray())
                    .AddScope(scopes.ToArray());
                if (!string.IsNullOrWhiteSpace(action.ClientName))
                    newClientBuilder.SetClientName(action.ClientName);
                var newClient = newClientBuilder.Build();
                dbContext.Clients.Add(newClient);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientId, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = ClientTypes.SPA });
            }
        }

        [EffectMethod]
        public async Task Handle(AddMachineClientApplicationAction action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientId, new List<string>(), dispatcher)) return;
            using (var dbContext = _factory.CreateDbContext())
            {
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var newClientBuilder = ClientBuilder.BuildApiClient(action.ClientId, action.ClientSecret, activeRealm);
                if (!string.IsNullOrWhiteSpace(action.ClientName))
                    newClientBuilder.SetClientName(action.ClientName);
                var newClient = newClientBuilder.Build();
                dbContext.Clients.Add(newClient);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientId, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = ClientTypes.MACHINE });
            }
        }

        [EffectMethod]
        public async Task Handle(AddWebsiteApplicationAction action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientId, new List<string>(), dispatcher)) return;
            using (var dbContext = _factory.CreateDbContext())
            {
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var scopes = await dbContext.Scopes.Include(s => s.Realms).Where(s => (s.Name == Constants.StandardScopes.OpenIdScope.Name || s.Name == Constants.StandardScopes.Profile.Name) && s.Realms.Any(r => r.Name == realm)).ToListAsync(CancellationToken.None);
                var newClientBuilder = ClientBuilder.BuildTraditionalWebsiteClient(action.ClientId, action.ClientSecret, activeRealm, action.RedirectionUrls.ToArray())
                    .AddScope(scopes.ToArray());
                if (!string.IsNullOrWhiteSpace(action.ClientName))
                    newClientBuilder.SetClientName(action.ClientName);
                var newClient = newClientBuilder.Build();
                dbContext.Clients.Add(newClient);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientId, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = ClientTypes.WEBSITE });
            }
        }

        [EffectMethod]
        public async Task Handle(AddHighlySecuredWebsiteApplicationAction action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientId, new List<string>(), dispatcher)) return;
            using (var dbContext = _factory.CreateDbContext())
            {
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var scopes = await dbContext.Scopes.Include(s => s.Realms).Where(s => (s.Name == Constants.StandardScopes.OpenIdScope.Name || s.Name == Constants.StandardScopes.Profile.Name) && s.Realms.Any(r => r.Name == realm)).ToListAsync(CancellationToken.None);
                var newClientBuilder = ClientBuilder.BuildTraditionalWebsiteClient(action.ClientId, action.ClientSecret, activeRealm, action.RedirectionUrls.ToArray())
                    .AddScope(scopes.ToArray());
                if (!string.IsNullOrWhiteSpace(action.ClientName))
                    newClientBuilder.SetClientName(action.ClientName);

                // FAPI2.0
                string jsonWebKeyStr = null;
                newClientBuilder.SetSigAuthorizationResponse(SecurityAlgorithms.EcdsaSha256);
                newClientBuilder.SetIdTokenSignedResponseAlg(SecurityAlgorithms.EcdsaSha256);
                newClientBuilder.SetRequestObjectSigning(SecurityAlgorithms.EcdsaSha256);
                var ecdsaSig = ClientKeyGenerator.GenerateECDsaSignatureKey("keyId", SecurityAlgorithms.EcdsaSha256);
                jsonWebKeyStr = ecdsaSig.SerializeJWKStr();
                newClientBuilder.AddSigningKey(ecdsaSig, SecurityAlgorithms.EcdsaSha256, SecurityKeyTypes.ECDSA);
                if (action.IsDPoP)
                {
                    newClientBuilder.UseClientPrivateKeyJwtAuthentication();
                    newClientBuilder.UseDPOPProof(false);
                }
                else
                {
                    newClientBuilder.UseClientTlsAuthentication(action.SubjectName);
                }

                var newClient = newClientBuilder.Build();
                newClient.ClientType = ClientTypes.HIGHLYSECUREDWEBSITE;
                dbContext.Clients.Add(newClient);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientId, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = ClientTypes.HIGHLYSECUREDWEBSITE, JsonWebKeyStr = jsonWebKeyStr });
            }
        }

        [EffectMethod]
        public async Task Handle(AddHighlySecuredWebsiteApplicationWithGrantMgtSupportAction action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientId, new List<string>(), dispatcher)) return;
            using (var dbContext = _factory.CreateDbContext())
            {
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var scopes = await dbContext.Scopes.Include(s => s.Realms).Where(s => (s.Name == Constants.StandardScopes.OpenIdScope.Name || s.Name == Constants.StandardScopes.Profile.Name) && s.Realms.Any(r => r.Name == realm)).ToListAsync(CancellationToken.None);
                var newClientBuilder = ClientBuilder.BuildTraditionalWebsiteClient(action.ClientId, action.ClientSecret, activeRealm, action.RedirectionUrls.ToArray())
                    .AddScope(scopes.ToArray());
                if (!string.IsNullOrWhiteSpace(action.ClientName))
                    newClientBuilder.SetClientName(action.ClientName);

                // FAPI2.0
                string jsonWebKeyStr = null;
                newClientBuilder.SetSigAuthorizationResponse(SecurityAlgorithms.EcdsaSha256);
                newClientBuilder.SetIdTokenSignedResponseAlg(SecurityAlgorithms.EcdsaSha256);
                newClientBuilder.SetRequestObjectSigning(SecurityAlgorithms.EcdsaSha256);
                var ecdsaSig = ClientKeyGenerator.GenerateECDsaSignatureKey("keyId", SecurityAlgorithms.EcdsaSha256);
                jsonWebKeyStr = ecdsaSig.SerializeJWKStr();
                newClientBuilder.AddSigningKey(ecdsaSig, SecurityAlgorithms.EcdsaSha256, SecurityKeyTypes.ECDSA);
                if (action.IsDPoP)
                {
                    newClientBuilder.UseClientPrivateKeyJwtAuthentication();
                    newClientBuilder.UseDPOPProof(false);
                }
                else
                {
                    newClientBuilder.UseClientTlsAuthentication(action.SubjectName);
                }

                // Grant management
                scopes = await dbContext.Scopes.Include(s => s.Realms).Where(s => (s.Name == Constants.StandardScopes.GrantManagementQuery.Name || s.Name == Constants.StandardScopes.GrantManagementRevoke.Name) && s.Realms.Any(r => r.Name == realm)).ToListAsync(CancellationToken.None);
                newClientBuilder.AddScope(scopes.ToArray());
                var authDataTypes = string.IsNullOrWhiteSpace(action.AuthDataTypes) || action.AuthDataTypes == null ? null : action.AuthDataTypes.Split(';');
                if (authDataTypes != null)
                    newClientBuilder.AddAuthDataTypes(authDataTypes);

                var newClient = newClientBuilder.Build();
                newClient.ClientType = ClientTypes.GRANTMANAGEMENT;
                dbContext.Clients.Add(newClient);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientId, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = ClientTypes.GRANTMANAGEMENT, JsonWebKeyStr = jsonWebKeyStr });
            }
        }

        [EffectMethod]
        public async Task Handle(AddMobileApplicationAction action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientId, new List<string>(), dispatcher)) return;
            using (var dbContext = _factory.CreateDbContext())
            {
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var scopes = await dbContext.Scopes.Include(s => s.Realms).Where(s => (s.Name == Constants.StandardScopes.OpenIdScope.Name || s.Name == Constants.StandardScopes.Profile.Name) && s.Realms.Any(r => r.Name == realm)).ToListAsync(CancellationToken.None);
                var newClientBuilder = ClientBuilder.BuildMobileApplication(action.ClientId, Guid.NewGuid().ToString(), activeRealm, action.RedirectionUrls.ToArray())
                    .AddScope(scopes.ToArray());
                if (!string.IsNullOrWhiteSpace(action.ClientName))
                    newClientBuilder.SetClientName(action.ClientName);
                var newClient = newClientBuilder.Build();
                dbContext.Clients.Add(newClient);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientId, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = ClientTypes.MOBILE });
            }
        }

        [EffectMethod]
        public async Task Handle(AddExternalDeviceApplicationAction action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientId, new List<string>(), dispatcher)) return;
            using (var dbContext = _factory.CreateDbContext())
            {
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var scopes = await dbContext.Scopes.Include(s => s.Realms).Where(s => (s.Name == Constants.StandardScopes.OpenIdScope.Name || s.Name == Constants.StandardScopes.Profile.Name) && s.Realms.Any(r => r.Name == realm)).ToListAsync(CancellationToken.None);
                var newClientBuilder = ClientBuilder.BuildExternalAuthDeviceClient(action.ClientId, action.SubjectName, activeRealm)
                    .AddScope(scopes.ToArray());
                if (!string.IsNullOrWhiteSpace(action.ClientName))
                    newClientBuilder.SetClientName(action.ClientName);
                var newClient = newClientBuilder.Build();
                dbContext.Clients.Add(newClient);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientId, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = ClientTypes.EXTERNAL });
            }
        }

        [EffectMethod]
        public async Task Handle(AddDeviceApplicationAction action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientId, new List<string>(), dispatcher)) return;
            using (var dbContext = _factory.CreateDbContext())
            {
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var scopes = await dbContext.Scopes.Include(s => s.Realms).Where(s => (s.Name == Constants.StandardScopes.OpenIdScope.Name || s.Name == Constants.StandardScopes.Profile.Name) && s.Realms.Any(r => r.Name == realm)).ToListAsync(CancellationToken.None);
                var newClientBuilder = ClientBuilder.BuildDeviceClient(action.ClientId, action.ClientSecret, activeRealm)
                    .AddScope(scopes.ToArray());
                if (!string.IsNullOrWhiteSpace(action.ClientName))
                    newClientBuilder.SetClientName(action.ClientName);
                var newClient = newClientBuilder.Build();
                dbContext.Clients.Add(newClient);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientId, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = ClientTypes.DEVICE });
            }
        }

        [EffectMethod]
        public async Task Handle(AddWsFederationApplicationAction action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientId, new List<string>(), dispatcher)) return;
            using (var dbContext = _factory.CreateDbContext())
            {
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var newClientBuilder = WsClientBuilder.BuildWsFederationClient(action.ClientId, activeRealm);
                if (!string.IsNullOrWhiteSpace(action.ClientName))
                    newClientBuilder.SetClientName(action.ClientName);
                var newClient = newClientBuilder.Build();
                var scopeNames = newClient.Scopes.Select(s => s.Name);
                var scopes = await dbContext.Scopes.Where(s => scopeNames.Contains(s.Name)).ToListAsync(CancellationToken.None);
                newClient.Scopes = scopes;
                dbContext.Clients.Add(newClient);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientId, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = WsFederationConstants.CLIENT_TYPE });
            }
        }

        [EffectMethod]
        public async Task Handle(AddSamlSpApplicationAction action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientIdentifier, new List<string>(), dispatcher)) return;
            using (var dbContext = _factory.CreateDbContext())
            {
                var certificate = KeyGenerator.GenerateSelfSignedCertificate();
                var securityKey = new X509SecurityKey(certificate, Guid.NewGuid().ToString());
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var newClientBuilder = SamlSpClientBuilder.BuildSamlSpClient(action.ClientIdentifier, action.MetadataUrl, certificate, activeRealm);
                if (!string.IsNullOrWhiteSpace(action.ClientName))
                    newClientBuilder.SetClientName(action.ClientName);
                newClientBuilder.SetUseAcsArtifact(action.UseAcs);
                var newClient = newClientBuilder.Build();
                var scopeNames = newClient.Scopes.Select(s => s.Name);
                var scopes = await dbContext.Scopes.Where(s => scopeNames.Contains(s.Name)).ToListAsync(CancellationToken.None);
                newClient.Scopes = scopes;
                dbContext.Clients.Add(newClient);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                var pemResult = PemConverter.ConvertFromSecurityKey(securityKey);
                dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientIdentifier, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = SimpleIdServer.IdServer.Saml.Idp.Constants.CLIENT_TYPE, Pem = pemResult });
            }
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedClientsAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var clients = await dbContext.Clients.Include(c => c.Realms).Where(c => action.ClientIds.Contains(c.ClientId) && c.Realms.Any(r => r.Name == realm)).ToListAsync(CancellationToken.None);
                dbContext.Clients.RemoveRange(clients);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new RemoveSelectedClientsSuccessAction { ClientIds = action.ClientIds });
            }
        }

        [EffectMethod]
        public async Task Handle(GetClientAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).Include(c => c.Translations).Include(c => c.Scopes).Include(c => c.SerializedJsonWebKeys).AsNoTracking().SingleOrDefaultAsync(c => c.ClientId == action.ClientId && c.Realms.Any(r => r.Name == realm), CancellationToken.None);
                if (client == null)
                {
                    dispatcher.Dispatch(new GetClientFailureAction { ErrorMessage = string.Format(Global.UnknownClient, action.ClientId) });
                    return;
                }

                dispatcher.Dispatch(new GetClientSuccessAction { Client = client });
            }
        }

        [EffectMethod]
        public async Task Handle(UpdateClientDetailsAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).Include(c => c.Translations).SingleAsync(c => c.ClientId == act.ClientId && c.Realms.Any(r => r.Name == realm), CancellationToken.None);
                client.RedirectionUrls = act.RedirectionUrls.Split(';');
                client.UpdateClientName(act.ClientName);
                client.PostLogoutRedirectUris = act.PostLogoutRedirectUris.Split(';');
                client.FrontChannelLogoutSessionRequired = act.FrontChannelLogoutSessionRequired;
                client.FrontChannelLogoutUri = act.FrontChannelLogoutUri;
                client.BackChannelLogoutUri = act.BackChannelLogoutUri;
                client.BackChannelLogoutSessionRequired = act.BackChannelLogoutSessionRequired;
                client.TokenExchangeType = act.TokenExchangeType;
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
                {
                    grantTypes.Add(TokenExchangeHandler.GRANT_TYPE);
                    client.IsTokenExchangeEnabled = true;
                }
                else
                {
                    client.IsTokenExchangeEnabled = false;
                }

                client.GrantTypes = grantTypes;
                client.IsConsentDisabled = !act.IsConsentEnabled;
                client.SetUseAcsArtifact(act.UseAcs);
                client.SetSaml2SpMetadataUrl(act.MetadataUrl);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new UpdateClientDetailsSuccessAction
                {
                    ClientId = act.ClientId,
                    ClientName = act.ClientName,
                    RedirectionUrls = act.RedirectionUrls,
                    PostLogoutRedirectUris = act.PostLogoutRedirectUris,
                    FrontChannelLogoutSessionRequired = act.FrontChannelLogoutSessionRequired,
                    FrontChannelLogoutUri = act.FrontChannelLogoutUri,
                    BackChannelLogoutUri = act.BackChannelLogoutUri,
                    BackChannelLogoutSessionRequired = act.BackChannelLogoutSessionRequired,
                    IsClientCredentialsGrantTypeEnabled = act.IsClientCredentialsGrantTypeEnabled,
                    IsPasswordGrantTypeEnabled = act.IsPasswordGrantTypeEnabled,
                    IsRefreshTokenGrantTypeEnabled = act.IsRefreshTokenGrantTypeEnabled,
                    IsAuthorizationCodeGrantTypeEnabled = act.IsAuthorizationCodeGrantTypeEnabled,
                    IsCIBAGrantTypeEnabled = act.IsCIBAGrantTypeEnabled,
                    IsUMAGrantTypeEnabled = act.IsUMAGrantTypeEnabled,
                    IsConsentEnabled = act.IsConsentEnabled,
                    IsDeviceGrantTypeEnabled = act.IsDeviceGrantTypeEnabled,
                    TokenExchangeType = act.TokenExchangeType,
                    IsTokenExchangeEnabled = act.IsTokenExchangeEnabled
                });
            }
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedClientScopesAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).Include(c => c.Scopes).SingleAsync(c => c.ClientId == act.ClientId && c.Realms.Any(r => r.Name == realm), CancellationToken.None);
                client.Scopes = client.Scopes.Where(s => !act.ScopeNames.Contains(s.Name)).ToList();
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new RemoveSelectedClientScopesSuccessAction
                {
                    ClientId = act.ClientId,
                    ScopeNames = act.ScopeNames
                });
            }
        }

        [EffectMethod]
        public async Task Handle(AddClientScopesAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).Include(c => c.Scopes).SingleAsync(c => c.ClientId == act.ClientId && c.Realms.Any(r => r.Name == realm), CancellationToken.None);
                var newScopes = await dbContext.Scopes.Where(s => act.ScopeNames.Contains(s.Name)).ToListAsync();
                foreach (var newScope in newScopes)
                    client.Scopes.Add(newScope);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientScopesSuccessAction
                {
                    ClientId = act.ClientId,
                    Scopes = newScopes
                });
            }
        }

        [EffectMethod]
        public async Task Handle(GenerateSigKeyAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).AsNoTracking().Include(c => c.SerializedJsonWebKeys).SingleAsync(c => c.ClientId == act.ClientId && c.Realms.Any(r => r.Name == realm));
                if (client.JsonWebKeys.Any(j => j.KeyId == act.KeyId))
                {
                    dispatcher.Dispatch(new GenerateKeyFailureAction { ErrorMessage = string.Format(Global.SigKeyAlreadyExists, act.KeyId) });
                    return;
                }

                SigningCredentials sigCredentials = null;
                switch (act.KeyType)
                {
                    case SecurityKeyTypes.RSA:
                        sigCredentials = ClientKeyGenerator.GenerateRSASignatureKey(act.KeyId, act.Alg);
                        break;
                    case SecurityKeyTypes.CERTIFICATE:
                        sigCredentials = ClientKeyGenerator.GenerateX509CertificateSignatureKey(act.KeyId, act.Alg);
                        break;
                    case SecurityKeyTypes.ECDSA:
                        sigCredentials = ClientKeyGenerator.GenerateECDsaSignatureKey(act.KeyId, act.Alg);
                        break;
                }

                var pemResult = PemConverter.ConvertFromSecurityKey(sigCredentials.Key);
                dispatcher.Dispatch(new GenerateSigKeySuccessAction { Alg = act.Alg, KeyId = act.KeyId, Credentials = sigCredentials, Pem = pemResult, KeyType = act.KeyType, JsonWebKey = sigCredentials.SerializeJWKStr() });
            }
        }

        [EffectMethod]
        public async Task Handle(GenerateEncKeyAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).AsNoTracking().Include(c => c.SerializedJsonWebKeys).SingleAsync(c => c.ClientId == act.ClientId && c.Realms.Any(r => r.Name == realm));
                if (client.JsonWebKeys.Any(j => j.KeyId == act.KeyId))
                {
                    dispatcher.Dispatch(new GenerateKeyFailureAction { ErrorMessage = string.Format(Global.SigKeyAlreadyExists, act.KeyId) });
                    return;
                }

                EncryptingCredentials encCredentials = null;
                switch (act.KeyType)
                {
                    case SecurityKeyTypes.RSA:
                        encCredentials = ClientKeyGenerator.GenerateRSAEncryptionKey(act.KeyId, act.Alg, act.Enc);
                        break;
                    case SecurityKeyTypes.CERTIFICATE:
                        encCredentials = ClientKeyGenerator.GenerateCertificateEncryptionKey(act.KeyId, act.Alg, act.Enc);
                        break;
                }

                var pemResult = PemConverter.ConvertFromSecurityKey(encCredentials.Key);
                dispatcher.Dispatch(new GenerateEncKeySuccessAction { Alg = act.Alg, KeyId = act.KeyId, Credentials = encCredentials, Pem = pemResult, KeyType = act.KeyType, Enc = act.Enc, JsonWebKey = encCredentials.SerializeJWKStr() });
            }
        }

        [EffectMethod]
        public async Task Handle(AddSigKeyAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).Include(c => c.SerializedJsonWebKeys).SingleAsync(c => c.ClientId == act.ClientId && c.Realms.Any(r => r.Name == realm));
                var jsonWebKey = act.Credentials.SerializePublicJWK();
                client.Add(act.KeyId, jsonWebKey, Constants.JWKUsages.Sig, act.KeyType);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddSigKeySuccessAction { Alg = act.Alg, ClientId = act.ClientId, Credentials = act.Credentials, KeyId = act.KeyId, KeyType = act.KeyType });
            }
        }

        [EffectMethod]
        public async Task Handle(AddEncKeyAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).Include(c => c.SerializedJsonWebKeys).SingleAsync(c => c.ClientId == act.ClientId && c.Realms.Any(r => r.Name == realm));
                var jsonWebKey = act.Credentials.SerializePublicJWK();
                client.Add(act.KeyId, jsonWebKey, Constants.JWKUsages.Enc, act.KeyType);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddEncKeySuccessAction { Alg = act.Alg, ClientId = act.ClientId, Credentials = act.Credentials, KeyId = act.KeyId, KeyType = act.KeyType, Enc = act.Enc });
            }
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedClientKeysAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).Include(c => c.SerializedJsonWebKeys).SingleAsync(c => c.ClientId == act.ClientId && c.Realms.Any(r => r.Name == realm));
                client.SerializedJsonWebKeys = client.SerializedJsonWebKeys.Where(j => !act.KeyIds.Contains(j.Kid)).ToList();
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new RemoveSelectedClientKeysSuccessAction { ClientId = act.ClientId, KeyIds = act.KeyIds });
            }
        }

        [EffectMethod]
        public async Task Handle(UpdateJWKSUrlAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).SingleAsync(c => c.ClientId == act.ClientId && c.Realms.Any(r => r.Name == realm));
                client.JwksUri = act.JWKSUrl;
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new UpdateJWKSUrlSuccessAction { ClientId = act.ClientId, JWKSUrl = act.JWKSUrl });
            }
        }

        [EffectMethod]
        public async Task Handle(UpdateClientCredentialsAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).SingleAsync(c => c.ClientId == act.ClientId && c.Realms.Any(r => r.Name == realm));
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

                client.UpdateDateTime = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new UpdateClientCredentialsSuccessAction
                {
                    AuthMethod = act.AuthMethod,
                    ClientId = act.ClientId,
                    ClientSecret = act.ClientSecret,
                    TlsClientAuthSubjectDN = act.TlsClientAuthSubjectDN,
                    TlsClientAuthSanDNS = act.TlsClientAuthSanDNS,
                    TlsClientAuthSanEmail = act.TlsClientAuthSanEmail,
                    TlsClientAuthSanIP = act.TlsClientAuthSanIP
                });
            }
        }

        [EffectMethod]
        public async Task Handle(AddClientRoleAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var r = await dbContext.Realms.SingleAsync(r => r.Name == realm);
                var client = await dbContext.Clients.SingleAsync(c => c.ClientId == act.ClientId);
                var scope = ScopeBuilder.CreateRoleScope(client, act.Name, act.Description, r).Build();
                dbContext.Scopes.Add(scope);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientRoleSuccessAction
                {
                    Scope = scope
                });
            }
        }

        [EffectMethod]
        public async Task Handle(UpdateAdvancedClientSettingsAction act, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var client = await dbContext.Clients.Include(c => c.Realms).SingleAsync(c => c.ClientId == act.ClientId && c.Realms.Any(r => r.Name == realm));
                client.IdTokenSignedResponseAlg = act.IdTokenSignedResponseAlg;
                client.AuthorizationSignedResponseAlg = act.AuthorizationSignedResponseAlg;
                client.AuthorizationDataTypes = string.IsNullOrWhiteSpace(act.AuthorizationDataTypes) ? new List<string>() : act.AuthorizationDataTypes.Split(';');
                client.ResponseTypes = act.ResponseTypes?.ToList();
                client.DPOPBoundAccessTokens = act.IsDPoPRequired;
                client.DPOPNonceLifetimeInSeconds = act.DPOPNonceLifetimeInSeconds;
                client.IsDPOPNonceRequired = act.IsDPoPNonceRequired;
                await dbContext.SaveChangesAsync();
                dispatcher.Dispatch(new UpdateAdvancedClientSettingsSuccessAction
                {
                    AuthorizationDataTypes = client.AuthorizationDataTypes,
                    ResponseTypes = act.ResponseTypes,
                    AuthorizationSignedResponseAlg = act.AuthorizationSignedResponseAlg,
                    IdTokenSignedResponseAlg = act.IdTokenSignedResponseAlg,
                    DPOPNonceLifetimeInSeconds = act.DPOPNonceLifetimeInSeconds,
                    IsDPoPNonceRequired = act.IsDPoPNonceRequired,
                    IsDPoPRequired = act.IsDPoPRequired
                });
            }
        }

        private async Task<bool> ValidateAddClient(string clientId, IEnumerable<string> redirectionUrls, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                var errors = new List<string>();
                foreach (var redirectionUrl in redirectionUrls)
                    if (!ValidateRedirectionUrl(redirectionUrl, out string errorMessage))
                        errors.Add(errorMessage);

                if (errors.Any())
                {
                    dispatcher.Dispatch(new AddClientFailureAction { ClientId = clientId, ErrorMessage = string.Join(",", errors) });
                    return false;
                }

                var existingClient = await dbContext.Clients.Include(c => c.Realms).AsNoTracking().AnyAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == realm), CancellationToken.None);
                if (existingClient)
                {
                    dispatcher.Dispatch(new AddClientFailureAction { ClientId = clientId, ErrorMessage = Global.ClientAlreadyExists });
                    return false;
                }

                return true;
            }
        }

        private bool ValidateRedirectionUrl(string redirectionUrl, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrWhiteSpace(redirectionUrl) || !Uri.IsWellFormedUriString(redirectionUrl, UriKind.Absolute))
            {
                errorMessage = string.Format(Global.InvalidRedirectUrl, redirectionUrl);
                return false;
            }

            Uri.TryCreate(redirectionUrl, UriKind.Absolute, out Uri uri);
            if (!string.IsNullOrWhiteSpace(uri.Fragment))
                errorMessage = string.Format(Global.RedirectUriContainsFragment, redirectionUrl);

            return errorMessage == null;
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class SearchClientsAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class GetAllClientsAction
    {
    }

    public class SearchClientsSuccessAction
    {
        public IEnumerable<Client> Clients { get; set; } = new List<Client>();
        public int Count { get; set; }
    }

    public class AddSpaClientAction
    {
        public IEnumerable<string> RedirectionUrls { get; set; } = new List<string>();
        public string ClientId { get; set; } = null!;
        public string? ClientName { get; set; } = null;
    }

    public class AddWebsiteApplicationAction
    {
        public IEnumerable<string> RedirectionUrls { get; set; } = new List<string>();
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string? ClientName { get; set; } = null;
    }

    public class AddSamlSpApplicationAction
    {
        public string ClientName { get; set; } = null!;
        public string ClientIdentifier { get; set; } = null!;
        public string MetadataUrl { get; set; } = null!;
        public bool UseAcs { get; set; } = false;
    }

    public class AddHighlySecuredWebsiteApplicationAction
    {
        public IEnumerable<string> RedirectionUrls { get; set; } = new List<string>();
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string? ClientName { get; set; } = null;
        public string? SubjectName { get; set; } = null;
        public bool IsDPoP { get; set; } = false;
    }

    public class AddHighlySecuredWebsiteApplicationWithGrantMgtSupportAction
    {
        public IEnumerable<string> RedirectionUrls { get; set; } = new List<string>();
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string? ClientName { get; set; } = null;
        public string? SubjectName { get; set; } = null;
        public bool IsDPoP { get; set; } = false;
        public string? AuthDataTypes { get; set; } = null;
    }

    public class AddMobileApplicationAction
    {
        public IEnumerable<string> RedirectionUrls { get; set; } = new List<string>();
        public string ClientId { get; set; } = null!;
        public string? ClientName { get; set; } = null;
    }

    public class AddExternalDeviceApplicationAction
    {
        public string ClientId { get; set; } = null!;
        public string? ClientName { get; set; } = null;
        public string SubjectName { get; set; } = null!;
    }

    public class AddDeviceApplicationAction
    {
        public string ClientId { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }

    public class AddWsFederationApplicationAction
    {
        public string ClientId { get; set; } = null!;
        public string? ClientName { get; set; } = null;
    }

    public class AddMachineClientApplicationAction
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string? ClientName { get; set; } = null;
    }

    public class AddClientFailureAction
    {
        public string ClientId { get; set; } = null!;
        public string ErrorMessage { get; set; } = null;
    }

    public class AddClientSuccessAction
    {
        public string ClientId { get; set; } = null!;
        public string? ClientName { get; set; } = null;
        public string? Language { get; set; } = null;
        public string ClientType { get; set; }
        public string? JsonWebKeyStr { get; set; } = null;
        public PemResult? Pem { get; set; } = null;
    }

    public class RemoveSelectedClientsAction 
    {
        public IEnumerable<string> ClientIds { get; set; } = new List<string>();
    }

    public class RemoveSelectedClientsSuccessAction
    {
        public IEnumerable<string> ClientIds { get; set; } = new List<string>();
    }

    public class ToggleClientSelectionAction
    {
        public bool IsSelected { get; set; } = false;
        public string ClientId { get; set; } = null!;
    }

    public class ToggleAllClientSelectionAction
    {
        public bool IsSelected { get; set; } = false;
    }

    public class GetClientAction
    {
        public string ClientId { get; set; } = null!;
    }

    public class GetClientFailureAction
    {
        public string ErrorMessage { get; set; } = null!;
    }

    public class GetClientSuccessAction
    {
        public Client Client { get; set; } = null!;
    }

    public class UpdateClientDetailsAction
    {
        public string ClientId { get; set; } = null!;
        public string? ClientName { get; set; } = null;
        public string? RedirectionUrls { get; set; } = null;
        public string? PostLogoutRedirectUris { get; set; } = null;
        public bool FrontChannelLogoutSessionRequired { get; set; }
        public string? FrontChannelLogoutUri { get; set; } = null;
        public string? BackChannelLogoutUri { get; set; } = null;
        public bool BackChannelLogoutSessionRequired { get; set; }
        public bool IsClientCredentialsGrantTypeEnabled { get; set; }
        public bool IsPasswordGrantTypeEnabled { get; set; }
        public bool IsRefreshTokenGrantTypeEnabled { get; set; }
        public bool IsAuthorizationCodeGrantTypeEnabled { get; set; }
        public bool IsCIBAGrantTypeEnabled { get; set; }
        public bool IsUMAGrantTypeEnabled { get; set; }
        public bool IsConsentEnabled { get; set; }
        public bool IsDeviceGrantTypeEnabled { get; set; }
        public bool IsTokenExchangeEnabled { get; set; }
        public bool UseAcs { get; set; }
        public string MetadataUrl { get; set; }
        public TokenExchangeTypes? TokenExchangeType { get; set; }
    }

    public class UpdateClientDetailsSuccessAction
    {
        public string ClientId { get; set; } = null!;
        public string? ClientName { get; set; } = null;
        public string? RedirectionUrls { get; set; } = null;
        public string? PostLogoutRedirectUris { get; set; } = null;
        public bool FrontChannelLogoutSessionRequired { get; set; }
        public string? FrontChannelLogoutUri { get; set; } = null;
        public string? BackChannelLogoutUri { get; set; } = null;
        public bool BackChannelLogoutSessionRequired { get; set; }
        public bool IsClientCredentialsGrantTypeEnabled { get; set; }
        public bool IsPasswordGrantTypeEnabled { get; set; }
        public bool IsRefreshTokenGrantTypeEnabled { get; set; }
        public bool IsAuthorizationCodeGrantTypeEnabled { get; set; }
        public bool IsCIBAGrantTypeEnabled { get; set; }
        public bool IsUMAGrantTypeEnabled { get; set; }
        public bool IsConsentEnabled { get; set; }
        public bool IsDeviceGrantTypeEnabled { get; set; }
        public TokenExchangeTypes? TokenExchangeType { get; set; }
        public bool IsTokenExchangeEnabled { get; set; }
    }

    public class ToggleAllClientScopeSelectionAction
    {
        public bool IsSelected { get; set; } = false;
    }

    public class ToggleClientScopeSelectionAction
    {
        public string ClientId { get; set; } = null!;
        public string ScopeName { get; set; } = null!;
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedClientScopesAction
    {
        public string ClientId { get; set; } = null!;
        public IEnumerable<string> ScopeNames { get; set; } = new List<string>();
    }

    public class RemoveSelectedClientScopesSuccessAction
    {
        public string ClientId { get; set; } = null!;
        public IEnumerable<string> ScopeNames { get; set; } = new List<string>();
    }

    public class ToggleEditableClientScopeSelectionAction
    {
        public bool IsSelected { get; set; }
        public string ScopeName { get; set; } = null!;
    }

    public class ToggleAllEditableClientScopeSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class AddClientScopesAction
    {
        public string ClientId { get; set; } = null!;
        public IEnumerable<string> ScopeNames { get; set; } = new List<string>();
    }

    public class AddClientScopesSuccessAction
    {
        public string ClientId { get; set; } = null!;
        public IEnumerable<Scope> Scopes { get; set; } = new List<Scope>();
    }

    public class GenerateSigKeyAction
    {
        public string ClientId { get; set; } = null!;
        public string KeyId { get; set; } = null!;
        public SecurityKeyTypes KeyType { get; set; }
        public string Alg { get; set; } = null!;
    }

    public class GenerateSigKeySuccessAction
    {
        public string KeyId { get; set; } = null!;
        public string Alg { get; set; } = null!;
        public SigningCredentials Credentials { get; set; } = null!;
        public SecurityKeyTypes KeyType { get; set; }
        public PemResult Pem { get; set; } = null!;
        public string JsonWebKey { get; set; } = null!;
    }

    public class GenerateEncKeySuccessAction
    {
        public string KeyId { get; set; } = null!;
        public string Alg { get; set; } = null!;
        public string Enc { get; set; } = null!;
        public EncryptingCredentials Credentials { get; set; } = null!;
        public SecurityKeyTypes KeyType { get; set; }
        public PemResult Pem { get; set; } = null!;
        public string JsonWebKey { get; set; } = null!;
    }

    public class GenerateEncKeyAction
    {
        public string ClientId { get; set; } = null!;
        public string KeyId { get; set; } = null!;
        public SecurityKeyTypes KeyType { get; set; }
        public string Alg { get; set; } = null!;
        public string Enc { get; set; } = null!;
    }

    public class GenerateKeyFailureAction
    {
        public string ErrorMessage { get; set; } = null!;
    }

    public class AddSigKeyAction
    {
        public string ClientId { get; set; } = null!;
        public string KeyId { get; set; } = null!;
        public string Alg { get; set; } = null!;
        public SecurityKeyTypes KeyType { get; set; }
        public SigningCredentials Credentials { get; set; } = null!;
    }

    public class AddEncKeyAction
    {
        public string ClientId { get; set; } = null!;
        public string KeyId { get; set; } = null!;
        public string Alg { get; set; } = null!;
        public string Enc { get; set; } = null!;
        public SecurityKeyTypes KeyType { get; set; }
        public EncryptingCredentials Credentials { get; set; } = null!;
    }

    public class AddSigKeySuccessAction
    {
        public string ClientId { get; set; } = null!;
        public string KeyId { get; set; } = null!;
        public string Alg { get; set; } = null!;
        public SecurityKeyTypes KeyType { get; set; }
        public SigningCredentials Credentials { get; set; } = null!;
    }

    public class AddEncKeySuccessAction
    {
        public string ClientId { get; set; } = null!;
        public string KeyId { get; set; } = null!;
        public string Alg { get; set; } = null!;
        public string Enc { get; set; } = null!;
        public SecurityKeyTypes KeyType { get; set; }
        public EncryptingCredentials Credentials { get; set; } = null!;
    }

    public class ToggleAllClientKeySelectionAction
    {
        public bool IsSelected { get; set; } = false;
    }

    public class ToggleClientKeySelectionAction
    {
        public bool IsSelected { get; set; } = false;
        public string KeyId { get; set; } = null!;
    }

    public class RemoveSelectedClientKeysAction
    {
        public string ClientId { get; set; } = null!;
        public IEnumerable<string> KeyIds { get; set; } = new List<string>();
    }

    public class RemoveSelectedClientKeysSuccessAction
    {
        public string ClientId { get; set; } = null!;
        public IEnumerable<string> KeyIds { get; set; } = new List<string>();
    }

    public class UpdateJWKSUrlAction
    {
        public string ClientId { get; set; } = null!;
        public string JWKSUrl { get; set; } = null!;
    }

    public class UpdateJWKSUrlSuccessAction
    {
        public string ClientId { get; set; } = null!;
        public string JWKSUrl { get; set; } = null!;
    }

    public class UpdateClientCredentialsAction
    {
        public string ClientId { get; set; }
        public string AuthMethod { get; set; } = null!;
        public string? ClientSecret { get; set; } = null;
        public string? TlsClientAuthSubjectDN { get; set; } = null;
        public string? TlsClientAuthSanDNS { get; set; } = null;
        public string? TlsClientAuthSanEmail { get; set; } = null;
        public string? TlsClientAuthSanIP { get; set; } = null;
    }

    public class UpdateClientCredentialsSuccessAction
    {
        public string ClientId { get; set; }
        public string AuthMethod { get; set; } = null!;
        public string? ClientSecret { get; set; } = null;
        public string? TlsClientAuthSubjectDN { get; set; } = null;
        public string? TlsClientAuthSanDNS { get; set; } = null;
        public string? TlsClientAuthSanEmail { get; set; } = null;
        public string? TlsClientAuthSanIP { get; set; } = null;
    }

    public class AddClientRoleAction
    {
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class AddClientRoleSuccessAction
    {
        public Scope Scope { get; set; }
    }

    public class ToggleClientRoleAction
    {
        public string RoleId { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ToggleAllClientRolesAction
    {
        public bool IsSelected { get; set; }
    }

    public class UpdateAdvancedClientSettingsAction
    {
        public string ClientId { get; set; } = null!;
        public string? IdTokenSignedResponseAlg { get; set; }
        public string? AuthorizationSignedResponseAlg { get; set; }
        public string? AuthorizationDataTypes { get; set; }
        public IEnumerable<string> ResponseTypes { get; set; }
        public bool IsDPoPRequired { get; set; } = false;
        public bool IsDPoPNonceRequired { get; set; } = false;
        public double DPOPNonceLifetimeInSeconds { get; set; }
    }

    public class UpdateAdvancedClientSettingsSuccessAction
    {
        public string? IdTokenSignedResponseAlg { get; set; }
        public string? AuthorizationSignedResponseAlg { get; set; }
        public IEnumerable<string> AuthorizationDataTypes { get; set; }
        public IEnumerable<string> ResponseTypes { get; set; }
        public bool IsDPoPRequired { get; set; } = false;
        public bool IsDPoPNonceRequired { get; set; } = false;
        public double DPOPNonceLifetimeInSeconds { get; set; }
    }

    public class GenerateClientSigKeyAction
    {

    }
}
