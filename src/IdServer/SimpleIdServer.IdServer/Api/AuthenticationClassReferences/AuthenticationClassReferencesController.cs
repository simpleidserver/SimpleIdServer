// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Builders;
using FormBuilder.Helpers;
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.AuthenticationClassReferences;

public class AuthenticationClassReferencesController : BaseController
{
    private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;
    private readonly IWorkflowStore _workflowStore;
    private readonly IRealmRepository _realmRepository;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IFormStore _formStore;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IEnumerable<IWorkflowLayoutService> _workflowLayoutServices;
    private readonly ILogger<AuthenticationClassReferencesController> _logger;

    public AuthenticationClassReferencesController(
        IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository, 
        IWorkflowStore workflowStore,
        IRealmRepository realmRepository,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder,
        ITransactionBuilder transactionBuilder,
        IFormStore formStore,
        IDateTimeHelper dateTimeHelper,
        IEnumerable<IWorkflowLayoutService> workflowLayoutServices,
        ILogger<AuthenticationClassReferencesController> logger) : base(tokenRepository, jwtBuilder)
    {
        _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
        _workflowStore = workflowStore;
        _realmRepository = realmRepository;
        _transactionBuilder = transactionBuilder;
        _formStore = formStore;
        _dateTimeHelper = dateTimeHelper;
        _workflowLayoutServices = workflowLayoutServices;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Config.DefaultScopes.Acrs.Name);
            var result = await _authenticationContextClassReferenceRepository.GetAll(prefix, cancellationToken);
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllForms([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        await CheckAccessToken(prefix, Config.DefaultScopes.Acrs.Name);
        var result = await _formStore.GetByCategory(prefix, FormCategories.Authentication, cancellationToken);
        return new OkObjectResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWorkflowLayouts([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Config.DefaultScopes.Acrs.Name);
            var result = _workflowLayoutServices.Where(w => w.Category == FormCategories.Authentication).Select(w => w.Get());
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Config.DefaultScopes.Acrs.Name);
            var acr = await _authenticationContextClassReferenceRepository.Get(prefix, id, cancellationToken);
            if (acr == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.UNKNOWN_ACR, string.Format(Global.UnknownAcr, id));
            return new OkObjectResult(acr);
        }
        catch(OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] AddAuthenticationClassReferenceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckAccessToken(prefix, Config.DefaultScopes.Acrs.Name);
                await Validate();
                var realm = await _realmRepository.Get(prefix, cancellationToken);
                var record = new AuthenticationContextClassReference
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = request.Name,
                    DisplayName = request.DisplayName,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                };
                record.Realms.Add(realm);
                var workflow = new WorkflowRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    Realm = prefix,
                    UpdateDateTime = _dateTimeHelper.GetCurrent(),
                    Links = new List<WorkflowLink>(),
                    Steps = new List<WorkflowStep>()
                };
                record.AuthenticationWorkflow = workflow.Id;
                _workflowStore.Add(workflow);
                await _workflowStore.SaveChanges(cancellationToken);
                _authenticationContextClassReferenceRepository.Add(record);
                await transaction.Commit(cancellationToken);
                return new ContentResult
                {
                    Content = JsonSerializer.Serialize(record),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.Created
                };
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }

        async Task Validate()
        {
            if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
            if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, AuthenticationContextClassReferenceNames.Name));
            var existingAcr = await _authenticationContextClassReferenceRepository.GetByName(prefix, request.Name, cancellationToken);
            if (existingAcr != null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.AcrWithSameNameExists);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
            using (var transaction = _transactionBuilder.Build())
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckAccessToken(prefix, Config.DefaultScopes.Acrs.Name);
                var acr = await _authenticationContextClassReferenceRepository.Get(prefix, id, cancellationToken);
                if (acr == null)
                {
                    return BuildError(HttpStatusCode.NotFound, ErrorCodes.UNKNOWN_ACR, string.Format(Global.UnknownAcr, id));
                }

                _authenticationContextClassReferenceRepository.Delete(acr);
                if (!string.IsNullOrWhiteSpace(acr.AuthenticationWorkflow))
                {
                    var workflow = await _workflowStore.Get(prefix, acr.AuthenticationWorkflow, cancellationToken);
                    if(workflow != null)
                    {
                        _workflowStore.Delete(workflow);
                        await _workflowStore.SaveChanges(cancellationToken);
                    }
                }

                await transaction.Commit(cancellationToken);
                return new NoContentResult();
            }
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkflow([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckAccessToken(prefix, Config.DefaultScopes.Acrs.Name);
                var realm = await _realmRepository.Get(prefix, cancellationToken);
                var existingAcr = await _authenticationContextClassReferenceRepository.Get(prefix, id, cancellationToken);
                if(existingAcr == null)
                {
                    return BuildError(HttpStatusCode.NotFound, ErrorCodes.UNKNOWN_ACR, string.Format(Global.UnknownAcr, id));
                }

                var workflowId = Guid.NewGuid().ToString();
                var workflow = WorkflowBuilder.New(workflowId, existingAcr.Name).Build(_dateTimeHelper.GetCurrent());
                _workflowStore.Add(workflow);
                existingAcr.AuthenticationWorkflow = workflowId;
                await _workflowStore.SaveChanges(cancellationToken);
                await transaction.Commit(cancellationToken);
                return new ContentResult
                {
                    Content = JsonSerializer.Serialize(workflow),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }
}
