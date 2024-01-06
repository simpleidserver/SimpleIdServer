// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Auditing;

[AllowAnonymous]
public class AuditingController : BaseController
{
    private readonly IAuditEventRepository _repository;

    public AuditingController(
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder, 
        IAuditEventRepository repository) : base(tokenRepository, jwtBuilder)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchAuditingRequest request)
    {
        await CheckAccessToken(prefix, Constants.StandardScopes.Auditing.Name);
        prefix = prefix ?? Constants.DefaultRealm;
        IQueryable<AuditEvent> query = _repository.Query().AsNoTracking().Where(r => r.Realm == prefix);
        if (request.DisplayOnlyErrors)
            query = query.Where(r => r.IsError);

        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);

        var nb = query.Count();
        var result = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync(CancellationToken.None);
        return new OkObjectResult(new SearchResult<AuditEvent>
        {
            Content = result,
            Count = nb
        });
    }
}