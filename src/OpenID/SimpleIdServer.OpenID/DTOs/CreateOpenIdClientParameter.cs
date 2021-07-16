// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;

namespace SimpleIdServer.OpenID.DTOs
{
    public class CreateOpenIdClientParameter
    {
        public ApplicationKinds? ApplicationKind { get; set; }
        public string ClientName { get; set; }
    }
}
