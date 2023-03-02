// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace SimpleIdServer.IdServer.UI.AuthProviders
{
    [AttributeUsage(AttributeTargets.Property)]
    public class VisibleAuthSchemeAttribute : Attribute
    {
        public VisibleAuthSchemeAttribute(string displayName, string? description = null)
        {
            DisplayName = displayName;
            Description = description;
        }

        public string DisplayName { get; private set; }
        public string Description { get; private set; }
    }
}
