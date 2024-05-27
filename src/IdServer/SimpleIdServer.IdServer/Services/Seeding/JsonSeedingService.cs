// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.DTOs.Seeds;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Services.Seeding.Interfaces;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

/// <summary>
/// implements the methods to seed from an external resource.
/// </summary>
namespace SimpleIdServer.IdServer.Services.Seeding
{
    internal class JsonSeedingService : IJsonSeedingService
    {
        private readonly JsonSeedingOptions _jsonSeedingOptions;
        private readonly ISeederService<UserSeedDto> _userSeederService;

        public JsonSeedingService(IOptions<JsonSeedingOptions> jsonSeedingOptions, ISeederService<UserSeedDto> userSeederService)
        {
            _jsonSeedingOptions = jsonSeedingOptions.Value;
            _userSeederService = userSeederService;
        }

        public async Task<SeedsDto?> GetDataFromResourceAsync()
        {
            string jsonText = await File.ReadAllTextAsync(_jsonSeedingOptions.JsonFilePath);
            var jsonSerializationOptions = new JsonSerializerOptions() { AllowTrailingCommas = true };
            SeedsDto? seedsDto = JsonSerializer.Deserialize<SeedsDto>(jsonText, jsonSerializationOptions);
            return seedsDto;
        }

        public async Task SeedDataAsync()
        {
            SeedsDto? seeds = await GetDataFromResourceAsync();
            if (seeds != null) await SeedDataAsync(seeds);
        }

        public async Task SeedDataAsync(SeedsDto seeds)
        {
            if (seeds.Users.Count > 0)
            {
                await _userSeederService.SeedAsync(seeds.Users);
            }
        }
    }
}
