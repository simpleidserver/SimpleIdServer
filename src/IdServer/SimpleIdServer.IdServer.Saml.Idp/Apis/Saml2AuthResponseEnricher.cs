// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens.Saml2;

namespace SimpleIdServer.IdServer.Saml.Idp.Apis;

public interface ISaml2AuthResponseEnricher
{
    void Enrich(Saml2SecurityToken securityToken);
}

public class Saml2AuthResponseEnricher : ISaml2AuthResponseEnricher
{
    public void Enrich(Saml2SecurityToken securityToken)
    {

    }
}
