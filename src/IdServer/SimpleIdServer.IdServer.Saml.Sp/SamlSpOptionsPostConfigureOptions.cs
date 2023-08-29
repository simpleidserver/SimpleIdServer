// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;

namespace SimpleIdServer.IdServer.Saml.Sp;

public class SamlSpOptionsPostConfigureOptions : IPostConfigureOptions<SamlSpOptions>
{
    public void PostConfigure(string? name, SamlSpOptions options)
    {
        if(options.Backchannel == null) options.Backchannel = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler());
    }
}
