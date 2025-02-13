// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Repositories;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI;

public class RegistrationController : BaseController
{
	private readonly IRegistrationWorkflowRepository _registrationWorkflowRepository;
	private readonly IDistributedCache _distributedCache;
	private readonly IWorkflowStore _workflowStore;
	private readonly IFormStore _formStore;
	private readonly IdServerHostOptions _options;

	public RegistrationController(
		IRegistrationWorkflowRepository registrationWorkflowRepository, 
		IDistributedCache distributedCache,
        IWorkflowStore workflowStore,
		IFormStore formStore,
        ITokenRepository tokenRepository,
		IJwtBuilder jwtBuilder,
		IOptions<IdServerHostOptions> options) : base(tokenRepository, jwtBuilder)
	{
		_registrationWorkflowRepository = registrationWorkflowRepository;
		_distributedCache = distributedCache;
		_workflowStore = workflowStore;
        _formStore = formStore;
        _options = options.Value;
	}

	[HttpGet]
	public async Task<IActionResult> Index([FromRoute] string prefix, string? workflowName = null, string? redirectUrl = null)
	{
		prefix = prefix ?? Constants.DefaultRealm;
		RegistrationWorkflow registrationWorkflow = null;
		if (string.IsNullOrWhiteSpace(workflowName)) registrationWorkflow = await _registrationWorkflowRepository.GetDefault(prefix, CancellationToken.None);
		else registrationWorkflow = await _registrationWorkflowRepository.GetByName(prefix, workflowName, CancellationToken.None);
		if(registrationWorkflow == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, Global.UnknownRegistrationWorkflow);
		var workflow = await _workflowStore.Get(prefix, registrationWorkflow.WorkflowId, CancellationToken.None);
		if(workflow == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownWorkflow, registrationWorkflow.WorkflowId));
		var cookieName = _options.GetRegistrationCookieName();
		var allForms = await _formStore.GetAll(CancellationToken.None);
		var registrationProgressId = Guid.NewGuid().ToString();
		var workflowSteps = workflow.Steps.Select(s => allForms.Single(f => f.CorrelationId == s.FormRecordCorrelationId)).Where(s => s.ActAsStep);
		var amr = workflowSteps.First().Name;
        var registrationProgress = new UserRegistrationProgress 
		{ 
			WorkflowId = registrationWorkflow.WorkflowId,
			RegistrationProgressId = registrationProgressId, 
			Realm = prefix, 
			RedirectUrl = redirectUrl
		};
        Response.Cookies.Append(cookieName, registrationProgress.RegistrationProgressId, new CookieOptions
        {
            Secure = true
        });
        await _distributedCache.SetStringAsync(registrationProgress.RegistrationProgressId, Newtonsoft.Json.JsonConvert.SerializeObject(registrationProgress));
        return RedirectToAction("Index", "Register", new { area = amr });
	}
}