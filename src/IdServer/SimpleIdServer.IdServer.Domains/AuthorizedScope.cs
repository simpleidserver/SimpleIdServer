// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class AuthorizedScope
    {
        public string Scope { get; set; } = null!;
        public ICollection<string> Resources { get; set; } = new List<string>();
    }
}
