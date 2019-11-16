// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Scim.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SCIMSchemaPropertyAttribute : Attribute
    {
        public SCIMSchemaPropertyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
