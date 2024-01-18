// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Scim.Benchmark
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            new ScimBenchmark().AddUserToGroupOneByOne().Wait();
        }
    }
}
