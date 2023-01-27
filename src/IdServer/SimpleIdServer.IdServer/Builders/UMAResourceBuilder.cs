// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Builders
{
    public class UMAResourceBuilder
    {
        private readonly UMAResource _umaResource;

        private UMAResourceBuilder(string id, IEnumerable<string> scopes)
        {
            _umaResource = new UMAResource
            {
                Id = id,
                Scopes = scopes.ToList()
            };
        }

        public static UMAResourceBuilder Create(string id, params string[] scopes)
        {
            var result = new UMAResourceBuilder(id, scopes);
            return result;
        }

        public UMAResource Build() => _umaResource;
    }
}
