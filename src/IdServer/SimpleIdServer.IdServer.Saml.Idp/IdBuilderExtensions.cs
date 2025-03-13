// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Saml.Idp;
using SimpleIdServer.IdServer.Saml.Idp.Apis;
using SimpleIdServer.IdServer.Saml.Idp.Factories;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdBuilderExtensions
{
    public static IdServerBuilder AddSamlIdp(this IdServerBuilder builder, Action<SamlIdpOptions> samlIdpConfigurationCallback = null)
    {
        var samlConfiguration = new SamlIdpOptions();
        if (samlIdpConfigurationCallback != null) builder.Services.Configure(samlIdpConfigurationCallback);
        else builder.Services.Configure<SamlIdpOptions>(c => { });
        builder.Services.AddTransient<ISaml2ConfigurationFactory, Saml2ConfigurationFactory>();
        builder.Services.AddTransient<ISaml2AuthResponseEnricher, Saml2AuthResponseEnricher>();
        builder.AddRoute("getSamlMetadataIdp", Constants.RouteNames.Metadata, new { controller = "SamlMetadata", action = "Get" });
        builder.AddRoute("ssoHttpRedirect", Constants.RouteNames.SingleSignOnHttpRedirect, new { controller = "SamlSSO", action = "LoginGet" });
        builder.AddRoute("ssoArtifact", Constants.RouteNames.SingleSignOnArtifact, new { controller = "SamlSSO", action = "LoginArtifact" });
        builder.AddRoute("ssoLogout", Constants.RouteNames.SingleSignLogout, new { controller = "SamlSSO", action = "Logout" });
        return builder;
    }
}
