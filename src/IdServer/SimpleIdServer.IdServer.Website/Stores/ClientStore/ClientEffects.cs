// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.DPoP;
using SimpleIdServer.IdServer.Api.Clients;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Saml.Idp.Extensions;
using SimpleIdServer.IdServer.Website.Infrastructures;
using SimpleIdServer.IdServer.WsFederation;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore;

public class ClientEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly IdServerWebsiteOptions _configuration;
    private readonly IRealmStore _realmStore;

    public ClientEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory, 
        IOptions<IdServerWebsiteOptions> configuration,
        IRealmStore realmStore)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _configuration = configuration.Value;
        _realmStore = realmStore;
    }

    [EffectMethod]
    public async Task Handle(SearchClientsAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/.search"),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(new SearchRequest
            {
                Filter = SanitizeExpression(action.Filter),
                OrderBy = SanitizeExpression(action.OrderBy),
                Skip = action.Skip,
                Take = action.Take
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync(); 
        var searchResult = SidJsonSerializer.DeserializeSearchClients(json);
        dispatcher.Dispatch(new SearchClientsSuccessAction { Clients = searchResult.Content, Count = searchResult.Count });
        string SanitizeExpression(string expression) => expression.Replace("Value.", "");
    }

    [EffectMethod]
    public async Task Handle(GetAllClientsAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(baseUrl),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var clients = SidJsonSerializer.DeserializeClients(json);
        dispatcher.Dispatch(new SearchClientsSuccessAction { Clients = clients, Count = clients.Count() });
    }

    [EffectMethod]
    public async Task Handle(AddSpaClientAction action, IDispatcher dispatcher)
    {
        var newClientBuilder = ClientBuilder.BuildUserAgentClient(action.ClientId, Guid.NewGuid().ToString(), null, action.RedirectionUrls.ToArray())
                            .AddScope(new Domains.Scope { Name = Constants.StandardScopes.OpenIdScope.Name }, new Domains.Scope { Name = Constants.StandardScopes.Profile.Name });
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        var newClient = newClientBuilder.Build();
        await CreateClient(newClient, dispatcher, ClientTypes.SPA);
    }

    [EffectMethod]
    public async Task Handle(AddMachineClientApplicationAction action, IDispatcher dispatcher)
    {
        var newClientBuilder = ClientBuilder.BuildApiClient(action.ClientId, action.ClientSecret, null);
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        var newClient = newClientBuilder.Build();
        await CreateClient(newClient, dispatcher, ClientTypes.MACHINE);
    }

    [EffectMethod]
    public async Task Handle(AddWebsiteApplicationAction action, IDispatcher dispatcher)
    {
        var newClientBuilder = ClientBuilder.BuildTraditionalWebsiteClient(action.ClientId, action.ClientSecret, null, action.RedirectionUrls.ToArray())
                .AddScope(new Domains.Scope { Name = Constants.StandardScopes.OpenIdScope.Name }, new Domains.Scope { Name = Constants.StandardScopes.Profile.Name });
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        var newClient = newClientBuilder.Build();
        await CreateClient(newClient, dispatcher, ClientTypes.WEBSITE);
    }

    [EffectMethod]
    public async Task Handle(AddHighlySecuredWebsiteApplicationAction action, IDispatcher dispatcher)
    {
        var newClientBuilder = ClientBuilder.BuildTraditionalWebsiteClient(action.ClientId, action.ClientSecret, null, action.RedirectionUrls.ToArray())
                .AddScope(new Domains.Scope { Name = Constants.StandardScopes.OpenIdScope.Name }, new Domains.Scope { Name = Constants.StandardScopes.Profile.Name });
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

        // FAPI2.0
        newClientBuilder.SetSigAuthorizationResponse(SecurityAlgorithms.EcdsaSha256);
        newClientBuilder.SetIdTokenSignedResponseAlg(SecurityAlgorithms.EcdsaSha256);
        newClientBuilder.SetRequestObjectSigning(SecurityAlgorithms.EcdsaSha256);
        var ecdsaSig = ClientKeyGenerator.GenerateECDsaSignatureKey("keyId", SecurityAlgorithms.EcdsaSha256);
        var jsonWebKeyStr = ecdsaSig.SerializeJWKStr();
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
        await CreateClient(newClient, dispatcher, ClientTypes.HIGHLYSECUREDWEBSITE, jsonWebKey : jsonWebKeyStr);
    }

    [EffectMethod]
    public async Task Handle(AddHighlySecuredWebsiteApplicationWithGrantMgtSupportAction action, IDispatcher dispatcher)
    {
        var newClientBuilder = ClientBuilder.BuildTraditionalWebsiteClient(action.ClientId, action.ClientSecret, null, action.RedirectionUrls.ToArray())
                .AddScope(new Domains.Scope { Name = Constants.StandardScopes.OpenIdScope.Name }, new Domains.Scope { Name = Constants.StandardScopes.Profile.Name });
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

        // FAPI2.0
        newClientBuilder.SetSigAuthorizationResponse(SecurityAlgorithms.EcdsaSha256);
        newClientBuilder.SetIdTokenSignedResponseAlg(SecurityAlgorithms.EcdsaSha256);
        newClientBuilder.SetRequestObjectSigning(SecurityAlgorithms.EcdsaSha256);
        var ecdsaSig = ClientKeyGenerator.GenerateECDsaSignatureKey("keyId", SecurityAlgorithms.EcdsaSha256);
        var jsonWebKeyStr = ecdsaSig.SerializeJWKStr();
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
        newClientBuilder.AddScope(new Domains.Scope { Name = Constants.StandardScopes.GrantManagementQuery.Name }, new Domains.Scope { Name = Constants.StandardScopes.GrantManagementRevoke.Name });
        var authDataTypes = string.IsNullOrWhiteSpace(action.AuthDataTypes) || action.AuthDataTypes == null ? null : action.AuthDataTypes.Split(';');
        if (authDataTypes != null)
            newClientBuilder.AddAuthDataTypes(authDataTypes);

        var newClient = newClientBuilder.Build();
        await CreateClient(newClient, dispatcher, ClientTypes.GRANTMANAGEMENT, jsonWebKey: jsonWebKeyStr);
    }

    [EffectMethod]
    public async Task Handle(AddMobileApplicationAction action, IDispatcher dispatcher)
    {
        var newClientBuilder = ClientBuilder.BuildMobileApplication(action.ClientId, Guid.NewGuid().ToString(), null, action.RedirectionUrls.ToArray())
                .AddScope(new Domains.Scope { Name = Constants.StandardScopes.OpenIdScope.Name }, new Domains.Scope { Name = Constants.StandardScopes.Profile.Name });
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        var newClient = newClientBuilder.Build();
        await CreateClient(newClient, dispatcher, ClientTypes.MOBILE);
    }

    [EffectMethod]
    public async Task Handle(AddExternalDeviceApplicationAction action, IDispatcher dispatcher)
    {
        var newClientBuilder = ClientBuilder.BuildExternalAuthDeviceClient(action.ClientId, action.SubjectName, null)
                .AddScope(new Domains.Scope { Name = Constants.StandardScopes.OpenIdScope.Name }, new Domains.Scope { Name = Constants.StandardScopes.Profile.Name });
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        var newClient = newClientBuilder.Build();
        await CreateClient(newClient, dispatcher, ClientTypes.EXTERNAL);
    }

    [EffectMethod]
    public async Task Handle(AddDeviceApplicationAction action, IDispatcher dispatcher)
    {
        var newClientBuilder = ClientBuilder.BuildDeviceClient(action.ClientId, action.ClientSecret, null)
                .AddScope(new Domains.Scope { Name = Constants.StandardScopes.OpenIdScope.Name }, new Domains.Scope { Name = Constants.StandardScopes.Profile.Name });
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        var newClient = newClientBuilder.Build();
        await CreateClient(newClient, dispatcher, ClientTypes.DEVICE);
    }

    [EffectMethod]
    public async Task Handle(AddCredentialIssuerApplicationAction action, IDispatcher dispatcher)
    {
        var newClientBuilder = ClientBuilder.BuildCredentialIssuer(action.ClientId, action.ClientSecret, null, action.RedirectionUrls.ToArray())
                .AddScope(new Domains.Scope { Name = Constants.StandardScopes.OpenIdScope.Name }, new Domains.Scope { Name = Constants.StandardScopes.Profile.Name });
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        var newClient = newClientBuilder.Build();
        await CreateClient(newClient, dispatcher, ClientTypes.CREDENTIAL_ISSUER);
    }

    [EffectMethod]
    public async Task Handle(AddWalletAction action, IDispatcher dispatcher)
    {
        var newClientBuilder = ClientBuilder.BuildWalletClient(action.ClientId, action.ClientSecret, null);
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        var newClient = newClientBuilder.Build();
        await CreateClient(newClient, dispatcher, ClientTypes.WALLET);
    }

    [EffectMethod]
    public async Task Handle(AddWsFederationApplicationAction action, IDispatcher dispatcher)
    {
        var newClientBuilder = WsClientBuilder.BuildWsFederationClient(action.ClientId, null);
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        var newClient = newClientBuilder.Build();
        await CreateClient(newClient, dispatcher, WsFederationConstants.CLIENT_TYPE);
    }

    [EffectMethod]
    public async Task Handle(AddSamlSpApplicationAction action, IDispatcher dispatcher)
    {
        var certificate = KeyGenerator.GenerateSelfSignedCertificate();
        var securityKey = new X509SecurityKey(certificate, Guid.NewGuid().ToString());
        var newClientBuilder = SamlSpClientBuilder.BuildSamlSpClient(action.ClientIdentifier, action.MetadataUrl, certificate, null);
        if (!string.IsNullOrWhiteSpace(action.ClientName))
            newClientBuilder.SetClientName(action.ClientName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        newClientBuilder.SetUseAcsArtifact(action.UseAcs);
        var newClient = newClientBuilder.Build();
        var pemResult = PemConverter.ConvertFromSecurityKey(securityKey);
        await CreateClient(newClient, dispatcher, Saml.Idp.Constants.CLIENT_TYPE, pemResult);
    }

    [EffectMethod]
    public async Task Handle(RemoveSelectedClientsAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        foreach(var clientId in action.Ids)
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{clientId}"),
                Method = HttpMethod.Delete
            };
            await httpClient.SendAsync(requestMessage);
        }

        dispatcher.Dispatch(new RemoveSelectedClientsSuccessAction { Ids = action.Ids });
    }

    [EffectMethod]
    public async Task Handle(GetClientAction action, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/bytechnicalid/{action.Id}"),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            var client = JsonSerializer.Deserialize<Domains.Client>(json);
            dispatcher.Dispatch(new GetClientSuccessAction { Client = client });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new GetClientFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(UpdateClientDetailsAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var isTokenExchangeEnabled = false;
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
        if (act.IsTokenExchangePreAuthorizedCodeEnabled)
            grantTypes.Add(TokenExchangePreAuthorizedCodeHandler.GRANT_TYPE);
        if (act.IsTokenExchangeEnabled)
        {
            grantTypes.Add(TokenExchangeHandler.GRANT_TYPE);
            isTokenExchangeEnabled = true;
        }

        var request = new UpdateClientRequest
        {
            RedirectionUrls = act.RedirectionUrls.Split(';'),
            ClientName = act.ClientName,
            PostLogoutRedirectUris = act.PostLogoutRedirectUris.Split(';'),
            FrontChannelLogoutSessionRequired = act.FrontChannelLogoutSessionRequired,
            FrontChannelLogoutUri = act.FrontChannelLogoutUri,
            BackChannelLogoutUri = act.BackChannelLogoutUri,
            BackChannelLogoutSessionRequired = act.BackChannelLogoutSessionRequired,
            TokenExchangeType = act.TokenExchangeType,
            GrantTypes = grantTypes,
            IsTokenExchangeEnabled = isTokenExchangeEnabled,
            IsConsentDisabled = !act.IsConsentEnabled,
            JwksUrl = act.JwksUrl,
            IsRedirectUrlCaseSensitive = act.IsRedirectUrlCaseSensitive,
            AccessTokenType = act.AccessTokenType,
            RedirectToRevokeSessionUI = act.RedirectToRevokeSessionUI,
            DefaultAcrValues = act.DefaultAcrValues,
            IsPublic = act.IsPublic,
            Parameters = new Dictionary<string, string>
            {
                { ClientExtensions.SAML2_USE_ACS_ARTIFACT_NAME, act.UseAcs.ToString() },
                { ClientExtensions.SAML2_SP_METADATA_NAME, act.MetadataUrl }
            }
        };
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{act.Id}"),
            Method = HttpMethod.Put,
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
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
                IsTokenExchangeEnabled = act.IsTokenExchangeEnabled,
                IsRedirectUrlCaseSensitive = act.IsRedirectUrlCaseSensitive,
                UseAcs = act.UseAcs,
                MetadataUrl = act.MetadataUrl,
                RedirectToRevokeSessionUI = act.RedirectToRevokeSessionUI,
                AccessTokenType = act.AccessTokenType,
                DefaultAcrValues = act.DefaultAcrValues,
                IsPublic = act.IsPublic
            });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new UpdateClientDetailsFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(RemoveSelectedClientScopesAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        foreach (var scopeName in act.ScopeNames)
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{act.Id}/scopes/{scopeName}"),
                Method = HttpMethod.Delete
            };
            await httpClient.SendAsync(requestMessage);
        }

        dispatcher.Dispatch(new RemoveSelectedClientScopesSuccessAction { ClientId = act.ClientId, ScopeNames = act.ScopeNames });
    }

    [EffectMethod]
    public async Task Handle(AddClientScopesAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        foreach (var scope in act.Scopes)
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{act.Id}/scopes"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(new AddClientScopeRequest
                {
                    Name = scope.Name
                }), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
        }

        dispatcher.Dispatch(new AddClientScopesSuccessAction { ClientId = act.ClientId, Scopes = act.Scopes });
    }

    [EffectMethod]
    public async Task Handle(GenerateSigKeyAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{act.Id}/sigkey/generate"),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(new GenerateSigKeyRequest
            {
                Alg = act.Alg,
                KeyId = act.KeyId,
                KeyType = act.KeyType
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var pemResult = PemResult.Deserialize(json);
            SigningCredentials sigCredentials = null;
            switch (act.KeyType)
            {
                case SecurityKeyTypes.RSA:
                    sigCredentials = new SigningCredentials(PemImporter.Import<RsaSecurityKey>(pemResult, act.KeyId), act.Alg);
                    break;
                case SecurityKeyTypes.CERTIFICATE:
                    sigCredentials = new SigningCredentials(PemImporter.Import<X509SecurityKey>(pemResult, act.KeyId), act.Alg);
                    break;
                case SecurityKeyTypes.ECDSA:
                    sigCredentials = new SigningCredentials(PemImporter.Import<ECDsaSecurityKey>(pemResult, act.KeyId), act.Alg);
                    break;
            }

            dispatcher.Dispatch(new GenerateSigKeySuccessAction { Alg = act.Alg, KeyId = act.KeyId, Credentials = sigCredentials, Pem = pemResult, KeyType = act.KeyType, JsonWebKey = sigCredentials.SerializeJWKStr() });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new GenerateKeyFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(GenerateEncKeyAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{act.Id}/enckey/generate"),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(new GenerateEncKeyRequest
            {
                Alg = act.Alg,
                KeyId = act.KeyId,
                KeyType = act.KeyType,
                Enc = act.Enc
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var pemResult = PemResult.Deserialize(json);
            EncryptingCredentials encCredentials = null;
            switch (act.KeyType)
            {
                case SecurityKeyTypes.RSA:
                    encCredentials = new EncryptingCredentials(PemImporter.Import<RsaSecurityKey>(pemResult, act.KeyId), act.Alg, act.Enc);
                    break;
                case SecurityKeyTypes.CERTIFICATE:
                    encCredentials = new EncryptingCredentials(PemImporter.Import<X509SecurityKey>(pemResult, act.KeyId), act.Alg, act.Enc);
                    break;
            }

            dispatcher.Dispatch(new GenerateEncKeySuccessAction { Alg = act.Alg, KeyId = act.KeyId, Credentials = encCredentials, Pem = pemResult, KeyType = act.KeyType, Enc = act.Enc, JsonWebKey = encCredentials.SerializeJWKStr() });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new GenerateKeyFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(AddSigKeyAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var jsonWebKey = act.Credentials.SerializePublicJWK();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{act.Id}/sigkey"),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(new AddSigKeyRequest
            {
                Alg = act.Alg,
                KeyId = act.KeyId,
                KeyType = act.KeyType,
                SerializedJsonWebKey = JsonWebKeySerializer.Write(jsonWebKey),
            }), Encoding.UTF8, "application/json")
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new AddSigKeySuccessAction { Alg = act.Alg, ClientId = act.ClientId, Credentials = act.Credentials, KeyId = act.KeyId, KeyType = act.KeyType });
    }

    [EffectMethod]
    public async Task Handle(AddEncKeyAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var jsonWebKey = act.Credentials.SerializePublicJWK();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{act.Id}/enckey"),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(new AddSigKeyRequest
            {
                Alg = act.Alg,
                KeyId = act.KeyId,
                KeyType = act.KeyType,
                SerializedJsonWebKey = JsonWebKeySerializer.Write(jsonWebKey),
            }), Encoding.UTF8, "application/json")
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new AddEncKeySuccessAction { Alg = act.Alg, ClientId = act.ClientId, Credentials = act.Credentials, KeyId = act.KeyId, KeyType = act.KeyType, Enc = act.Enc });
    }

    [EffectMethod]
    public async Task Handle(RemoveSelectedClientKeysAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        foreach(var keyId in act.KeyIds)
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{act.Id}/keys/{keyId}"),
                Method = HttpMethod.Delete
            };
            await httpClient.SendAsync(requestMessage);
        }

        dispatcher.Dispatch(new RemoveSelectedClientKeysSuccessAction { ClientId = act.ClientId, KeyIds = act.KeyIds });
    }

    [EffectMethod]
    public async Task Handle(UpdateJWKSUrlAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var request = new UpdateClientRequest
        {
            RedirectionUrls = act.Client.RedirectionUrls,
            ClientName = act.Client.ClientName,
            PostLogoutRedirectUris = act.Client.PostLogoutRedirectUris,
            FrontChannelLogoutSessionRequired = act.Client.FrontChannelLogoutSessionRequired,
            FrontChannelLogoutUri = act.Client.FrontChannelLogoutUri,
            BackChannelLogoutUri = act.Client.BackChannelLogoutUri,
            BackChannelLogoutSessionRequired = act.Client.BackChannelLogoutSessionRequired,
            TokenExchangeType = act.Client.TokenExchangeType,
            GrantTypes = act.Client.GrantTypes,
            IsTokenExchangeEnabled = act.Client.IsTokenExchangeEnabled,
            IsConsentDisabled = act.Client.IsConsentDisabled,
            Parameters = new Dictionary<string, string>(),
            JwksUrl = act.JWKSUrl
        };
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{act.Id}"),
            Method = HttpMethod.Put,
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new UpdateJWKSUrlSuccessAction { ClientId = act.ClientId, JWKSUrl = act.JWKSUrl });
    }

    [EffectMethod]
    public async Task Handle(UpdateClientCredentialsAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var request = new UpdateClientCredentialsRequest
        {
            ClientSecret = act.ClientSecret,
            TlsClientAuthSanDNS = act.TlsClientAuthSanDNS,
            TlsClientAuthSanEmail = act.TlsClientAuthSanEmail,
            TlsClientAuthSanIp = act.TlsClientAuthSanIP,
            TlsClientAuthSubjectDN = act.TlsClientAuthSubjectDN,
            TokenEndpointAuthMethod = act.AuthMethod
        };
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{act.Id}/credentials"),
            Method = HttpMethod.Put,
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };
        await httpClient.SendAsync(requestMessage);
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

    [EffectMethod]
    public async Task Handle(AddClientRoleAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var request = new AddClientRoleRequest
        {
            Description = act.Description,
            Name = act.Name
        };
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{act.Id}/roles"),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var newScope = JsonSerializer.Deserialize<Domains.Scope>(json);
            dispatcher.Dispatch(new AddClientRoleSuccessAction
            {
                Scope = newScope
            });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new AddClientRoleFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(UpdateAdvancedClientSettingsAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var request = new UpdateAdvancedClientSettingsRequest
        {
            TokenSignedResponseAlg = act.TokenSignedResponseAlg,
            IdTokenSignedResponseAlg = act.IdTokenSignedResponseAlg,
            AuthorizationSignedResponseAlg = act.AuthorizationSignedResponseAlg,
            AuthorizationDataTypes = string.IsNullOrWhiteSpace(act.AuthorizationDataTypes) ? new List<string>() : act.AuthorizationDataTypes.Split(';'),
            ResponseTypes = act.ResponseTypes?.ToList(), 
            DPOPBoundAccessTokens = act.IsDPoPRequired,
            DPOPNonceLifetimeInSeconds = act.DPOPNonceLifetimeInSeconds,
            IsDPOPNonceRequired = act.IsDPoPNonceRequired,
            TokenExpirationTimeInSeconds = act.TokenExpirationTimeInSeconds,
            UserCookieExpirationTimeInSeconds = act.UserCookieExpirationTimeInSeconds
        };
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{act.Id}/advanced"),
            Method = HttpMethod.Put,
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            dispatcher.Dispatch(new UpdateAdvancedClientSettingsSuccessAction
            {
                TokenSignedResponseAlg = request.TokenSignedResponseAlg,
                AuthorizationDataTypes = request.AuthorizationDataTypes,
                ResponseTypes = act.ResponseTypes,
                AuthorizationSignedResponseAlg = act.AuthorizationSignedResponseAlg,
                IdTokenSignedResponseAlg = act.IdTokenSignedResponseAlg,
                DPOPNonceLifetimeInSeconds = act.DPOPNonceLifetimeInSeconds,
                IsDPoPNonceRequired = act.IsDPoPNonceRequired,
                IsDPoPRequired = act.IsDPoPRequired,
                TokenExpirationTimeInSeconds = act.TokenExpirationTimeInSeconds,
                UserCookieExpirationTimeInSeconds = act.UserCookieExpirationTimeInSeconds
            });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new UpdateAdvancedClientSettingsFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(UpdateClientRealmsAction act, IDispatcher dispatcher)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var request = new UpdateClientRealmsRequest
        {
            Realms = act.Realms
        };
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{baseUrl}/{act.Id}/realms"),
            Method = HttpMethod.Put,
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new UpdateClientRealmsSuccessAction());
    }

    private async Task CreateClient(Domains.Client client, IDispatcher dispatcher, string clientType, PemResult pemResult = null, string jsonWebKey = null)
    {
        var baseUrl = await GetClientsUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(baseUrl),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(client), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var newClient = JsonSerializer.Deserialize<Client>(json);
            dispatcher.Dispatch(new AddClientSuccessAction { Id = newClient.Id, ClientId = client.ClientId, ClientName = client.ClientName, Language = client.Translations.FirstOrDefault()?.Language, ClientType = clientType, Pem = pemResult, JsonWebKeyStr = jsonWebKey });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new AddClientFailureAction { ClientId = client.ClientId, ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    private Task<string> GetClientsUrl() => GetBaseUrl("clients");

    private async Task<string> GetBaseUrl(string subUrl)
    {
        if (_configuration.IsReamEnabled)
        {
            var realm = _realmStore.Realm;
            var realmStr = !string.IsNullOrWhiteSpace(realm) ? realm : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return $"{_configuration.IdServerBaseUrl}/{realmStr}/{subUrl}";
        }

        return $"{_configuration.IdServerBaseUrl}/{subUrl}";
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
    public IEnumerable<Domains.Client> Clients { get; set; } = new List<Domains.Client>();
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

public class AddCredentialIssuerApplicationAction
{
    public string ClientId { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public IEnumerable<string> RedirectionUrls { get; set; } = new List<string>();
}

public class AddWalletAction
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string ClientName { get; set; }
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
    public string Id { get; set; }
    public string ClientId { get; set; } = null!;
    public string? ClientName { get; set; } = null;
    public string? Language { get; set; } = null;
    public string ClientType { get; set; }
    public string? JsonWebKeyStr { get; set; } = null;
    public PemResult? Pem { get; set; } = null;
}

public class RemoveSelectedClientsAction 
{
    public IEnumerable<string> Ids { get; set; } = new List<string>();
}

public class RemoveSelectedClientsSuccessAction
{
    public IEnumerable<string> Ids { get; set; } = new List<string>();
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
    public string Id { get; set; } = null!;
}

public class GetClientFailureAction
{
    public string ErrorMessage { get; set; } = null!;
}

public class GetClientSuccessAction
{
    public Domains.Client Client { get; set; } = null!;
}

public class UpdateClientDetailsAction
{
    public string Id { get; set; }
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
    public string JwksUrl { get; set; }
    public TokenExchangeTypes? TokenExchangeType { get; set; }
    public bool IsRedirectUrlCaseSensitive { get; set; }
    public AccessTokenTypes AccessTokenType { get; set; }
    public bool RedirectToRevokeSessionUI { get; set; }
    public bool IsTokenExchangePreAuthorizedCodeEnabled { get; set; }
    public ICollection<string> DefaultAcrValues { get; set; }
    public bool IsPublic { get; set; }
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
    public bool IsRedirectUrlCaseSensitive { get; set; }
    public bool UseAcs { get; set; }
    public string MetadataUrl { get; set; }
    public AccessTokenTypes AccessTokenType { get; set; }
    public bool RedirectToRevokeSessionUI { get; set; }
    public ICollection<string> DefaultAcrValues { get; set; }
    public bool IsPublic { get; set; }
}

public class UpdateClientDetailsFailureAction
{
    public string ErrorMessage { get; set; }
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
    public string Id { get; set; }
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
    public string Id { get; set; }
    public string ClientId { get; set; } = null!;
    public IEnumerable<SimpleIdServer.IdServer.Domains.Scope> Scopes { get; set; } = new List<SimpleIdServer.IdServer.Domains.Scope>();
}

public class AddClientScopesSuccessAction
{
    public string ClientId { get; set; } = null!;
    public IEnumerable<SimpleIdServer.IdServer.Domains.Scope> Scopes { get; set; } = new List<SimpleIdServer.IdServer.Domains.Scope>();
}

public class GenerateSigKeyAction
{
    public string Id { get; set; }
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
    public string Id { get; set; }
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
    public string Id { get; set; }
    public string ClientId { get; set; } = null!;
    public string KeyId { get; set; } = null!;
    public string Alg { get; set; } = null!;
    public SecurityKeyTypes KeyType { get; set; }
    public SigningCredentials Credentials { get; set; } = null!;
}

public class AddEncKeyAction
{
    public string Id { get; set; }
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
    public string Id { get; set; }
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
    public string Id { get; set; }
    public string ClientId { get; set; } = null!;
    public string JWKSUrl { get; set; } = null!;
    public SimpleIdServer.IdServer.Domains.Client Client { get; set; } = null!;
}

public class UpdateJWKSUrlSuccessAction
{
    public string ClientId { get; set; } = null!;
    public string JWKSUrl { get; set; } = null!;
}

public class UpdateClientCredentialsAction
{
    public string Id { get; set; }
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
    public string Id { get; set; }
    public string ClientId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class AddClientRoleSuccessAction
{
    public SimpleIdServer.IdServer.Domains.Scope Scope { get; set; }
}

public class AddClientRoleFailureAction
{
    public string ErrorMessage { get; set; }
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
    public string Id { get; set; }
    public string ClientId { get; set; } = null!;
    public string? TokenSignedResponseAlg { get; set; }
    public string? IdTokenSignedResponseAlg { get; set; }
    public string? AuthorizationSignedResponseAlg { get; set; }
    public string? AuthorizationDataTypes { get; set; }
    public IEnumerable<string> ResponseTypes { get; set; }
    public bool IsDPoPRequired { get; set; } = false;
    public bool IsDPoPNonceRequired { get; set; } = false;
    public double DPOPNonceLifetimeInSeconds { get; set; }
    public double TokenExpirationTimeInSeconds { get; set; }
    public double UserCookieExpirationTimeInSeconds { get; set; }
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
    public double TokenExpirationTimeInSeconds { get; set; }
    public string? TokenSignedResponseAlg { get; set;  }
    public double UserCookieExpirationTimeInSeconds { get; set; }
}

public class UpdateAdvancedClientSettingsFailureAction
{
    public string ErrorMessage { get; set; }
}

public class GenerateClientSigKeyAction
{

}

public class StartAddClientAction
{

}

public class StartGenerateClientKeyAction { }

public class UpdateClientRealmsAction
{
    public string Id { get; set; }
    public string ClientId { get; set; }
    public List<string> Realms { get; set; }
}

public class UpdateClientRealmsSuccessAction
{

}