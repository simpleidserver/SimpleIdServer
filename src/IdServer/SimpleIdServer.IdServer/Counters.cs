// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.Metrics;

namespace SimpleIdServer.IdServer;

public static class Counters
{
    public static string ServiceName => typeof(Counters).Assembly.GetName().Name;

    public static string ServiceVersion => typeof(Counters).Assembly.GetName().Version.ToString();

    private static readonly Meter Meter = new Meter(ServiceName, ServiceVersion);

    public static class Names
    {
        public const string User = "user_registration_total";
    }

    private static Counter<long> UserCounter = Meter.CreateCounter<long>(Names.User);

    public static void UserRegistered()
    {
        UserCounter.Add(1);
    }
}