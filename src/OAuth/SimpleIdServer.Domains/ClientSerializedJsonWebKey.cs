// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Domains
{
    public class ClientSerializedJsonWebKey
    {
        public string Kid { get; set; } = null!;
        public string Content {  get; set; } = null!;
    }
}
