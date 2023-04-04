// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class RealmUser
    {
        public string RealmsName { get; set; } = null!;
        public string UsersId { get; set; } = null!;
        public User User { get; set; } = null!;
        public Realm Realm { get; set; } = null!;
    }
}
