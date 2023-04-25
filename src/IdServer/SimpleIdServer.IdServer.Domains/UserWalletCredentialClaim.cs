// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class UserWalletCredentialClaim
    {
        public string Id { get; set; } = null!;
        public string Path { get; set; } = null!;
        public string Value { get; set; } = null!;
        public UserWalletCredential WalletCredential { get; set; } = null!;
        public string WalletCredentialId { get; set; } = null!;
    }
}
