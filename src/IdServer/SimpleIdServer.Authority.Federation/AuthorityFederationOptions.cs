// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Authority.Federation;

public class AuthorityFederationOptions
{
    /// <summary>
    /// Organization name.
    /// </summary>
    public string OrganizationName { get; set; }
    /// <summary>
    /// Key used to sign the response.
    /// </summary>
    public SigningCredentials SigningCredentials { get; set; }
}