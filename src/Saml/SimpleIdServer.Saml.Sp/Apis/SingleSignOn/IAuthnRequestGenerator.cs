// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Xml;

namespace SimpleIdServer.Saml.Sp.Apis.SingleSignOn
{
    public interface IAuthnRequestGenerator
    {
        XmlElement BuildHttpGetBinding();
    }
}
