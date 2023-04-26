// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Key
{
    public class KeyIdentityDocumentBuilder : IdentityDocumentBuilder
    {
        protected KeyIdentityDocumentBuilder(IdentityDocument identityDocument) : base(identityDocument)
        {
        }

        public static KeyIdentityDocumentBuilder NewKey(string did)
        {
            var parsed = IdentityDocumentIdentifierParser.InternalParse(did);
            var identityDocument = BuildDefaultDocument(did);
            return new KeyIdentityDocumentBuilder(identityDocument);
        }
    }
}
