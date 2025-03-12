// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Configuration.Models;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Stores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.ConfDefs;

public class ConfigurationDefsController : BaseController
{
	private readonly IConfigurationDefinitionStore _configurationDefinitionStore;

	public ConfigurationDefsController(
        IConfigurationDefinitionStore configurationDefinitionStore, 
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
	{
        _configurationDefinitionStore = configurationDefinitionStore;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll([FromRoute] string prefix)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        await CheckAccessToken(prefix, IdServer.Constants.StandardScopes.ConfigurationsScope.Name);
        var confDefs = await _configurationDefinitionStore.GetAll(CancellationToken.None);
        return Ok(confDefs.Select(d => Build(d)));
    }

    private static ConfigurationDefResult Build(ConfigurationDefinition confDef)
    {
        var result = new ConfigurationDefResult
        {
            Id = confDef.Id,
            CreateDateTime = confDef.CreateDateTime,
            UpdateDateTime = confDef.UpdateDateTime,
            Properties = confDef.Records.Select(r => new ConfigurationDefRecordResult
            {
                Id = r.Id,
                Description = r.Description,
                DisplayName = r.DisplayName,
                Order = r.Order,
                Name = r.Name,
                IsRequired = r.IsRequired,
                Type = r.Type,
                DisplayCondition = r.DisplayCondition,
                PossibleValues = r.Values.Select(v => new ConfigurationDefRecordValueResult
                {
                    Id = v.Id,
                    Name = v.Name,
                    Value = v.Value
                }).ToList()
            }).ToList()
        };
        return result;
    }
}