// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using BenchmarkDotNet.Running;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Benchmark
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
