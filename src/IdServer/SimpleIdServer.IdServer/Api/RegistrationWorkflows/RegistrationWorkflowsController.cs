// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
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

namespace SimpleIdServer.IdServer.Api.RegistrationWorkflows;

public class RegistrationWorkflowsController : BaseController
{
	private readonly IRegistrationWorkflowRepository _registrationWorkflowRepository;
    private readonly IEnumerable<IAuthenticationMethodService> _authenticationMethodServices;

    public RegistrationWorkflowsController(
        IRegistrationWorkflowRepository registrationWorkflowRepository, 
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder, 
        IEnumerable<IAuthenticationMethodService> authenticationMethodServices) : base(tokenRepository, jwtBuilder)
	{
		_registrationWorkflowRepository = registrationWorkflowRepository;
        _authenticationMethodServices = authenticationMethodServices;
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
		if (registrationWorkflow == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, Global.UnknownRegistrationWorkflow);
		return new OkObjectResult(Build(registrationWorkflow));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
		try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.RegistrationWorkflows.Name);
            var registrationWorkflow = await _registrationWorkflowRepository.Get(prefix, id, cancellationToken);
            if (registrationWorkflow == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, Global.UnknownRegistrationWorkflow);
            _registrationWorkflowRepository.Delete(registrationWorkflow);
            await _registrationWorkflowRepository.SaveChanges(CancellationToken.None);
            return new NoContentResult();
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
            await CheckAccessToken(prefix, Constants.StandardScopes.RegistrationWorkflows.Name);
            if (string.IsNullOrWhiteSpace(request.Name)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, RegistrationWorkflowNames.Name));
            if (request.Steps == null || !request.Steps.Any()) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, RegistrationWorkflowNames.Steps));
            var existingAmrs = _authenticationMethodServices.Select(a => a.Amr);
            var unknownAmrs = request.Steps.Where(s => !existingAmrs.Contains(s));
            if (unknownAmrs.Any()) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownAuthMethods, string.Join(",", unknownAmrs)));
            var existingRegistrationWorkflow = await _registrationWorkflowRepository.GetByName(prefix, request.Name, cancellationToken);
            if (existingRegistrationWorkflow != null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.RegistrationWorkflowExists);
            if(request.IsDefault)
            {
                var defaultRegistrationWorkflow = await _registrationWorkflowRepository.GetDefault(prefix, cancellationToken);
                if (defaultRegistrationWorkflow != null) defaultRegistrationWorkflow.IsDefault = false;
            }

            var builder = RegistrationWorkflowBuilder.New(request.Name, request.IsDefault, prefix);
            foreach (var step in request.Steps) builder.AddStep(step);
            var record = builder.Build();
            _registrationWorkflowRepository.Add(record);
            await _registrationWorkflowRepository.SaveChanges(CancellationToken.None);
            var json = JsonSerializer.Serialize(new RegistrationWorkflowResult
            {
                CreateDateTime = record.CreateDateTime,
                UpdateDateTime = record.UpdateDateTime,
                Id = record.Id,
                IsDefault = record.IsDefault,
                Name = record.Name,
                Steps = record.Steps
            });
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = json,
                ContentType = "application/json"
            };
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
            await CheckAccessToken(prefix, Constants.StandardScopes.RegistrationWorkflows.Name);
            if (string.IsNullOrWhiteSpace(request.Name)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, RegistrationWorkflowNames.Name));
            if (request.Steps == null || !request.Steps.Any()) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, RegistrationWorkflowNames.Steps));
            var existingAmrs = _authenticationMethodServices.Select(a => a.Amr);
            var unknownAmrs = request.Steps.Where(s => !existingAmrs.Contains(s));
            if (unknownAmrs.Any()) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownAuthMethods, string.Join(",", unknownAmrs)));
            var existingRegistrationWorkflow = await _registrationWorkflowRepository.Get(prefix, id, cancellationToken);
            if (existingRegistrationWorkflow == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownRegistrationWorkflow, id));
            existingRegistrationWorkflow.UpdateDateTime = DateTime.UtcNow;
            existingRegistrationWorkflow.IsDefault = request.IsDefault;
            existingRegistrationWorkflow.Steps = request.Steps;
            var defaultRegistrationWorkflow = await _registrationWorkflowRepository.GetDefault(prefix, cancellationToken);
            if(defaultRegistrationWorkflow != null) 
            {
                defaultRegistrationWorkflow.IsDefault = false;
                defaultRegistrationWorkflow.UpdateDateTime = DateTime.UtcNow;
            }

            await _registrationWorkflowRepository.SaveChanges(CancellationToken.None);
            return new NoContentResult();
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    private static RegistrationWorkflowResult Build(RegistrationWorkflow r) => new RegistrationWorkflowResult
	{
		Id = r.Id,
		CreateDateTime = r.CreateDateTime,
		IsDefault = r.IsDefault,
		Name = r.Name,
		Steps = r.Steps,
		UpdateDateTime = r.UpdateDateTime
	};
}
