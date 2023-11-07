// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api.ApiResources;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Scopes;

public class ScopesController : BaseController
{
    private readonly IScopeRepository _scopeRepository;
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IBusControl _busControl;
    private readonly IJwtBuilder _jwtBuilder;
    private readonly ILogger<ApiResourcesController> _logger;

    public ScopesController(IScopeRepository scopeRepository, IApiResourceRepository apiResourceRepository, IBusControl busControl, IJwtBuilder jwtBuilder, ILogger<ApiResourcesController> logger)
    {
        _scopeRepository = scopeRepository;
        _apiResourceRepository = apiResourceRepository;
        _busControl = busControl;
        _jwtBuilder = jwtBuilder;
        _logger = logger;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateResources([FromRoute] string prefix, string name, [FromBody] UpdateScopeResourcesRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity($"Update API resources of the scope {name}"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                activity?.SetTag("scope", name);
                CheckAccessToken(prefix, Constants.StandardScopes.Scopes.Name, _jwtBuilder);
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
                if (request.Resources == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, ScopeNames.Resources));
                var existingScope = await _scopeRepository.Query().Include(s => s.Realms).Include(s => s.ApiResources).FirstOrDefaultAsync(s => s.Name == name && s.Realms.Any(r => r.Name == prefix));
                if (existingScope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_SCOPE, name));
                var existingApiResources = await _apiResourceRepository.Query().Include(r => r.Realms).Where(r => request.Resources.Contains(r.Name) && r.Realms.Any(re => re.Name == prefix)).ToListAsync();
                var unknownApiResources = request.Resources.Where(r => !existingApiResources.Any(er => er.Name == r));
                if (unknownApiResources.Any()) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_RESOURCE, string.Join(",", unknownApiResources)));
                existingScope.ApiResources.Clear();
                foreach (var apiResource in existingApiResources) existingScope.ApiResources.Add(apiResource);
                await _scopeRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"API resources have been updated for the scope {name}");
                await _busControl.Publish(new UpdateScopeResourcesSuccessEvent
                {
                    Realm = prefix,
                    Name = name,
                    Resources = request.Resources.ToList()
                });
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new UpdateScopeResourcesFailureEvent
                {
                    Realm = prefix,
                    Name = name,
                    Resources = request.Resources?.ToList()
                });
                return BuildError(ex);
            }
        }
    }
}