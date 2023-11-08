// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Clients;

public class ClientsController : BaseController
{
    private readonly IClientRepository _clientRepository;
    private readonly IRealmRepository _realmRepository;
    private readonly IBusControl _busControl;
    private readonly IJwtBuilder _jwtBuilder;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(IClientRepository clientRepository, IRealmRepository realmRepository, IBusControl busControl, IJwtBuilder jwtBuilder, ILogger<ClientsController> logger)
    {
        _clientRepository = clientRepository;
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
            CheckAccessToken(prefix, Constants.StandardScopes.ApiResources.Name, _jwtBuilder);
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
            var apiResources = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
            /*
            return new OkObjectResult(new SearchResult<ClientResult>
            {
                Count = nb,
                Content = apiResources.Select(p => Build(p)).ToList()
            });
            */
            return null;
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }
}
