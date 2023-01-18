// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Builders
{
    public class ApiResourceBuilder
    {
        private readonly ApiResource _apiResource;

        private ApiResourceBuilder(ApiResource scope)
        {
            _apiResource = scope;
        }

        public static ApiResourceBuilder Create(string name, string description, params string[] scopes)
        {
            return new ApiResourceBuilder(new ApiResource { Name = name, Description = description, Scopes = scopes.Select(s => new Scope { Name = s }).ToList()  });
        }

        public static ApiResourceBuilder Create(string name, string description)
        {
            return new ApiResourceBuilder(new ApiResource { Name = name, Description = description, Scopes = new List<Scope> { new Scope { Name = name } } });
        }

        public ApiResource Build() => _apiResource;
    }
}
