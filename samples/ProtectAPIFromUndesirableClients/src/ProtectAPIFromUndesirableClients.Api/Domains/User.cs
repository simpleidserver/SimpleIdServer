// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace ProtectAPIFromUndesirableClients.Api.Domains
{
    public class User
    {
        public User(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
