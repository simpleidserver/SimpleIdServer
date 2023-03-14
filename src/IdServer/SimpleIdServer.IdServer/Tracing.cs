// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Diagnostics;

namespace SimpleIdServer.IdServer
{
    public class Tracing
    {
        public static ActivitySource IdServerActivitySource = new(ActivitySourceName);

        public const string ActivitySourceName = "IdServer";

        public static void Init()
        {
            IdServerActivitySource = new(ActivitySourceName);
        }
    }
}
