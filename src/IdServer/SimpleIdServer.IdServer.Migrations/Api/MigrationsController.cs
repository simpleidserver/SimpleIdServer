// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Migrations.Api;

public class MigrationsController : BaseController
{
    private readonly IEnumerable<IMigrationService> _migrationServices;
    private readonly IMigrationStore _migrationStore;
    private readonly IBusControl _busControl;
    private readonly ILogger<MigrationsController> _logger;

    public MigrationsController(
        IEnumerable<IMigrationService> migrationServices,
        IMigrationStore migrationStore,
        IBusControl busControl,
        ILogger<MigrationsController> logger,
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _migrationServices = migrationServices;
        _migrationStore = migrationStore;
        _busControl = busControl;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDefinitions([FromRoute] string prefix, CancellationToken cancellationToken)
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
    public async Task<IActionResult> GetAllExecutions([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
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
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Migrations.Name);
            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{LaunchMigrationConsumer.QueueName}"));
            await sendEndpoint.Send(new LaunchMigrationCommand
            {
                Name = name,
                Realm = prefix
            });
            return NoContent();
        }
        catch(OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }
}
