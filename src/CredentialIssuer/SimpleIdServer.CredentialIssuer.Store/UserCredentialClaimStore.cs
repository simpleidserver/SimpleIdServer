// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface IUserCredentialClaimStore
{
    Task<List<UserCredentialClaim>> Resolve(string subject, List<CredentialTemplateClaim> claims);
}

public class UserCredentialClaimStore : IUserCredentialClaimStore
{
    public Task<List<UserCredentialClaim>> Resolve(string subject, List<CredentialTemplateClaim> claims)
    {
        throw new NotImplementedException();
    }
}