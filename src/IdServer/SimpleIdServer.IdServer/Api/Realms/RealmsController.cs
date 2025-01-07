// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static SimpleIdServer.IdServer.Constants;

namespace SimpleIdServer.IdServer.Api.Realms;

public class RealmsController : BaseController
{
    private readonly IBusControl _busControl;
    private readonly IRealmRepository _realmRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IScopeRepository _scopeRepository;
    private readonly IFileSerializedKeyStore _fileSerializedKeyStore;
    private readonly IGroupRepository _groupRepository;
    private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IUserSessionResitory _userSessionRepository;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IRegistrationWorkflowRepository _registrationWorkflowRepository;
    private readonly ILogger<RealmsController> _logger;

    public RealmsController(
        IBusControl busControl,
        IRealmRepository realmRepository, 
        IUserRepository userRepository,
        IClientRepository clientRepository,
        IScopeRepository scopeRepository,
        IFileSerializedKeyStore fileSerializedKeyStore,
        IGroupRepository groupRepository,
        IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository,
        ITransactionBuilder transactionBuilder,
        IUserSessionResitory userSessionRepository,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder,
        IRecurringJobManager recurringJobManager,
        IRegistrationWorkflowRepository registrationWorkflowRepository,
        ILogger<RealmsController> logger) : base(tokenRepository, jwtBuilder)
    {
        _busControl = busControl;
        _realmRepository = realmRepository;
        _userRepository = userRepository;
        _clientRepository = clientRepository;
        _scopeRepository = scopeRepository;
        _fileSerializedKeyStore = fileSerializedKeyStore;
        _groupRepository = groupRepository;
        _transactionBuilder = transactionBuilder;
        _userSessionRepository = userSessionRepository;
        _recurringJobManager = recurringJobManager;
        _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
        _registrationWorkflowRepository = registrationWorkflowRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.Realms.Name);
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
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] AddRealmRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add realm"))
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Realms.Name);
                    var existingRealm = await _realmRepository.Get(request.Name, cancellationToken);
                    if (existingRealm != null) throw new OAuthException(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.RealmExists, request.Name));
                    var realm = new Realm { Name = request.Name, Description = request.Description, CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow };
                    var administratorRole = RealmRoleBuilder.BuildAdministrativeRole(realm);
                    var users = await _userRepository.GetUsersBySubjects(Constants.RealmStandardUsers, Constants.DefaultRealm, cancellationToken);
                    var groups = await _groupRepository.GetAllByStrictFullPath(Constants.DefaultRealm, Constants.RealmStandardGroupsFullPath, cancellationToken);
                    var clients = await _clientRepository.GetAll(Constants.DefaultRealm, Constants.RealmStandardClients, cancellationToken);
                    var scopes = await _scopeRepository.GetAll(Constants.DefaultRealm, Constants.RealmStandardScopes, cancellationToken);
                    var keys = await _fileSerializedKeyStore.GetAll(Constants.DefaultRealm, cancellationToken);
                    var acrs = await _authenticationContextClassReferenceRepository.GetAll(Constants.DefaultRealm, cancellationToken);
                    var registrationMethods = acrs.Select(c => c.RegistrationWorkflowId).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();
                    var registrationWorkflows = await _registrationWorkflowRepository.GetByIds(Constants.DefaultRealm, registrationMethods, cancellationToken);
                    _realmRepository.Add(realm);
                    foreach (var user in users)
                    {
                        user.Realms.Add(new RealmUser { RealmsName = request.Name });
                        _userRepository.Update(user);
                    }

                    foreach(var group in groups)
                    {
                        group.Realms.Add(new GroupRealm { RealmsName = request.Name });
                        if(group.FullPath == StandardGroups.AdministratorGroup.FullPath)
                        {
                            foreach(var scope in administratorRole)
                                group.Roles.Add(scope);
                        }

                        _groupRepository.Update(group);
                    }

                    foreach (var client in clients)
                    {
                        client.Realms.Add(realm);
                        _clientRepository.Update(client);
                    }

                    foreach (var scope in scopes)
                    {
                        scope.Realms.Add(realm);
                        _scopeRepository.Update(scope);
                    }

                    foreach (var key in keys)
                    {
                        key.Realms.Add(realm);
                        _fileSerializedKeyStore.Update(key);
                    }

                    var registrationWorkflowIds = new Dictionary<string, string>();
                    foreach (var registrationWorkflow in registrationWorkflows)
                    {
                        var id = Guid.NewGuid().ToString();
                        registrationWorkflowIds.Add(registrationWorkflow.Id, id);
                        _registrationWorkflowRepository.Add(new RegistrationWorkflow
                        {
                            Id = id,
                            RealmName = realm.Name,
                            Name = registrationWorkflow.Name,
                            CreateDateTime = DateTime.UtcNow,
                            IsDefault = registrationWorkflow.IsDefault,
                            UpdateDateTime = registrationWorkflow.UpdateDateTime,
                            WorkflowId = registrationWorkflow.WorkflowId
                        });
                    }

                    foreach (var acr in acrs)
                    {
                        string registrationWorkflowId = null;
                        if(!string.IsNullOrWhiteSpace(acr.RegistrationWorkflowId) && registrationWorkflowIds.ContainsKey(acr.RegistrationWorkflowId))
                        {
                            registrationWorkflowId = registrationWorkflowIds[acr.RegistrationWorkflowId];
                        }

                        realm.AuthenticationContextClassReferences.Add(new AuthenticationContextClassReference
                        {
                            CreateDateTime = DateTime.UtcNow,
                            DisplayName = acr.DisplayName,
                            Id = Guid.NewGuid().ToString(),
                            Name = acr.Name,
                            RegistrationWorkflowId = registrationWorkflowId,
                            UpdateDateTime = DateTime.UtcNow
                        });
                    }

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

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove realm"))
        {
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.Realms.Name);
                if (id == Constants.DefaultRealm) throw new OAuthException(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.CannotRemoveMasterRealm);
                using (var transaction = _transactionBuilder.Build())
                {
                    var existingRealm = await _realmRepository.Get(id, cancellationToken);
                    if (existingRealm == null) throw new OAuthException(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownRealm, id));
                    var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{RemoveRealmCommandConsumer.Queuename}"));
                    await sendEndpoint.Send(new RemoveRealmCommand
                    {
                        Realm = id
                    });
                    activity?.SetStatus(ActivityStatusCode.Ok, $"Realm {id} is removed");
                    return new NoContentResult();
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
