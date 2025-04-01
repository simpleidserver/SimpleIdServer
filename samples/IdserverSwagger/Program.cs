// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.TokenTypes;
using System.Collections.Generic;
var builder = WebApplication.CreateBuilder(args);

var users = new List<User>
{
    UserBuilder.Create("administrator", "password", "Administrator").SetEmail("adm@mail.com").SetFirstname("Administrator").Build()
};
builder.AddSidIdentityServer(o =>
    {
        o.Authority = "https://localhost:5001";
    })
    .AddDeveloperSigningCredential()
    .AddInMemoryUsers(users)
    .AddPwdAuthentication(true)
    .AddSwagger(opt =>
    {
        opt.IncludeDocumentation<AccessTokenTypeService>();
    })
    .SeedSwagger(new List<string> { "https://localhost:5001/swagger/oauth2-redirect.html" });

var app = builder.Build();
app.Services.SeedData();
app.UseSid();
app.UseSidSwagger();
app.UseSidSwaggerUi();
app.Run();