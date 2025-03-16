// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Config;

public static class DefaultNotificationModes
{
    public static List<string> All => new List<string>
    {
        Poll,
        Ping,
        Push
    };

    public const string Poll = "poll";
    public const string Ping = "ping";
    public const string Push = "push";
}
