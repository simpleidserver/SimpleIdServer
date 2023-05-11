// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class UserCredentialOffer
    {
        public string Id { get; set; } = null!;
        public ICollection<string> CredentialNames { get; set; } = new List<string>();
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
