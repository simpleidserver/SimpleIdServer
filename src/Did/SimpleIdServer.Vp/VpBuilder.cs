// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Threading;

namespace SimpleIdServer.Vp;

public class VpBuilder
{
    private VpBuilder()
    {
        
    }

    public static VpBuilder New()
    {
        return new VpBuilder();
    }
}