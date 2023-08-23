// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdBuilderExtensions
{
    public static IdServerBuilder AddSamlAuthentication(this IdServerBuilder builder, Action<Saml2Configuration> saml2ConfigurationCallback = null)
    {
        var samlConfiguration = new Saml2Configuration();
        if (saml2ConfigurationCallback != null) builder.Services.Configure(saml2ConfigurationCallback);
        else builder.Services.Configure<Saml2Configuration>(c => { });
        builder.Services.AddSaml2();
        return builder;
    }
}
