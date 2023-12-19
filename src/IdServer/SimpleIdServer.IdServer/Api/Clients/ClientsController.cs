// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Register;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
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
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(
        IClientRepository clientRepository, 
        IScopeRepository scopeRepository, 
        IRealmRepository realmRepository, 
        IRegisterClientRequestValidator registerClientRequestValidator, 
        IBusControl busControl, 
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder, 
        ILogger<ClientsController> logger) : base(tokenRepository, jwtBuilder)
    {
        _clientRepository = clientRepository;
        _scopeRepository = scopeRepository;
        _realmRepository = realmRepository;
        _registerClientRequestValidator = registerClientRequestValidator;
        _busControl = busControl;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
            IQueryable<Client> query = _clientRepository.Query()
                .Include(c => c.Translations)
                .Include(p => p.Realms)
                .Include(p => p.Scopes)
                .Where(p => p.Realms.Any(r => r.Name == prefix))
                .AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.Filter))
                query = query.Where(request.Filter);

            if (!string.IsNullOrWhiteSpace(request.OrderBy)) 
                query = query.OrderBy(request.OrderBy);
            else 
                query = query.OrderByDescending(r => r.UpdateDateTime);

            var nb = query.Count();
            var clients = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
            return new OkObjectResult(new SearchResult<Client>
            {
                Count = nb,
                Content = clients
            });
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromRoute] string prefix)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
            IQueryable<Client> query = _clientRepository.Query()
                .Include(c => c.Translations)
                .Include(p => p.Realms)
                .Include(p => p.Scopes)
                .Where(p => p.Realms.Any(r => r.Name == prefix))
                .AsNoTracking();
            var clients = await query.ToListAsync();
            return new OkObjectResult(clients);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] Client request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add client"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
                request.Scopes = await GetScopes(prefix, request.Scope, CancellationToken.None);
                var realm = await _realmRepository.Query().SingleAsync(r => r.Name == prefix);
                request.Realms.Add(realm);
                await _registerClientRequestValidator.Validate(prefix, request, CancellationToken.None);
                _clientRepository.Add(request);
                await _clientRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Client {request.Id} is added");
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
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new ClientRegisteredFailureEvent
                {
                    Realm = prefix,
                    ErrorMessage = ex.Message,
                    RequestJSON = JsonSerializer.Serialize(request)
                });
                return BuildError(ex);
            }
        }

        async Task<ICollection<Domains.Scope>> GetScopes(string realm, string scope, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(scope)) return new List<Domains.Scope>();
            var scopeNames = scope.ToScopes();
            return await _scopeRepository.Query()
                .Include(s => s.Realms)
                .Where(s => scopeNames.Contains(s.Name) && s.Realms.Any(r => r.Name == realm))
                .ToListAsync(cancellationToken);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
            var result = await _clientRepository.Query()
                .Include(c => c.Realms)
                .Include(c => c.Translations)
                .Include(c => c.Scopes)
                .Include(c => c.SerializedJsonWebKeys)
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
            if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
            return new OkObjectResult(result);
        }
        catch(OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string prefix, string id)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
        var result = await _clientRepository.Query()
            .Include(c => c.Realms)
            .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
        if (result == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
        _clientRepository.Delete(result);
        await _clientRepository.SaveChanges(CancellationToken.None);
        return new NoContentResult();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] UpdateClientRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Update client"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
                var result = await _clientRepository.Query()
                    .Include(c => c.Realms)
                    .Include(c => c.Translations)
                    .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
                Update(result, request);
                result.UpdateDateTime = DateTime.UtcNow;
                await _registerClientRequestValidator.Validate(prefix, result, CancellationToken.None);
                await _clientRepository.SaveChanges(CancellationToken.None);
                await _busControl.Publish(new ClientUpdatedSuccessEvent
                {
                    Realm = prefix,
                    Request = request,
                    Id = id
                });
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new ClientUpdatedFailureEvent
                {
                    Realm = prefix,
                    Request = request,
                    Id = id
                });
                return BuildError(ex);
            }
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
    public async Task<IActionResult> UpdateAdvanced([FromRoute] string prefix, string id, [FromBody] UpdateAdvancedClientSettingsRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Update advanced client settings"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
                var result = await _clientRepository.Query()
                    .Include(c => c.Realms)
                    .Include(c => c.Translations)
                    .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
                Update(result, request);
                await _registerClientRequestValidator.Validate(prefix, result, CancellationToken.None);
                result.UpdateDateTime = DateTime.UtcNow;
                await _clientRepository.SaveChanges(CancellationToken.None);
                await _busControl.Publish(new ClientAdvancedSettingsUpdatedSuccessEvent
                {
                    Realm = prefix,
                    Request = request,
                    Id = id
                });
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new ClientAdvancedSettingsUpdatedFailureEvent
                {
                    Realm = prefix,
                    Request = request,
                    Id = id
                });
                return BuildError(ex);
            }
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
        }
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveScope([FromRoute] string prefix, string id, string name)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove client scope"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
                var result = await _clientRepository.Query()
                    .Include(c => c.Realms)
                    .Include(c => c.Scopes)
                    .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
                var scope = result.Scopes.SingleOrDefault(s => s.Name == name);
                if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_SCOPE, name));
                result.Scopes.Remove(scope);
                result.UpdateDateTime = DateTime.UtcNow;
                await _clientRepository.SaveChanges(CancellationToken.None);
                await _busControl.Publish(new ClientScopeRemovedSuccessEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Scope = name
                });
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new ClientScopeRemovedFailureEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Scope = name
                });
                return BuildError(ex);
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddScope([FromRoute] string prefix, string id, [FromBody] AddClientScopeRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add client scope"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_REQUEST_PARAMETER);
                if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, nameof(ScopeNames.Name)));
                await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
                var result = await _clientRepository.Query()
                    .Include(c => c.Realms)
                    .Include(c => c.Scopes)
                    .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
                if (result.Scopes.Any(s => s.Name == request.Name)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.SCOPE_ALREADY_EXISTS, request.Name));
                var scope = await _scopeRepository.Query()
                    .Include(s => s.Realms)
                    .SingleOrDefaultAsync(c => c.Name == request.Name && c.Realms.Any(r => r.Name == prefix));
                if (scope == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_SCOPE, request.Name));
                result.Scopes.Add(scope);
                result.UpdateDateTime = DateTime.UtcNow;
                await _clientRepository.SaveChanges(CancellationToken.None);
                await _busControl.Publish(new AddClientScopeSuccessEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Scope = request.Name
                });
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new AddClientScopeFailureEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Scope = request.Name
                });
                return BuildError(ex);
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> GenerateSigKey([FromRoute] string prefix, string id, [FromBody] GenerateSigKeyRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
            var client = await _clientRepository.Query()
                .Include(c => c.Realms)
                .Include(c => c.SerializedJsonWebKeys)
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
            if (client == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
            if (client.JsonWebKeys.Any(j => j.KeyId == request.KeyId)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.SIGKEY_ALREADY_EXISTS, request.KeyId));
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
    public async Task<IActionResult> GenerateEncKey([FromRoute] string prefix, string id, [FromBody] GenerateEncKeyRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
            var client = await _clientRepository.Query()
                .Include(c => c.Realms)
                .Include(c => c.SerializedJsonWebKeys)
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
            if (client == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
            if (client.JsonWebKeys.Any(j => j.KeyId == request.KeyId)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.SIGKEY_ALREADY_EXISTS, request.KeyId));

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
    public async Task<IActionResult> AddSigKey([FromRoute] string prefix, string id, [FromBody] AddSigKeyRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add client signature key"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
                var result = await _clientRepository.Query()
                    .Include(c => c.Realms)
                    .Include(c => c.SerializedJsonWebKeys)
                    .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
                result.SerializedJsonWebKeys.Add(new ClientJsonWebKey
                {
                    Kid = request.KeyId,
                    SerializedJsonWebKey = request.SerializedJsonWebKey,
                    Alg = request.Alg,
                    KeyType = request.KeyType,
                    Usage = Constants.JWKUsages.Sig
                });
                result.UpdateDateTime = DateTime.UtcNow;
                await _clientRepository.SaveChanges(CancellationToken.None);
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
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
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
    }

    [HttpPost]
    public async Task<IActionResult> AddEncKey([FromRoute] string prefix, string id, [FromBody] AddSigKeyRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add client encryption key"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
                var result = await _clientRepository.Query()
                    .Include(c => c.Realms)
                    .Include(c => c.SerializedJsonWebKeys)
                    .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
                result.SerializedJsonWebKeys.Add(new ClientJsonWebKey
                {
                    Kid = request.KeyId,
                    SerializedJsonWebKey = request.SerializedJsonWebKey,
                    Alg = request.Alg,
                    KeyType = request.KeyType,
                    Usage = Constants.JWKUsages.Enc
                });
                result.UpdateDateTime = DateTime.UtcNow;
                await _clientRepository.SaveChanges(CancellationToken.None);
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
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
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
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveKey([FromRoute] string prefix, string id, string keyId)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove client key"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
                var result = await _clientRepository.Query()
                    .Include(c => c.Realms)
                    .Include(c => c.SerializedJsonWebKeys)
                    .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
                var serializedJsonWebKey = result.SerializedJsonWebKeys.SingleOrDefault(k => k.Kid == keyId);
                if (serializedJsonWebKey == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_JSON_WEB_KEY, keyId));
                result.SerializedJsonWebKeys.Remove(serializedJsonWebKey);
                result.UpdateDateTime = DateTime.UtcNow;
                await _clientRepository.SaveChanges(CancellationToken.None);
                await _busControl.Publish(new RemoveClientKeySuccessEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Kid = keyId
                });
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new RemoveClientKeyFailureEvent
                {
                    Realm = prefix,
                    ClientId = id,
                    Kid = keyId
                });
                return BuildError(ex);
            }
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCredentials([FromRoute] string prefix, string id, [FromBody] UpdateClientCredentialsRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Update client credentials"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
                var result = await _clientRepository.Query()
                    .Include(c => c.Realms)
                    .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
                result.TokenEndPointAuthMethod = request.TokenEndpointAuthMethod;
                result.ClientSecret = request.ClientSecret;
                result.TlsClientAuthSubjectDN = request.TlsClientAuthSubjectDN;
                result.TlsClientAuthSanDNS = request.TlsClientAuthSanDNS;
                result.TlsClientAuthSanEmail = request.TlsClientAuthSanEmail;
                result.TlsClientAuthSanIP = request.TlsClientAuthSanIp;
                result.UpdateDateTime = DateTime.UtcNow;
                await _clientRepository.SaveChanges(CancellationToken.None);
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
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
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
    }

    [HttpPost]
    public async Task<IActionResult> AddRole([FromRoute] string prefix, string id, [FromBody] AddClientRoleRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add client role"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name);
                var result = await _clientRepository.Query()
                    .Include(c => c.Scopes)
                    .SingleOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == prefix));
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT, id));
                var newScope = new Scope
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"{result.ClientId}/{request.Name}",
                    Type = ScopeTypes.ROLE,
                    Protocol = ScopeProtocols.OAUTH,
                    Description = request.Description,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                };
                var realm = await _realmRepository.Query().SingleAsync(r => r.Name == prefix);
                newScope.Realms.Add(realm);
                result.Scopes.Add(newScope);
                result.UpdateDateTime = DateTime.UtcNow;
                await _clientRepository.SaveChanges(CancellationToken.None);
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
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
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
    }
}
