// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer;
var builder = WebApplication.CreateBuilder(args);

const string connectionString = "Data Source=localhost;Initial Catalog=OtherIdServer;User Id=sa;Password=tJWBx3ccNJ6dyp1wxoA99qqQ;TrustServerCertificate=True";
builder.AddSidIdentityServer()
    .AddDeveloperSigningCredential()
    .UseEfStore(e =>
    {
        e.UseSqlServer(connectionString, o =>
        {
            o.MigrationsAssembly("SimpleIdServer.IdServer.SqlServerMigrations");
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    }, e =>
    {
        e.UseSqlServer(connectionString, o =>
        {
            o.MigrationsAssembly("SidFormBuilder.SqlServerMigrations");
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    });

var app = builder.Build();
app.Services.SeedData();
app.UseSid();
app.Run();