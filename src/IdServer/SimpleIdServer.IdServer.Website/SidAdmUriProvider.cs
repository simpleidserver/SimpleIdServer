// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace SimpleIdServer.IdServer.Website;

public class SidAdmUriProvider : UriProvider
{
    private readonly IdServerWebsiteOptions _options;

    public SidAdmUriProvider(IHttpContextAccessor httpContextAccessor, IOptions<IdServerWebsiteOptions> options) : base(httpContextAccessor)
    {
        _options = options.Value;
    }

    public override string GetAbsoluteUriWithVirtualPath()
        => _options.Issuer;
}
