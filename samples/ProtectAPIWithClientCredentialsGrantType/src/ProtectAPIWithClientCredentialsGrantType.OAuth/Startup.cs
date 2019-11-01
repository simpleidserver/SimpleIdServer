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

namespace ProtectAPIWithClientCredentialsGrantType.OAuth
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            JsonWebKey sigJsonWebKey;
            using (var rsa = RSA.Create())
            {
                var json = File.ReadAllText(Path.Combine(_env.ContentRootPath, "oauth_key.txt"));
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                rsa.Import(dic);
                sigJsonWebKey = new JsonWebKeyBuilder().NewSign("1", new[]
                {
                    KeyOperations.Sign,
                    KeyOperations.Verify
                }).SetAlg(rsa, "RS256").Build();
            }

            services.AddSIDOAuth()
				.AddScopes(DefaultConfiguration.Scopes)
                .AddClients(DefaultConfiguration.Clients)
                .AddJsonWebKeys(new List<JsonWebKey> { sigJsonWebKey });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSID();
        }
    }
}