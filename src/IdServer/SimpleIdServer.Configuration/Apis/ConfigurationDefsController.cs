// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Configuration.DTOs;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Configuration.Apis;

public class ConfigurationDefsController : BaseController
{
	private readonly IConfigurationDefinitionStore _configurationDefinitionStore;
	private readonly IJwtBuilder _jwtBuilder;

	public ConfigurationDefsController(IConfigurationDefinitionStore configurationDefinitionStore, IJwtBuilder jwtBuilder)
	{
        _configurationDefinitionStore = configurationDefinitionStore;
		_jwtBuilder = jwtBuilder;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll([FromRoute] string prefix)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        CheckAccessToken(prefix, Constants.ConfigurationsScope.Name, _jwtBuilder);
        var confDefs = await _configurationDefinitionStore.Query()
            .Include(c => c.Records).ThenInclude(r => r.Values)
            .Include(c => c.Records).ThenInclude(r => r.Translations)
            .AsNoTracking()
            .ToListAsync();
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
                Type = r.Type,
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