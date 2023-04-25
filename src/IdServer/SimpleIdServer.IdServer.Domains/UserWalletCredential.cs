// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class UserWalletCredential
    {
        public string Id { get; set; } = null!;
        public ICollection<string> ContextUrls { get; set; } = new List<string>();
        /// <summary>
        /// The credential types, which declare what data to except in the credential.
        /// </summary>
        public ICollection<string> CredentialTypes { get; set; } = new List<string>();
        /// <summary>
        /// The entity that issued the credential.
        /// </summary>
        public string? Issuer { get; set; } = null;
        /// <summary>
        /// When the credential was issued.
        /// </summary>
        public DateTime IssuanceDate { get; set; }
        /// <summary>
        /// Claims about the subjects of the credential.
        /// </summary>
        public ICollection<UserWalletCredentialClaim> Claims { get; set; } = new List<UserWalletCredentialClaim>();
        public User User { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }
}
