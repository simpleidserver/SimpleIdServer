// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Migration;

public class MigrateUsersCommand
{
    public required string Name
    {
        get; set;
    }

    public required string Realm
    {
        get; set;
    }
}
