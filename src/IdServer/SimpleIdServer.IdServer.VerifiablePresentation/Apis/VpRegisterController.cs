// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
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
    public async Task<IActionResult> Status([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        try
        {
            var cachedValue = await _distributedCache.GetStringAsync(id, cancellationToken);
            if (cachedValue == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.StateIsNotValid);
            var vpPendingAuthorization = JsonSerializer.Deserialize<VpPendingAuthorization>(cachedValue);
            if (!vpPendingAuthorization.IsAuthorized) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.VpNotReceived);
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
        prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
        try
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            if (request == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
            var cachedValue = await _distributedCache.GetStringAsync(request.State, cancellationToken);
            if (cachedValue == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.StateIsNotValid);
            var vpPendingAuthorization = JsonSerializer.Deserialize<VpPendingAuthorization>(cachedValue);
            if (!vpPendingAuthorization.IsAuthorized) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.VpNotReceived);
            var userRegistrationProgress = await GetRegistrationProgress();
            var registrationResult = await CreateUser(issuer, userRegistrationProgress, prefix, Constants.AMR, cancellationToken);
            await _distributedCache.RemoveAsync(request.State, cancellationToken);
            return new OkObjectResult(registrationResult);
        }
        catch(OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    protected async Task<VpEndRegisterResult> CreateUser(string issuer,
        UserRegistrationProgress registrationProgress, 
        string prefix, 
        string amr, 
        CancellationToken cancellationToken)
    {
        var user = registrationProgress.User ?? new Domains.User
        {
            Id = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
        var lastStep = registrationProgress.Steps.Last();
        if (lastStep == amr)
        {
            user.Realms.Add(new Domains.RealmUser
            {
                RealmsName = prefix
            });
            _userRepository.Add(user);
            await _userRepository.SaveChanges(cancellationToken);
            return new VpEndRegisterResult();
        }

        registrationProgress.NextAmr();
        registrationProgress.User = user;
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(registrationProgress);
        await _distributedCache.SetStringAsync(registrationProgress.RegistrationProgressId, json);
        var nextRegistrationRedirectUrl = $"{issuer}/{prefix}/{registrationProgress.Amr}/register";
        return new VpEndRegisterResult
        {
            NextRegistrationRedirectUrl = nextRegistrationRedirectUrl
        };
    }

    private async Task<UserRegistrationProgress> GetRegistrationProgress()
    {
        var cookieName = _idServerHostOptions.GetRegistrationCookieName();
        if (!Request.Cookies.ContainsKey(cookieName)) return null;
        var cookieValue = Request.Cookies[cookieName];
        var json = await _distributedCache.GetStringAsync(cookieValue);
        if (string.IsNullOrWhiteSpace(json)) return null;
        var registrationProgress = Newtonsoft.Json.JsonConvert.DeserializeObject<UserRegistrationProgress>(json);
        return registrationProgress;
    }
}
