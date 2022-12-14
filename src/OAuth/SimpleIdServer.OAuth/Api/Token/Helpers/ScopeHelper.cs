// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Api.Token.Helpers
{
    public static class ScopeHelper
    {
        public static IEnumerable<string> Validate(string scopeParameter, IEnumerable<string> allowedScopes)
        {
            var scopes = scopeParameter.ToScopes();
            return Validate(scopes, allowedScopes);
        }

        public static IEnumerable<string> Validate(IEnumerable<string> scopes, IEnumerable<string> allowedScopes)
        {
            var duplicates = scopes.GroupBy(p => p)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicates.Any()) throw new OAuthException(ErrorCodes.INVALID_SCOPE, string.Format(ErrorMessages.DUPLICATE_SCOPES, duplicates.Join()));

            var invalidScopes = scopes
                .Where(s => !allowedScopes.Contains(s))
                .ToList();
            if (invalidScopes.Any()) throw new OAuthException(ErrorCodes.INVALID_SCOPE, string.Format(ErrorMessages.INVALID_SCOPES, invalidScopes.Join()));

            return scopes;
        }
    }
}
