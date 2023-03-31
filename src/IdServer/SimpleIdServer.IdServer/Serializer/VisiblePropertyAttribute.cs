// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace SimpleIdServer.IdServer.Serializer
{
    [AttributeUsage(AttributeTargets.Property)]
    public class VisiblePropertyAttribute : Attribute
    {
        public VisiblePropertyAttribute(string displayName, string description = null)
        {
            DisplayName = displayName;
            Description = description;
        }

        public string DisplayName { get; private set; }
        public string Description { get; private set; }
    }
}
