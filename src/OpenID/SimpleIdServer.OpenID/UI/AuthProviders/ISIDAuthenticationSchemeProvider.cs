// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.UI.AuthProviders
{
    public interface ISIDAuthenticationSchemeProvider
    {
        Task<SIDAuthenticationScheme> GetSIDSchemeAsync(string name);
    }
}
