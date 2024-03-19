// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.VerifiablePresentation.DTOs;
using SimpleIdServer.IdServer.VerifiablePresentation.Resources;
using System.Net;
using System.Text.Json;

namespace SimpleIdServer.IdServer.VerifiablePresentation.Apis;

public class VpRegisterController : BaseController
{
    private readonly IDistributedCache _distributedCache;
    private readonly IUserRepository _userRepository;
    private readonly IdServerHostOptions _idServerHostOptions;
    private readonly ILogger<VpRegisterController> _logger;

    public VpRegisterController(
        IDistributedCache distributedCache,
        IUserRepository userRepository,
        Microsoft.Extensions.Options.IOptions<IdServerHostOptions> idServerHostOptions,
        ILogger<VpRegisterController> logger,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _distributedCache = distributedCache;
        _userRepository = userRepository;
        _idServerHostOptions = idServerHostOptions.Value;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Status([FromRoute] string prefix, string state, CancellationToken cancellationToken)
    {
        try
        {
            var cachedValue = await _distributedCache.GetStringAsync(state, cancellationToken);
            if (cachedValue == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.StateIsNotValid);
            return new NoContentResult();
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> EndRegister([FromRoute] string prefix, [FromBody] VpEndRegisterRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new OAuthException();
        var cachedValue = await _distributedCache.GetStringAsync(request.State, cancellationToken);
        if (cachedValue == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.StateIsNotValid);
        var vpPendingAuthorization = JsonSerializer.Deserialize<VpPendingAuthorization>(cachedValue);
        if (!vpPendingAuthorization.IsAuthorized) throw new OAuthException();
        var userRegistrationProgress = await GetRegistrationProgress();
        var lastStep = userRegistrationProgress.Steps.Last();
        if(lastStep == Constants.AMR)
        {
            // REGISTER THE USER.
        }


        if (userRegistrationProgress.)
    }

    protected async Task CreateUser(UserRegistrationProgress registrationProgress, string prefix, string amr, CancellationToken cancellationToken)
    {
        var user = registrationProgress.User ?? new Domains.User
        {
            Id = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
        // ADD VERIFIABLE CREDENTIAL.
        var lastStep = registrationProgress.Steps.Last();
        if (lastStep == amr)
        {
            user.Realms.Add(new Domains.RealmUser
            {
                RealmsName = prefix
            });
            _userRepository.Add(user);
            await _userRepository.SaveChanges(CancellationToken.None);
            return;
        }

        registrationProgress.NextAmr();
        registrationProgress.User = user;
        var json = JsonConvert.SerializeObject(registrationProgress);
        await _distributedCache.SetStringAsync(registrationProgress.RegistrationProgressId, json);
        // NEXT AMR.
        return;
    }

    private async Task<UserRegistrationProgress> GetRegistrationProgress()
    {
        var cookieName = _idServerHostOptions.GetRegistrationCookieName();
        if (!Request.Cookies.ContainsKey(cookieName)) return null;
        var cookieValue = Request.Cookies[cookieName];
        var json = await _distributedCache.GetStringAsync(cookieValue);
        if (string.IsNullOrWhiteSpace(json)) return null;
        var registrationProgress = JsonConvert.DeserializeObject<UserRegistrationProgress>(json);
        return registrationProgress;
    }
}
