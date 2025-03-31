// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Negotiate;
using SimpleIdServer.IdServer.UI.AuthProviders;

namespace IdserverWindowsauth;

public class NegotiateOptionsLite : IDynamicAuthenticationOptions<NegotiateOptions>
{
    public NegotiateOptions Convert()
    {
        return new NegotiateOptions();
    }
}
