// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;

namespace SimpleIdServer.IdServer.Saml.Sp;

public class SamlSpEvents : RemoteAuthenticationEvents
{
    public Func<RedirectContext<SamlSpOptions>, Task> OnRedirectToSsoEndpoint { get; set; } = context =>
    {
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    public virtual Task RedirectToSsoEndpoint(RedirectContext<SamlSpOptions> context) => OnRedirectToSsoEndpoint(context);
}
