// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.UI.AuthProviders
{
    public static class ExternalProviderHelper
    {
        public static IEnumerable<AuthenticationScheme> GetExternalAuthenticationSchemes(IEnumerable<AuthenticationScheme> authenticationSchemes) => authenticationSchemes.Where(s => !string.IsNullOrWhiteSpace(s.DisplayName) && Constants.DefaultOIDCAuthenticationScheme != s.Name);
    }
}
