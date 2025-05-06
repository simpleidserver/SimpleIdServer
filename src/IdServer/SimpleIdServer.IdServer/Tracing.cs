// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Diagnostics;

namespace SimpleIdServer.IdServer;

public static class Tracing
{
    public static ActivitySource BasicActivitySource = new(Names.Basic);

    public static ActivitySource StoreActivitySource = new(Names.Store);

    public static ActivitySource CacheActivitySource = new(Names.Cache);

    public static class CommonTagNames
    {
        public const string Realm  = "realm";
    }

    public static class UserTagNames
    {
        private const string root = "user";
        public const string Id = root + ".id";
    }

    public static class IdserverTagNames
    {
        private const string root = "oauth";
        public const string GrantType = root + ".grant_type";
        public const string ClientId = root + ".client_id";
    }

    public static class Names
    {
        public const string Basic = "Basic";
        public const string Store = "Store";
        public const string Cache = "Cache";
    }
}
