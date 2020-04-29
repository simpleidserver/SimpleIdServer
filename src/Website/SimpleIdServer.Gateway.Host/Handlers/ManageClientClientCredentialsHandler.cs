// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;

namespace SimpleIdServer.Gateway.Host.Handlers
{
    public class ManageClientClientCredentialsHandler : BaseClientCredentialsHandler
    {
        public ManageClientClientCredentialsHandler(IConfiguration configuration) : base(configuration, new[] { "manage_clients" }) { }
    }
}
