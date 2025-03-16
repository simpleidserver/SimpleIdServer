// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Config;

public static class DefaultUsers
{
    public static List<string> AllUserNames => new List<string>
    {
        Administrator.Name,
        ReadonlyAdministrator.Name
    };

    public static User Administrator = UserBuilder.Create("administrator", "password", "Administrator").SetFirstname("Administrator").SetEmail("adm@email.com").SetPhoneNumber("0485").SetPicture("https://cdn-icons-png.flaticon.com/512/149/149071.png").AddGroup(DefaultGroups.AdministratorGroup).GenerateRandomTOTPKey().Build();
    public static User ReadonlyAdministrator = UserBuilder.Create("administrator-ro", "password", "AdministratorRo").SetFirstname("AdministratorRo").SetEmail("adm-ro@email.com").SetPicture("https://cdn-icons-png.flaticon.com/512/149/149071.png").AddGroup(DefaultGroups.AdministratorReadonlyGroup).GenerateRandomTOTPKey().Build();
}
