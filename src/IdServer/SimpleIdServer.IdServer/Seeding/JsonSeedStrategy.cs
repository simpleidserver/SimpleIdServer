﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Options;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Seeding;

/// <summary>
/// Implements the methods to seed from a JSON file.
/// </summary>
public class JsonSeedStrategy : ISeedStrategy
{
    private readonly JsonSeedingOptions _jsonSeedingOptions;
    private readonly ILogger<JsonSeedStrategy> _logger;
    private readonly IEntitySeeder<RealmSeedDto> _realmsSeeder;
    private readonly IEntitySeeder<ScopeSeedDto> _scopesSeeder;
    private readonly IEntitySeeder<UserSeedDto> _usersSeeder;

    public JsonSeedStrategy(
        IOptions<JsonSeedingOptions> jsonSeedingOptions,
        ILogger<JsonSeedStrategy> logger,
        IEntitySeeder<RealmSeedDto> realmsSeeder,
        IEntitySeeder<ScopeSeedDto> scopesSeeder,
        IEntitySeeder<UserSeedDto> usersSeeder)
    {
        _jsonSeedingOptions = jsonSeedingOptions.Value;
        _logger = logger;
        _realmsSeeder = realmsSeeder;
        _scopesSeeder = scopesSeeder;
        _usersSeeder = usersSeeder;
    }

    /// <inheritdoc/>
    public async Task<SeedsDto?> GetDataFromResourceAsync(CancellationToken cancellationToken = default)
    {
        string jsonText = await File.ReadAllTextAsync(_jsonSeedingOptions.JsonFilePath, cancellationToken);
        var jsonSerializationOptions = new JsonSerializerOptions() { AllowTrailingCommas = true };
        SeedsDto? seedsDto = JsonSerializer.Deserialize<SeedsDto>(jsonText, jsonSerializationOptions);
        return seedsDto;
    }

    /// <inheritdoc/>
    public async Task SeedDataAsync(CancellationToken cancellationToken = default)
    {
        if (_jsonSeedingOptions.SeedFromJson)
        {
            _logger.LogTrace("Seeding from JSON file started.");
            SeedsDto? seeds = await GetDataFromResourceAsync(cancellationToken);
            if (seeds != null) await SeedDataAsync(seeds, cancellationToken);
            _logger.LogTrace("Seeding from JSON file ended.");
        }
    }

    /// <inheritdoc/>
    public async Task SeedDataAsync(SeedsDto seeds, CancellationToken cancellationToken = default)
    {
        if (seeds.Realms.Count > 0)
        {
            await _realmsSeeder?.SeedAsync(seeds.Realms , cancellationToken);
        }

        if (seeds.Scopes.Count > 0)
        {
            await _scopesSeeder?.SeedAsync(seeds.Scopes, cancellationToken);
        }

        if (seeds.Users.Count > 0)
        {
            await _usersSeeder?.SeedAsync(seeds.Users, cancellationToken);
        }
    }
}
