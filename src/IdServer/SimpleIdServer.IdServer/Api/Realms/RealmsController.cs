// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Realms;

[AllowAnonymous]
public class RealmsController : BaseController
{
    private readonly IRealmRepository _realmRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly IFileSerializedKeyStore _fileSerializedKeyStore;
    private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;
    private readonly ILogger<RealmsController> _logger;

    public RealmsController(
        IRealmRepository realmRepository, 
        IUserRepository userRepository,
        IClientRepository clientRepository,
        IScopeRepository scopeRepository,
        IFileSerializedKeyStore fileSerializedKeyStore,
        IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder,
        ILogger<RealmsController> logger) : base(tokenRepository, jwtBuilder)
    {
        _realmRepository = realmRepository;
        _userRepository = userRepository;
        _clientRepository = clientRepository;
        _scopeRepository = scopeRepository;
        _fileSerializedKeyStore = fileSerializedKeyStore;
        _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            await CheckAccessToken(Constants.DefaultRealm, Constants.StandardScopes.Realms.Name);
            var realms = await _realmRepository.Query().AsNoTracking().ToListAsync();
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
                await CheckAccessToken(Constants.DefaultRealm, Constants.StandardScopes.Realms.Name);
                var realmExists = await _realmRepository.Query()
                    .AsNoTracking()
                    .AnyAsync(r => r.Name == request.Name);
                if (realmExists) throw new OAuthException(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.REALM_EXISTS, request.Name));
                var realm = new Realm { Name = request.Name, Description = request.Description, CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow };
                var users = await _userRepository.GetUsersBySubjects(Constants.RealmStandardUsers, Constants.DefaultRealm, cancellationToken);
                var clients = await _clientRepository.Query().Include(c => c.Realms).Where(c => Constants.RealmStandardClients.Contains(c.ClientId)).ToListAsync();
                var scopes = await _scopeRepository.Query().Include(s => s.Realms).Where(s => Constants.RealmStandardScopes.Contains(s.Name)).ToListAsync();
                var keys = await _fileSerializedKeyStore.Query().Include(s => s.Realms).Where(s => s.Realms.Any(r => r.Name == Constants.DefaultRealm)).ToListAsync();
                var acrs = await _authenticationContextClassReferenceRepository.Query().Include(a => a.Realms).ToListAsync();
                foreach (var user in users)
                    user.Realms.Add(new RealmUser { RealmsName = request.Name });

                foreach (var client in clients)
                    client.Realms.Add(realm);

                foreach (var scope in scopes)
                    scope.Realms.Add(realm);

                foreach (var acr in acrs)
                    acr.Realms.Add(realm);

                foreach (var key in keys)
                    key.Realms.Add(realm);

                _realmRepository.Add(realm);
                await _userRepository.SaveChanges(CancellationToken.None);
                await _clientRepository.SaveChanges(CancellationToken.None);
                await _scopeRepository.SaveChanges(CancellationToken.None);
                await _fileSerializedKeyStore.SaveChanges(CancellationToken.None);
                await _authenticationContextClassReferenceRepository.SaveChanges(CancellationToken.None);
                await _realmRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Realm {request.Name} is added");
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(realm).ToString(),
                    ContentType = "application/json"
                };
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
