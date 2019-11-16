// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SCIMSchemaAttribute : Attribute
    {
        public SCIMSchemaAttribute(params string[] schemas)
        {
            Schemas = schemas;
        }

        public IEnumerable<string> Schemas { get; }
    }
}