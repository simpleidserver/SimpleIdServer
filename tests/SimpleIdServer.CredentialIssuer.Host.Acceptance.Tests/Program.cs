﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using SimpleIdServer.CredentialIssuer.Host.Acceptance.Tests;
using SimpleIdServer.CredentialIssuer.Services;
using SimpleIdServer.Did.Key;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization(b =>
{
    b.AddPolicy("ApiAuthenticated", p => p.RequireAuthenticatedUser());
    b.AddPolicy("WebsiteAuthenticated", p => p.RequireAuthenticatedUser());
    b.AddPolicy("credconfs", p => p.RequireAssertion(_ => true));
    b.AddPolicy("credinstances", p => p.RequireAssertion(_ => true));
});
builder.Services.AddCredentialIssuer(o =>
{
    o.GenerateRandomDidKey();
})
    .UseInMemoryStore(o =>
    {
        o.AddCredentialConfigurations(CredentialIssuerConfiguration.CredentialConfigurations);
        o.AddCredentials(CredentialIssuerConfiguration.Credentials);
        o.AddDeferredCredentials(CredentialIssuerConfiguration.DeferredCredentials);
    });
builder.Services.AddDidKey();
builder.Services.RemoveAll<IPreAuthorizedCodeService>();
var mck = new Mock<IPreAuthorizedCodeService>();
mck.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
    .Returns(Task.FromResult(Guid.NewGuid().ToString()));
builder.Services.AddSingleton(mck.Object);
builder.Services.AddControllers();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }