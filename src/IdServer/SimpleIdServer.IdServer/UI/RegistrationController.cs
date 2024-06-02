// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
	private readonly IdServerHostOptions _options;

	public RegistrationController(
		IRegistrationWorkflowRepository registrationWorkflowRepository, 
		IDistributedCache distributedCache, 
		ITokenRepository tokenRepository,
		IJwtBuilder jwtBuilder,
		IOptions<IdServerHostOptions> options) : base(tokenRepository, jwtBuilder)
	{
		_registrationWorkflowRepository = registrationWorkflowRepository;
		_distributedCache = distributedCache;
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
		var amr = registrationWorkflow.Steps.First();
		var cookieName = _options.GetRegistrationCookieName();
		var registrationProgressId = Guid.NewGuid().ToString();
		var registrationProgress = new UserRegistrationProgress 
		{ 
			RegistrationProgressId = registrationProgressId, 
			Amr = amr, 
			WorkflowName = registrationWorkflow.Name, 
			Realm = prefix, 
			Steps = registrationWorkflow.Steps,
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