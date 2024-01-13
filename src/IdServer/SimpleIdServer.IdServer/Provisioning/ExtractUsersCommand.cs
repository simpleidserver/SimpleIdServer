// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Provisioning;

public class ExtractUsersCommand
{
    public string InstanceId { get; set; }
    public string Realm { get; set; }
    public string ProcessId { get; set; }
    public int BatchSize { get; set; }
    public int Page { get; set; }
}
