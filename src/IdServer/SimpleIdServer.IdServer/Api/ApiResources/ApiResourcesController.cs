// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
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
    private readonly IJwtBuilder _jwtBuilder;
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
        _jwtBuilder = jwtBuilder;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.ApiResources.Name);
            IQueryable<ApiResource> query = _apiResourceRepository.Query()
                .Include(p => p.Realms)
                .Include(p => p.Scopes)
                .Where(p => p.Realms.Any(r => r.Name == prefix))
                .AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.Filter))
                query = query.Where(request.Filter);

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
                query = query.OrderBy(request.OrderBy);
            var nb = query.Count();
            var apiResources = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
            return new OkObjectResult(new SearchResult<ApiResource>
            {
                Count = nb,
                Content = apiResources
            });
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] AddApiResourceRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add API resource"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.ApiResources.Name);
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
                if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, ApiResourceNames.Name));
                if (await _apiResourceRepository.Query().Include(r => r.Realms).AsNoTracking().AnyAsync(r => r.Name == request.Name && r.Realms.Any(r => r.Name == prefix))) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.APIRESOURCE_ALREADY_EXISTS, request.Name));
                var realm = await _realmRepository.Query().SingleAsync(r => r.Name == prefix);
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
                await _apiResourceRepository.SaveChanges(CancellationToken.None);
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
}
