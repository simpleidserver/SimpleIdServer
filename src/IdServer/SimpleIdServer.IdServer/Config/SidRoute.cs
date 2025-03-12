// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Config;

public class SidRoute
{
    public string Name
    {
        get; set;
    }

    public string RelativePattern
    {
        get; set;
    }

    public object Default
    {
        get; set;
    }
}
