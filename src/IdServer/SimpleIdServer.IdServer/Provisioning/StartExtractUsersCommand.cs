// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Provisioning;

public class StartExtractUsersCommand
{
    public string InstanceId { get; set; } = null!;
    public string Realm { get; set; } = null!;
    public string ProcessId { get; set; } = null!;
}