// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api.Migrations;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Migration;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Migrate;

public class MigrationsController : BaseController
{
    private readonly IEnumerable<IMigrationService> _migrationServices;
    private readonly IMigrationStore _migrationStore;
    private readonly ILogger<MigrationsController> _logger;

    public MigrationsController(
        IEnumerable<IMigrationService> migrationServices,
        IMigrationStore migrationStore,
        ILogger<MigrationsController> logger,
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _migrationServices = migrationServices;
        _migrationStore = migrationStore;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDefinitions([FromRoute] string prefix, string name, CancellationToken cancellationToken)
    {
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Migrations.Name);
            var result = _migrationServices.Select(s =>
            {
                return new MigrationDefinitionResult
                {
                    Name = s.Name
                };
            });
            return Ok(result);
        }
        catch(OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllExecutions([FromRoute] string prefix, string name, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Migrations.Name);
            var migrationExecutions = await _migrationStore.GetAll(prefix, cancellationToken);
            var migrationServiceNames = _migrationServices.Select(s => s.Name);
            var unknownServiceNames = migrationServiceNames.Where(n => !migrationExecutions.Any(me => me.Name == n));
            migrationExecutions.AddRange(unknownServiceNames.Select(n => new Domains.MigrationExecution
            {
                Name = n
            }));
            return Ok(migrationExecutions);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Launch([FromRoute] string prefix, string name, CancellationToken cancellationToken)
    {
        return null;
    }
}
