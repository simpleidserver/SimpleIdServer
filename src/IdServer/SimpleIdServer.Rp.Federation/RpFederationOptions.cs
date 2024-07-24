// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.Rp.Federation;

public class RpFederationOptions
{
    /// <summary>
    /// Enable or disable federation.
    /// </summary>
    public bool IsFederationEnabled { get; set; }
    /// <summary>
    /// Organization name.
    /// </summary>
    public string OrganizationName { get; set; }
    /// <summary>
    /// Key used to sign the response.
    /// </summary>
    public SigningCredentials SigningCredentials { get; set; }
    /// <summary>
    /// OPENID client of the relying party.
    /// </summary>
    public Client Client { get; set; }
}