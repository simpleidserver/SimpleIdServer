// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class Consent
    {
        public Consent() { }

        public Consent(string id, string clientId, IEnumerable<string> scopes, IEnumerable<string> claims, string realm)
        {
            Id = id;
            ClientId = clientId;
            Scopes = scopes;
            Claims = claims;
            Realm = realm;
        }


        public string Id { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public DateTime CreateDateTime { get; set; }
        public User User { get; set; }
        public string Realm { get; set; }
        public IEnumerable<string> Scopes { get; set; } = new List<string>();
        public IEnumerable<string> Claims { get; set; } = new List<string>();
    }
}
