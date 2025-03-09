// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSidIdentityServer()
    .UseInMemoryEFStore();

var app = builder.Build();
app.UseSID();
app.Run();