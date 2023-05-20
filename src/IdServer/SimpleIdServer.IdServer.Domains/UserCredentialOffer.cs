// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class UserCredentialOffer
    {
        public string Id { get; set; } = null!;
        public string CredentialTemplateId { get; set; } = null!;
        public ICollection<string> CredentialNames { get; set; } = new List<string>();
        public string UserId { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public User User { get; set; } = null!;
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public UserCredentialOfferStatus Status { get; set; } = UserCredentialOfferStatus.VALID;
        public string? Pin { get; set; } = null;
        public string? PreAuthorizedCode { get; set; } = null;
        public string? CredIssuerState { get; set; } = null;
        public CredentialTemplate CredentialTemplate { get; set; } = null!;
    }

    public enum UserCredentialOfferStatus
    {
        VALID = 0,
        INVALID = 1
    }
}
