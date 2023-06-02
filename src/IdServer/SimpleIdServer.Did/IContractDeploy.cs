// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Threading.Tasks;

namespace SimpleIdServer.Did
{
    public interface IContractDeploy
    {
        string Type { get; }
        Task<string> Deploy(NetworkConfiguration networkConfiguration);
    }
}
