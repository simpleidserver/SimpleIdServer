// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer;
var builder = WebApplication.CreateBuilder(args);

const string connectionString = "server=localhost;port=3306;database=mydatabase;user=admin;password=tJWBx3ccNJ6dyp1wxoA99qqQ";
builder.AddSidIdentityServer()
    .AddDeveloperSigningCredential()
    .UseEfStore(e =>
    {
        e.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
        {
            o.MigrationsAssembly("SimpleIdServer.IdServer.MySQLMigrations");
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    }, e =>
    {
        e.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
        {
            o.MigrationsAssembly("SidFormBuilder.MySQLMigrations");
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    });

var app = builder.Build();
app.Services.SeedData();
app.UseSid();
app.Run();