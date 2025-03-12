// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.UI;

public class UserLockingOptions
{
    [ConfigurationRecord("Account lock time", "Lock time of the user account (in seconds)", order: 1)]
    public int LockTimeInSeconds { get; set; }
    [ConfigurationRecord("Max login attempts", "Number of login attempts", order: 2)]
    public int MaxLoginAttempts { get; set; }
}