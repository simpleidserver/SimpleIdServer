// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using SimpleIdServer.IdServer.Light.Startup.Infras;

var builder = WebApplication.CreateBuilder(args);
ClientCredentialsConfig.Run(builder);