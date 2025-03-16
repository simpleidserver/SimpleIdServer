// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;

namespace SimpleIdServer.IdServer.Builders
{
    public class ApiResourceBuilder
    {
        private readonly ApiResource _apiResource;

        private ApiResourceBuilder(ApiResource scope)
        {
            _apiResource = scope;
        }

        public static ApiResourceBuilder Create(string name, string description, params Scope[] scopes)
        {
            var record = new ApiResource { Id = Guid.NewGuid().ToString(), Name = name, Description = description, Scopes = scopes };
            record.Realms.Add(Config.DefaultRealms.Master);
            return new ApiResourceBuilder(record);
        }

        public static ApiResourceBuilder Create(string name, string audience, string description, params Scope[] scopes)
        {
            var record = new ApiResource { Id = Guid.NewGuid().ToString(), Name = name, Description = description, Scopes = scopes, Audience = audience };
            record.Realms.Add(Config.DefaultRealms.Master);
            return new ApiResourceBuilder(record);
        }

        public ApiResource Build() => _apiResource;
    }
}
