// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.ApiResources;

public class ApiResourcesController : BaseController
{
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IRealmRepository _realmRepository;
    private readonly IBusControl _busControl;
    private readonly ILogger<ApiResourcesController> _logger;

    public ApiResourcesController(
        IApiResourceRepository apiResourceRepository, 
        IRealmRepository realmRepository, 
        IBusControl busControl, 
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder, 
        ILogger<ApiResourcesController> logger) : base(tokenRepository, jwtBuilder)
    {
        _apiResourceRepository = apiResourceRepository;
        _realmRepository = realmRepository;
        _busControl = busControl;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.ApiResources.Name);
            var result = await _apiResourceRepository.Search(prefix, request, cancellationToken);
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] AddApiResourceRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add API resource"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.ApiResources.Name);
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
                if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, ApiResourceNames.Name));
                await _apiResourceRepository.StartTransaction();
                var existingApiResource = await _apiResourceRepository.GetByName(prefix, request.Name, cancellationToken);
                if (existingApiResource != null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.ApiResourceAlreadyExists, request.Name));
                var realm = await _realmRepository.Get(prefix, cancellationToken);
                var apiResource = new ApiResource
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = request.Name,
                    Audience = request.Audience,
                    Description = request.Description,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                };
                apiResource.Realms.Add(realm);
                _apiResourceRepository.Add(apiResource);
                await _apiResourceRepository.CommitTransaction();
                activity?.SetStatus(ActivityStatusCode.Ok, $"API resource {request.Name} added");
                await _busControl.Publish(new AddApiResourceSuccessEvent
                {
                    Realm = prefix,
                    Name = request.Name,
                    Audience = request.Audience
                });
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(apiResource).ToString(),
                    ContentType = "application/json"
                };
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new AddApiResourceFailureEvent
                {
                    Realm = prefix,
                    Name = request.Name,
                    Audience = request.Audience
                });
                return BuildError(ex);
            }
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity($"Remove API resource {id}"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.Scopes.Name);
                await _apiResourceRepository.StartTransaction();
                var apiResource = await _apiResourceRepository.Get(prefix, id, cancellationToken);
                if (apiResource == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownApiResource, id));
                activity?.SetStatus(ActivityStatusCode.Ok, $"API resource {id} is removed");
                _apiResourceRepository.Delete(apiResource);
                await _apiResourceRepository.CommitTransaction();
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }
}
