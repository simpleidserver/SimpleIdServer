// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Models;
using System;
using System.Linq;

namespace SimpleIdServer.Did.Key
{
    public class IdentityDocumentIdentifierParser : IIdentityDocumentIdentifierParser
    {
        public string Type => Constants.Type;

        public IdentityDocumentIdentifier Parse(string id) => InternalParse(id);

        internal static IdentityDocumentIdentifier InternalParse(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(Constants.Namespace));
            if (!id.StartsWith(Constants.Namespace)) throw new ArgumentOutOfRangeException($"the DID Identifier must start by {Constants.Namespace}");
            var splitted = id.Split(':');
            if (splitted.Length != 3) throw new ArgumentException($"the DID Identifier must have the following format {Constants.Namespace}:<mb-value>");
            var multibaseValue = splitted.Last();
            if (!multibaseValue.StartsWith("z")) throw new InvalidOperationException("The Multi Base Value must start with z");
            var result = new IdentityDocumentIdentifier(id, null);
            result.PublicKey = multibaseValue;
            return result;
        }
    }
}
