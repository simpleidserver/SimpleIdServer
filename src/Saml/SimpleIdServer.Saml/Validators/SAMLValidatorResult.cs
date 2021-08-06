// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Xml;

namespace SimpleIdServer.Saml.Validators
{
    public class SAMLValidatorResult<T> where T : class
    {
        public T Content { get; set; }
        public XmlDocument Document { get; set; }
    }
}
