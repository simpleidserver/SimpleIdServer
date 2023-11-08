// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api.Register;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Clients;

public class ClientsController : BaseController
{
    private readonly IClientRepository _clientRepository;
    private readonly IRegisterClientRequestValidator _registerClientRequestValidator;
    private readonly IBusControl _busControl;
    private readonly IJwtBuilder _jwtBuilder;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(IClientRepository clientRepository, IRegisterClientRequestValidator registerClientRequestValidator, IBusControl busControl, IJwtBuilder jwtBuilder, ILogger<ClientsController> logger)
    {
        _clientRepository = clientRepository;
        _registerClientRequestValidator = registerClientRequestValidator;
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
            CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name, _jwtBuilder);
            IQueryable<Client> query = _clientRepository.Query()
                .Include(c => c.Translations)
                .Include(p => p.Realms)
                .Include(p => p.Scopes)
                .Where(p => p.Realms.Any(r => r.Name == prefix))
                .AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.Filter))
                query = query.Where(request.Filter);

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
                query = query.OrderBy(request.OrderBy);
            var nb = query.Count();
            var clients = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
            return new OkObjectResult(new SearchResult<Client>
            {
                Count = nb,
                Content = clients
            });
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromRoute] string prefix)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name, _jwtBuilder);
            IQueryable<Client> query = _clientRepository.Query()
                .Include(c => c.Translations)
                .Include(p => p.Realms)
                .Include(p => p.Scopes)
                .Where(p => p.Realms.Any(r => r.Name == prefix))
                .AsNoTracking();
            var clients = await query.ToListAsync();
            return new OkObjectResult(clients);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] Client request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add client"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                CheckAccessToken(prefix, Constants.StandardScopes.Clients.Name, _jwtBuilder);
                await _registerClientRequestValidator.Validate(prefix, request, CancellationToken.None);
                _clientRepository.Add(request);
                await _clientRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Client {request.Id} is added");
                await _busControl.Publish(new ClientRegisteredSuccessEvent
                {
                    Realm = prefix,
                    RequestJSON = JsonSerializer.Serialize(request)
                });
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(request).ToString(),
                    ContentType = "application/json"
                };
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new ClientRegisteredFailureEvent
                {
                    Realm = prefix,
                    ErrorMessage = ex.Message,
                    RequestJSON = JsonSerializer.Serialize(request)
                });
                return BuildError(ex);
            }
        }
    }
}
