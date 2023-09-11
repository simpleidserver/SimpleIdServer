// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Configuration.DTOs;

public class GetConfigurationRequest
{
    public string Id { get; set; }
    public string Definition { get; set; }
    public string Type { get; set; }
}
