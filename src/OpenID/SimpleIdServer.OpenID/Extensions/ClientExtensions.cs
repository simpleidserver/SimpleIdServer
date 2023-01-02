// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Domains
{
    public static class ClientExtensions
    {
        public static double? GetDefaultMaxAge(this Client client) => client.GetDoubleParameter("default_max_age");
    }
}
