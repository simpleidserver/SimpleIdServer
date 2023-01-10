// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Builders
{
    public class ScopeBuilder
    {
        private readonly Scope _scope;

        private ScopeBuilder(Scope scope)
        {
            _scope = scope;
        }

        public static ScopeBuilder Create(string name, bool isExposed = false)
        {
            return new ScopeBuilder(new Scope { Name = name, IsExposedInConfigurationEdp = isExposed });
        }

        public Scope Build() => _scope;
    }
}
