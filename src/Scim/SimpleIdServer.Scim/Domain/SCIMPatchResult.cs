// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.DTOs;

namespace SimpleIdServer.Scim.Domain
{
    public class SCIMPatchResult
    {
        public string Path { get; set; }
        public SCIMRepresentationAttribute Attr { get; set; }
        public SCIMPatchOperations Operation { get; set; }
    }
}
