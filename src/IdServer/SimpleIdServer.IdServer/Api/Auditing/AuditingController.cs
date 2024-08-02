// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Auditing;

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
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchAuditingRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        await CheckAccessToken(prefix, Constants.StandardScopes.Auditing.Name);
        prefix = prefix ?? Constants.DefaultRealm;
        var result = await _repository.Search(prefix, request, cancellationToken);
        return new OkObjectResult(result);
    }
}