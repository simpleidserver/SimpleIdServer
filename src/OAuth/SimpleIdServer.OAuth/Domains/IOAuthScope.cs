// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.OAuth.Domains
{
    public interface IOAuthScope
    {
        string Name { get; set; }
        bool IsExposedInConfigurationEdp { get; set; }
        DateTime CreateDateTime { get; set; }
        DateTime UpdateDateTime { get; set; }
    }
}