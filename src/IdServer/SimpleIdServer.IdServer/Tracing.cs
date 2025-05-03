// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Diagnostics;

namespace SimpleIdServer.IdServer;

public class Tracing
{
    public static ActivitySource ApiActivitySource = new(Names.Api);

    public static ActivitySource UserInfoSource = new(Names.UserInfo);

    public static ActivitySource RegisterActivitySource = new(Names.Register);

    public static ActivitySource TokenActivitySource = new(Names.Token);

    public static ActivitySource AuthzActivitySource = new(Names.Authz);

    public static class Names
    {
        public const string UserInfo = "UserInfo";
        public const string Api = "Api";
        public const string Register = "Register";
        public const string Token = "Token";
        public const string Authz = "Authz";
    }
}
