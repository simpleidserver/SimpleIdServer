// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Config;

public static class DefaultGrantManagementActions
{
    public static List<string> All => new List<string>
    {
        Create,
        Merge,
        Replace
    };

    public const string Create = "create";
    public const string Merge = "merge";
    public const string Replace = "replace";
}
