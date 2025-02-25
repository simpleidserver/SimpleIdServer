// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Helpers;
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
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

namespace SimpleIdServer.IdServer.Api.RegistrationWorkflows;

public class RegistrationWorkflowsController : BaseController
{
	private readonly IRegistrationWorkflowRepository _registrationWorkflowRepository;
    private readonly IWorkflowStore _workflowStore;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IFormStore _formStore;
    private readonly IEnumerable<IWorkflowLayoutService> _workflowLayoutServices;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly ILogger<RegistrationWorkflowsController> _logger;

    public RegistrationWorkflowsController(
        IRegistrationWorkflowRepository registrationWorkflowRepository, 
        ITokenRepository tokenRepository,
        IWorkflowStore workflowStore,
        IJwtBuilder jwtBuilder, 
        IEnumerable<IAuthenticationMethodService> authenticationMethodServices,
        ITransactionBuilder transactionBuilder,
        IFormStore formStore,
        IEnumerable<IWorkflowLayoutService> workflowLayoutServices,
        IDateTimeHelper dateTimeHelper,
        ILogger<RegistrationWorkflowsController> logger) : base(tokenRepository, jwtBuilder)
	{
		_registrationWorkflowRepository = registrationWorkflowRepository;
        _workflowStore = workflowStore;
        _transactionBuilder = transactionBuilder;
        _formStore = formStore;
        _workflowLayoutServices = workflowLayoutServices;
        _dateTimeHelper = dateTimeHelper;
        _logger = logger;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll([FromRoute] string prefix, CancellationToken cancellationToken)
	{
		prefix = prefix ?? Constants.DefaultRealm;
        var registrationWorkflows = await _registrationWorkflowRepository.GetAll(prefix, cancellationToken);
		return new OkObjectResult(registrationWorkflows.Select(r => Build(r)));
	}

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
		var registrationWorkflow = await _registrationWorkflowRepository.Get(prefix, id, cancellationToken);
		if (registrationWorkflow == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, Global.UnknownRegistrationWorkflow);
		return new OkObjectResult(Build(registrationWorkflow));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
		try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.RegistrationWorkflows.Name);
                var registrationWorkflow = await _registrationWorkflowRepository.Get(prefix, id, cancellationToken);
                if (registrationWorkflow == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, Global.UnknownRegistrationWorkflow);
                if (!string.IsNullOrWhiteSpace(registrationWorkflow.WorkflowId))
                {
                    var workflow = await _workflowStore.Get(prefix, registrationWorkflow.WorkflowId, cancellationToken);
                    if (workflow != null)
                    {
                        _workflowStore.Delete(workflow);
                        await _workflowStore.SaveChanges(cancellationToken);
                    }
                }

                _registrationWorkflowRepository.Delete(registrationWorkflow);
                await transaction.Commit(cancellationToken);
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] RegistrationWorkflowResult request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.RegistrationWorkflows.Name);
                if (string.IsNullOrWhiteSpace(request.Name)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, RegistrationWorkflowNames.Name));
                var existingRegistrationWorkflow = await _registrationWorkflowRepository.GetByName(prefix, request.Name, cancellationToken);
                if (existingRegistrationWorkflow != null) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.RegistrationWorkflowExists);
                if (request.IsDefault)
                {
                    var defaultRegistrationWorkflow = await _registrationWorkflowRepository.GetDefault(prefix, cancellationToken);
                    if (defaultRegistrationWorkflow != null) defaultRegistrationWorkflow.IsDefault = false;
                }

                var workflow = new WorkflowRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    Realm = prefix,
                    UpdateDateTime = _dateTimeHelper.GetCurrent(),
                    Links = new List<WorkflowLink>(),
                    Steps = new List<WorkflowStep>()
                };
                _workflowStore.Add(workflow);
                await _workflowStore.SaveChanges(cancellationToken);
                var builder = RegistrationWorkflowBuilder.New(request.Name, workflow.Id.ToString(), request.IsDefault, prefix);
                var record = builder.Build();
                _registrationWorkflowRepository.Add(record);
                await transaction.Commit(cancellationToken);
                var json = JsonSerializer.Serialize(new RegistrationWorkflowResult
                {
                    CreateDateTime = record.CreateDateTime,
                    UpdateDateTime = record.UpdateDateTime,
                    Id = record.Id,
                    IsDefault = record.IsDefault,
                    Name = record.Name,
                    WorkflowId = record.WorkflowId
                });
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = json,
                    ContentType = "application/json"
                };
            }
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] RegistrationWorkflowResult request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.RegistrationWorkflows.Name);
                if (string.IsNullOrWhiteSpace(request.Name)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, RegistrationWorkflowNames.Name));
                var existingRegistrationWorkflow = await _registrationWorkflowRepository.Get(prefix, id, cancellationToken);
                if (existingRegistrationWorkflow == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownRegistrationWorkflow, id));
                existingRegistrationWorkflow.UpdateDateTime = DateTime.UtcNow;
                existingRegistrationWorkflow.IsDefault = request.IsDefault;
                var defaultRegistrationWorkflow = await _registrationWorkflowRepository.GetDefault(prefix, cancellationToken);
                if (defaultRegistrationWorkflow != null)
                {
                    defaultRegistrationWorkflow.IsDefault = false;
                    defaultRegistrationWorkflow.UpdateDateTime = DateTime.UtcNow;
                }

                _registrationWorkflowRepository.Update(existingRegistrationWorkflow);
                _registrationWorkflowRepository.Update(defaultRegistrationWorkflow);
                await transaction.Commit(cancellationToken);
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWorkflowLayouts([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Constants.StandardScopes.Acrs.Name);
            var result = _workflowLayoutServices.Where(w => w.Category == FormCategories.Registration).Select(w => w.Get());
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
        await CheckAccessToken(prefix, Constants.StandardScopes.Acrs.Name);
        var result = await _formStore.GetByCategory(prefix, FormCategories.Registration, cancellationToken);
        return new OkObjectResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetForms([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        await CheckAccessToken(prefix, Constants.StandardScopes.Register.Name);
        var forms = await _formStore.GetByCategory(prefix, FormCategories.Registration, cancellationToken);
        return new OkObjectResult(forms);
    }

    private static RegistrationWorkflowResult Build(RegistrationWorkflow r) => new RegistrationWorkflowResult
	{
		Id = r.Id,
		CreateDateTime = r.CreateDateTime,
		IsDefault = r.IsDefault,
		Name = r.Name,
        WorkflowId = r.WorkflowId,
		UpdateDateTime = r.UpdateDateTime
	};
}
