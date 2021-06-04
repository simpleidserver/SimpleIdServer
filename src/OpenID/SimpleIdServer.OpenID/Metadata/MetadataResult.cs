// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.Metadata
{
    public class MetadataResult
    {
        public MetadataResult()
        {
            Content = new Dictionary<string, MetadataRecord>();
        }

        public Dictionary<string, MetadataRecord> Content { get; set; }
    }
}
