// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
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
	public async Task<IActionResult> GetAll([FromRoute] string prefix)
	{
		prefix = prefix ?? Constants.DefaultRealm;
		var registrationWorkflows = await _registrationWorkflowRepository.Query().AsNoTracking().Where(r => r.RealmName == prefix).OrderByDescending(r => r.UpdateDateTime).ToListAsync();
		return new OkObjectResult(registrationWorkflows.Select(r => Build(r)));
	}

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id)
    {
        prefix = prefix ?? Constants.DefaultRealm;
		var registrationWorkflow = await _registrationWorkflowRepository.Query().AsNoTracking().FirstOrDefaultAsync(r => r.RealmName == prefix && r.Id == id);
		if (registrationWorkflow == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, ErrorMessages.UNKNOWN_REGISTRATION_WORKFLOW);
		return new OkObjectResult(Build(registrationWorkflow));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string prefix, string id)
    {
        prefix = prefix ?? Constants.DefaultRealm;
		try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.RegistrationWorkflows.Name);
            var registrationWorkflow = await _registrationWorkflowRepository.Query().FirstOrDefaultAsync(r => r.RealmName == prefix && r.Id == id);
            if (registrationWorkflow == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, ErrorMessages.UNKNOWN_REGISTRATION_WORKFLOW);
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
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] RegistrationWorkflowResult request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.RegistrationWorkflows.Name);
            if (string.IsNullOrWhiteSpace(request.Name)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, RegistrationWorkflowNames.Name));
            if (request.Steps == null || !request.Steps.Any()) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, RegistrationWorkflowNames.Steps));
            var existingAmrs = _authenticationMethodServices.Select(a => a.Amr);
            var unknownAmrs = request.Steps.Where(s => !existingAmrs.Contains(s));
            if (unknownAmrs.Any()) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_AUTHENTICATION_METHODS, string.Join(",", unknownAmrs)));
            var existingRegistrationWorkflow = await _registrationWorkflowRepository.Query().AsNoTracking().FirstOrDefaultAsync(r => r.RealmName == prefix && r.Name == request.Name);
            if (existingRegistrationWorkflow != null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.REGISTRATION_WORFKLOW_EXISTS);
            if(request.IsDefault)
            {
                var defaultRegistrationWorkflow = await _registrationWorkflowRepository.Query().FirstOrDefaultAsync(r => r.RealmName == prefix && r.IsDefault);
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
    public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] RegistrationWorkflowResult request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.RegistrationWorkflows.Name);
            if (string.IsNullOrWhiteSpace(request.Name)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, RegistrationWorkflowNames.Name));
            if (request.Steps == null || !request.Steps.Any()) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, RegistrationWorkflowNames.Steps));
            var existingAmrs = _authenticationMethodServices.Select(a => a.Amr);
            var unknownAmrs = request.Steps.Where(s => !existingAmrs.Contains(s));
            if (unknownAmrs.Any()) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_AUTHENTICATION_METHODS, string.Join(",", unknownAmrs)));
            var existingRegistrationWorkflow = await _registrationWorkflowRepository.Query().FirstOrDefaultAsync(r => r.RealmName == prefix && r.Id == id);
            if (existingRegistrationWorkflow == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_REGISTRATION_WORKFLOW, id));
            existingRegistrationWorkflow.UpdateDateTime = DateTime.UtcNow;
            existingRegistrationWorkflow.IsDefault = request.IsDefault;
            existingRegistrationWorkflow.Steps = request.Steps;
            var defaultRegistrationWorkflow = await _registrationWorkflowRepository.Query().FirstOrDefaultAsync(r => r.RealmName == prefix && r.IsDefault);
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
