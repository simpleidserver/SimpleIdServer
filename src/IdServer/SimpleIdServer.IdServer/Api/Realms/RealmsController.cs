// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace SimpleIdServer.IdServer.Api.Realms;

public class RealmsController : BaseController
{
    private readonly IRealmRepository _realmRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly IFileSerializedKeyStore _fileSerializedKeyStore;
    private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly ILogger<RealmsController> _logger;

    public RealmsController(
        IRealmRepository realmRepository, 
        IUserRepository userRepository,
        IClientRepository clientRepository,
        IScopeRepository scopeRepository,
        IFileSerializedKeyStore fileSerializedKeyStore,
        IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository,
        ITransactionBuilder transactionBuilder,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder,
        ILogger<RealmsController> logger) : base(tokenRepository, jwtBuilder)
    {
        _realmRepository = realmRepository;
        _userRepository = userRepository;
        _clientRepository = clientRepository;
        _scopeRepository = scopeRepository;
        _fileSerializedKeyStore = fileSerializedKeyStore;
        _transactionBuilder = transactionBuilder;
        _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            await CheckAccessToken(Constants.DefaultRealm, Constants.StandardScopes.Realms.Name);
            var realms = await _realmRepository.GetAll(cancellationToken);
            return new OkObjectResult(realms);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddRealmRequest request, CancellationToken cancellationToken)
    {
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add realm"))
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(Constants.DefaultRealm, Constants.StandardScopes.Realms.Name);
                    var existingRealm = await _realmRepository.Get(request.Name, cancellationToken);
                    if (existingRealm != null) throw new OAuthException(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.RealmExists, request.Name));
                    var realm = new Realm { Name = request.Name, Description = request.Description, CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow };
                    var users = await _userRepository.GetUsersBySubjects(Constants.RealmStandardUsers, Constants.DefaultRealm, cancellationToken);
                    var clients = await _clientRepository.GetAll(Constants.DefaultRealm, Constants.RealmStandardClients, cancellationToken);
                    var scopes = await _scopeRepository.GetAll(Constants.DefaultRealm, Constants.RealmStandardScopes, cancellationToken);
                    var keys = await _fileSerializedKeyStore.GetAll(Constants.DefaultRealm, cancellationToken);
                    var acrs = await _authenticationContextClassReferenceRepository.GetAll(cancellationToken);
                    foreach (var user in users)
                    {
                        user.Realms.Add(new RealmUser { RealmsName = request.Name });
                        _userRepository.Update(user);
                    }

                    foreach (var client in clients)
                        client.Realms.Add(realm);

                    foreach (var scope in scopes)
                    {
                        scope.Realms.Add(realm);
                        _scopeRepository.Update(scope);
                    }

                    foreach (var acr in acrs)
                        acr.Realms.Add(realm);

                    foreach (var key in keys)
                    {
                        key.Realms.Add(realm);
                        _fileSerializedKeyStore.Update(key);
                    }

                    _realmRepository.Add(realm);
                    await transaction.Commit(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, $"Realm {request.Name} is added");
                    return new ContentResult
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Content = JsonSerializer.Serialize(realm).ToString(),
                        ContentType = "application/json"
                    };
                }
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }
}
