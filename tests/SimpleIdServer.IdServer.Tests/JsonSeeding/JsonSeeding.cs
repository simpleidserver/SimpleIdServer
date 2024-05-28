// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using mOptions = Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SimpleIdServer.IdServer.DTOs.Seeds;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Services.Seeding;
using SimpleIdServer.IdServer.Services.Seeding.Interfaces;

namespace SimpleIdServer.IdServer.Tests;

[TestFixture]
public class JsonSeeding
{
    private Mock<ILogger<JsonSeedingService>> moqLogger = new();
    private Mock<ISeederService<UserSeedDto>> moqUserSeederService = new();

    [Test]
    public void Can_read_users_from_json_file()
    {
        mOptions.IOptions<JsonSeedingOptions> options = mOptions.Options.Create(new JsonSeedingOptions()
        {
            JsonFilePath = "./JsonSeeding/Seed.json",
            SeedFromJson = true
        });

        ISeedingService seedingService = new JsonSeedingService(options, moqLogger.Object, moqUserSeederService.Object);

        SeedsDto? seedsDto = seedingService.GetDataFromResourceAsync().Result;

        Assert.NotZero(seedsDto!.Users.Count);
        Assert.AreEqual(seedsDto!.Users.ElementAt(1).Login, "user3");
    }
}