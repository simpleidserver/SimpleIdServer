// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SimpleIdServer.OpenID.Startup
{
    public class Startup
    {
        public Startup(IHostingEnvironment env) { }

        public void ConfigureServices(IServiceCollection services)
        {
            JsonWebKey sigJsonWebKey;
            using (var rsa = RSA.Create())
            {
                var json = File.ReadAllText("oauth_key.txt");
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                rsa.Import(dic);
                sigJsonWebKey = new JsonWebKeyBuilder().NewSign("1", new[]
                {
                    KeyOperations.Sign,
                    KeyOperations.Verify
                }).SetAlg(rsa, "RS256").Build();
            }

            services.AddSIDOpenID()
                .AddClients(DefaultConfiguration.Clients)
                .AddAcrs(DefaultConfiguration.AcrLst)
                .AddUsers(DefaultConfiguration.Users)
                .AddScopes(DefaultConfiguration.Scopes)
                .AddJsonWebKeys(new List<JsonWebKey> { sigJsonWebKey })
                .AddLoginPasswordAuthentication();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSID();
        }
    }
}