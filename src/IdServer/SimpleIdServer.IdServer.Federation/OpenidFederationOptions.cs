// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Federation;

public class OpenidFederationOptions
{
    /// <summary>
    /// Key Identifier of the signature key, used to sign all the response.
    /// </summary>
    public string TokenSignedKid { get; set; } = SimpleIdServer.IdServer.Constants.StandardKeys.First().KeyId;
    /// <summary>
    /// Enable or disable the OPENID federation.
    /// </summary>
    public bool IsFederationEnabled { get; set; }
    /// <summary>
    /// Organization name.
    /// </summary>
    public string OrganizationName { get; set; } = "SID";
}