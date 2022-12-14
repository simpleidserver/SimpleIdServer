// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Domains
{
    public class ClientScope
    {
        public string Name { get; set; } = null!;

        public static implicit operator ClientScope(string name) => new ClientScope { Name = name };
    }
}
