// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Register;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ScopeNames = SimpleIdServer.IdServer.Domains.DTOs.ScopeNames;

namespace SimpleIdServer.IdServer.Api.Clients;

public class ClientsController : BaseController
{
    private readonly IClientRepository _clientRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly IRealmRepository _realmRepository;
    private readonly IRegisterClientRequestValidator _registerClientRequestValidator;
    private readonly IBusControl _busControl;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(
        IClientRepository clientRepository,
        IScopeRepository scopeRepository,
        IRealmRepository realmRepository,
        IRegisterClientRequestValidator registerClientRequestValidator,
        IBusControl busControl,
        ITransactionBuilder transactionBuilder,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder,
        ILogger<ClientsController> logger) : base(tokenRepository, jwtBuilder)
    {
        _clientRepository = clientRepository;
        _scopeRepository = scopeRepository;
        _realmRepository = realmRepository;
        _registerClientRequestValidator = registerClientRequestValidator;
        _busControl = busControl;
        _transactionBuilder = transactionBuilder;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
            var result = await _clientRepository.Search(prefix, request, cancellationToken);
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
            var result = await _clientRepository.GetAll(prefix, cancellationToken);
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] Client request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
                if (string.IsNullOrWhiteSpace(request.ClientId))
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, "id"));
                var existingClient = await _clientRepository.GetByClientId(prefix, request.ClientId, cancellationToken);
                if (existingClient != null)
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.ClientIdentifierAlreadyExists, request.ClientId));
                request.Scopes = await GetScopes(prefix, request.Scope, CancellationToken.None);
                var realm = await _realmRepository.Get(prefix, cancellationToken);
                request.Realms.Clear();
                request.Realms.Add(realm);
                await _registerClientRequestValidator.Validate(prefix, request, CancellationToken.None);
                _clientRepository.Add(request);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new ClientRegisteredSuccessEvent
                {
                    Realm = prefix,
                    RequestJSON = JsonSerializer.Serialize(request)
                });
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(request).ToString(),
                    ContentType = "application/json"
                };
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new ClientRegisteredFailureEvent
            {
                Realm = prefix,
                ErrorMessage = ex.Message,
                RequestJSON = JsonSerializer.Serialize(request)
            });
            return BuildError(ex);
        }

        async Task<ICollection<Domains.Scope>> GetScopes(string realm, string scope, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(scope)) return new List<Domains.Scope>();
            var scopeNames = scope.ToScopes();
            return await _scopeRepository.GetByNames(realm, scopeNames.ToList(), cancellationToken);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            id = System.Web.HttpUtility.UrlDecode(id);
            await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
            var result = await _clientRepository.GetByClientId(prefix, id, cancellationToken);
            if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByTechnicalId([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            id = System.Web.HttpUtility.UrlDecode(id);
            await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
            var result = await _clientRepository.GetById(prefix, id, cancellationToken);
            if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
            var result = await _clientRepository.GetById(prefix, id, cancellationToken);
            if (result == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
            _clientRepository.Delete(result);
            await transaction.Commit(cancellationToken);
            return new NoContentResult();
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] UpdateClientRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
                var result = await _clientRepository.GetById(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
                Update(result, request);
                result.UpdateDateTime = DateTime.UtcNow;
                await _registerClientRequestValidator.Validate(prefix, result, CancellationToken.None);
                _clientRepository.Update(result);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new ClientUpdatedSuccessEvent
                {
                    Realm = prefix,
                    SerializedRequest = JsonSerializer.Serialize(request),
                    Id = id
                });
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new ClientUpdatedFailureEvent
            {
                Realm = prefix,
                SerializedRequest = JsonSerializer.Serialize(request),
                Id = id
            });
            return BuildError(ex);
        }

        void Update(Client existingClient, UpdateClientRequest newClient)
        {
            existingClient.RedirectionUrls = newClient.RedirectionUrls;
            existingClient.UpdateClientName(newClient.ClientName);
            existingClient.AccessTokenType = newClient.AccessTokenType;
            existingClient.RedirectToRevokeSessionUI = newClient.RedirectToRevokeSessionUI;
            existingClient.PostLogoutRedirectUris = newClient.PostLogoutRedirectUris;
            existingClient.FrontChannelLogoutSessionRequired = newClient.FrontChannelLogoutSessionRequired;
            existingClient.FrontChannelLogoutUri = newClient.FrontChannelLogoutUri;
            existingClient.BackChannelLogoutUri = newClient.BackChannelLogoutUri;
            existingClient.BackChannelLogoutSessionRequired = newClient.BackChannelLogoutSessionRequired;
            existingClient.TokenExchangeType = newClient.TokenExchangeType;
            existingClient.GrantTypes = newClient.GrantTypes?.ToList();
            existingClient.IsTokenExchangeEnabled = newClient.IsTokenExchangeEnabled;
            existingClient.IsConsentDisabled = newClient.IsConsentDisabled;
            existingClient.JwksUri = newClient.JwksUrl;
            existingClient.IsRedirectUrlCaseSensitive = newClient.IsRedirectUrlCaseSensitive;
            existingClient.DefaultAcrValues = newClient.DefaultAcrValues;
            existingClient.IsPublic = newClient.IsPublic;
            if (newClient.Parameters != null)
            {
                var existingClientParameters = existingClient.Parameters;
                foreach (var kvp in newClient.Parameters)
                {
                    if (existingClientParameters.ContainsKey(kvp.Key)) existingClientParameters[kvp.Key] = kvp.Value;
                    else existingClientParameters.Add(kvp.Key, kvp.Value);
                }

                existingClient.Parameters = existingClientParameters;
            }
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAdvanced([FromRoute] string prefix, string id, [FromBody] UpdateAdvancedClientSettingsRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
                var result = await _clientRepository.GetById(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
                Update(result, request);
                await _registerClientRequestValidator.Validate(prefix, result, CancellationToken.None);
                result.UpdateDateTime = DateTime.UtcNow;
                _clientRepository.Update(result);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new ClientAdvancedSettingsUpdatedSuccessEvent
                {
                    Realm = prefix,
                    SerializedRequest = JsonSerializer.Serialize(request),
                    Id = id
                });
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new ClientAdvancedSettingsUpdatedFailureEvent
            {
                Realm = prefix,
                SerializedRequest = JsonSerializer.Serialize(request),
                Id = id
            });
            return BuildError(ex);
        }

        void Update(Client existingClient, UpdateAdvancedClientSettingsRequest request)
        {
            existingClient.TokenSignedResponseAlg = request.TokenSignedResponseAlg;
            existingClient.IdTokenSignedResponseAlg = request.IdTokenSignedResponseAlg;
            existingClient.AuthorizationSignedResponseAlg = request.AuthorizationSignedResponseAlg;
            existingClient.AuthorizationDataTypes = request.AuthorizationDataTypes;
            existingClient.ResponseTypes = request.ResponseTypes;
            existingClient.DPOPBoundAccessTokens = request.DPOPBoundAccessTokens;
            existingClient.DPOPNonceLifetimeInSeconds = request.DPOPNonceLifetimeInSeconds;
            existingClient.IsDPOPNonceRequired = request.IsDPOPNonceRequired;
            existingClient.TokenExpirationTimeInSeconds = request.TokenExpirationTimeInSeconds;
            existingClient.UserCookieExpirationTimeInSeconds = request.UserCookieExpirationTimeInSeconds;
            existingClient.AuthorizationCodeExpirationInSeconds = request.AuthorizationCodeExpirationInSeconds;
            existingClient.DeviceCodeExpirationInSeconds = request.DeviceCodeExpirationInSeconds;
            existingClient.DeviceCodePollingInterval = request.DeviceCodePollingInterval;
            existingClient.PARExpirationTimeInSeconds = request.PARExpirationTimeInSeconds;
        }
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveScope([FromRoute] string prefix, string id, string name, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
                var result = await _clientRepository.GetById(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
                var scope = result.Scopes.SingleOrDefault(s => s.Name == name);
                if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownScope, name));
                result.Scopes.Remove(scope);
                result.UpdateDateTime = DateTime.UtcNow;
                _clientRepository.Update(result);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new ClientScopeRemovedSuccessEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Scope = name
                });
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new ClientScopeRemovedFailureEvent
            {
                Realm = prefix,
                ClientId = id,
                Scope = name
            });
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddScope([FromRoute] string prefix, string id, [FromBody] AddClientScopeRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidRequestParameter);
                if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, nameof(ScopeNames.Name)));
                await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
                var result = await _clientRepository.GetById(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
                if (result.Scopes.Any(s => s.Name == request.Name)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.ScopeAlreadyExists, request.Name));
                var scope = await _scopeRepository.GetByName(prefix, request.Name, cancellationToken);
                if (scope == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownScope, request.Name));
                result.Scopes.Add(scope);
                result.UpdateDateTime = DateTime.UtcNow;
                _clientRepository.Update(result);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new AddClientScopeSuccessEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Scope = request.Name
                });
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new AddClientScopeFailureEvent
            {
                Realm = prefix,
                ClientId = id,
                Scope = request.Name
            });
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> GenerateSigKey([FromRoute] string prefix, string id, [FromBody] GenerateSigKeyRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
            var client = await _clientRepository.GetById(prefix, id, cancellationToken);
            if (client == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
            if (client.JsonWebKeys.Any(j => j.KeyId == request.KeyId)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.SigKeyAlreadyExists, request.KeyId));
            SigningCredentials sigCredentials = null;
            switch (request.KeyType)
            {
                case SecurityKeyTypes.RSA:
                    sigCredentials = ClientKeyGenerator.GenerateRSASignatureKey(request.KeyId, request.Alg);
                    break;
                case SecurityKeyTypes.CERTIFICATE:
                    sigCredentials = ClientKeyGenerator.GenerateX509CertificateSignatureKey(request.KeyId, request.Alg);
                    break;
                case SecurityKeyTypes.ECDSA:
                    sigCredentials = ClientKeyGenerator.GenerateECDsaSignatureKey(request.KeyId, request.Alg);
                    break;
            }

            var pemResult = PemConverter.ConvertFromSecurityKey(sigCredentials.Key);
            return new OkObjectResult(pemResult);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> GenerateEncKey([FromRoute] string prefix, string id, [FromBody] GenerateEncKeyRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
            var client = await _clientRepository.GetById(prefix, id, cancellationToken);
            if (client == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
            if (client.JsonWebKeys.Any(j => j.KeyId == request.KeyId)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.SigKeyAlreadyExists, request.KeyId));

            EncryptingCredentials encCredentials = null;
            switch (request.KeyType)
            {
                case SecurityKeyTypes.RSA:
                    encCredentials = ClientKeyGenerator.GenerateRSAEncryptionKey(request.KeyId, request.Alg, request.Enc);
                    break;
                case SecurityKeyTypes.CERTIFICATE:
                    encCredentials = ClientKeyGenerator.GenerateCertificateEncryptionKey(request.KeyId, request.Alg, request.Enc);
                    break;
            }

            var pemResult = PemConverter.ConvertFromSecurityKey(encCredentials.Key);
            return new OkObjectResult(pemResult);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddSigKey([FromRoute] string prefix, string id, [FromBody] AddSigKeyRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
                var result = await _clientRepository.GetById(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
                result.SerializedJsonWebKeys.Add(new ClientJsonWebKey
                {
                    Kid = request.KeyId,
                    SerializedJsonWebKey = request.SerializedJsonWebKey,
                    Alg = request.Alg,
                    KeyType = request.KeyType,
                    Usage = DefaultTokenSecurityAlgs.JwkUsages.Sig
                });
                result.UpdateDateTime = DateTime.UtcNow;
                _clientRepository.Update(result);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new AddClientSignatureKeySuccessEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Alg = request.Alg,
                    KeyType = request.KeyType,
                    KeyId = request.KeyId,
                    SerializedJsonWebKey = request.SerializedJsonWebKey
                });
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new AddClientSignatureKeyFailureEvent
            {
                Realm = prefix,
                ClientId = id,
                Alg = request.Alg,
                KeyType = request.KeyType,
                KeyId = request.KeyId,
                SerializedJsonWebKey = request.SerializedJsonWebKey
            });
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddEncKey([FromRoute] string prefix, string id, [FromBody] AddSigKeyRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
                var result = await _clientRepository.GetById(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
                result.SerializedJsonWebKeys.Add(new ClientJsonWebKey
                {
                    Kid = request.KeyId,
                    SerializedJsonWebKey = request.SerializedJsonWebKey,
                    Alg = request.Alg,
                    KeyType = request.KeyType,
                    Usage = DefaultTokenSecurityAlgs.JwkUsages.Enc
                });
                result.UpdateDateTime = DateTime.UtcNow;
                _clientRepository.Update(result);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new AddClientEncryptionKeySuccessEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Alg = request.Alg,
                    KeyType = request.KeyType,
                    KeyId = request.KeyId,
                    SerializedJsonWebKey = request.SerializedJsonWebKey
                });
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new AddClientEncryptionKeyFailureEvent
            {
                Realm = prefix,
                ClientId = id,
                Alg = request.Alg,
                KeyType = request.KeyType,
                KeyId = request.KeyId,
                SerializedJsonWebKey = request.SerializedJsonWebKey
            });
            return BuildError(ex);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveKey([FromRoute] string prefix, string id, string keyId, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
                var result = await _clientRepository.GetById(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
                var serializedJsonWebKey = result.SerializedJsonWebKeys.SingleOrDefault(k => k.Kid == keyId);
                if (serializedJsonWebKey == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownJwk, keyId));
                result.SerializedJsonWebKeys.Remove(serializedJsonWebKey);
                result.UpdateDateTime = DateTime.UtcNow;
                _clientRepository.Update(result);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new RemoveClientKeySuccessEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Kid = keyId
                });
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new RemoveClientKeyFailureEvent
            {
                Realm = prefix,
                ClientId = id,
                Kid = keyId
            });
            return BuildError(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCredentials([FromRoute] string prefix, string id, [FromBody] UpdateClientCredentialsRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
                var result = await _clientRepository.GetById(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
                result.TokenEndPointAuthMethod = request.TokenEndpointAuthMethod;
                result.ClientSecret = request.ClientSecret;
                result.TlsClientAuthSubjectDN = request.TlsClientAuthSubjectDN;
                result.TlsClientAuthSanDNS = request.TlsClientAuthSanDNS;
                result.TlsClientAuthSanEmail = request.TlsClientAuthSanEmail;
                result.TlsClientAuthSanIP = request.TlsClientAuthSanIp;
                result.UpdateDateTime = DateTime.UtcNow;
                _clientRepository.Update(result);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new ClientCredentialUpdatedSuccessEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    ClientSecret = request.ClientSecret,
                    TokenEndpointAuthMethod = request.TokenEndpointAuthMethod,
                    TlsClientAuthSanDNS = request.TlsClientAuthSanDNS,
                    TlsClientAuthSanEmail = request.TlsClientAuthSanEmail,
                    TlsClientAuthSanIp = request.TlsClientAuthSanIp,
                    TlsClientAuthSubjectDN = request.TlsClientAuthSubjectDN
                });
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new ClientCredentialUpdatedFailureEvent
            {
                Realm = prefix,
                ClientId = id,
                ClientSecret = request.ClientSecret,
                TokenEndpointAuthMethod = request.TokenEndpointAuthMethod,
                TlsClientAuthSanDNS = request.TlsClientAuthSanDNS,
                TlsClientAuthSanEmail = request.TlsClientAuthSanEmail,
                TlsClientAuthSanIp = request.TlsClientAuthSanIp,
                TlsClientAuthSubjectDN = request.TlsClientAuthSubjectDN
            });
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddRole([FromRoute] string prefix, string id, [FromBody] AddClientRoleRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
                var result = await _clientRepository.GetById(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
                var scopeName = $"{result.ClientId}/{request.Name}";
                var existingScope = await _scopeRepository.GetByName(prefix, scopeName, cancellationToken);
                if (existingScope != null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.ScopeAlreadyExists, scopeName));
                var newScope = new Domains.Scope
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = scopeName,
                    Type = ScopeTypes.ROLE,
                    Protocol = ScopeProtocols.OAUTH,
                    Description = request.Description,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                };
                var realm = await _realmRepository.Get(prefix, cancellationToken);
                newScope.Realms.Add(realm);
                result.Scopes.Add(newScope);
                result.UpdateDateTime = DateTime.UtcNow;
                _scopeRepository.Add(newScope);
                _clientRepository.Update(result);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new AddClientRoleSuccessEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Description = request.Description,
                    Name = request.Name
                });
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(newScope).ToString(),
                    ContentType = "application/json"
                };
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new AddClientRoleFailureEvent
            {
                Realm = prefix,
                ClientId = id,
                Description = request.Description,
                Name = request.Name
            });
            return BuildError(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRealms([FromRoute] string prefix, string id, [FromBody] UpdateClientRealmsRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.Clients.Name);
                var result = await _clientRepository.GetById(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClient, id));
                var realms = await _realmRepository.GetAll(cancellationToken);
                result.Realms = realms.Where(r => r.Name == prefix || request.Realms.Contains(r.Name)).ToList();
                result.UpdateDateTime = DateTime.UtcNow;
                _clientRepository.Update(result);
                await transaction.Commit(cancellationToken);
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }
}
