// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace SimpleIdServer.Did.Helpers;

public static class DateTimeHelper
{
    public static long GetTime()
    {
        DateTime utcNow = DateTime.UtcNow;
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        long ts = (long)((utcNow - epoch).TotalMilliseconds);
        return ts;
    }
}
