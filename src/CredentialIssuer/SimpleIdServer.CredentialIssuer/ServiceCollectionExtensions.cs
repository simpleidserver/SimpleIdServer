// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.CredentialIssuer;
using SimpleIdServer.CredentialIssuer.Api.Credential.Validators;
using SimpleIdServer.CredentialIssuer.Api.CredentialOffer.Commands;
using SimpleIdServer.CredentialIssuer.Api.CredentialOffer.Queries;
using SimpleIdServer.CredentialIssuer.CredentialFormats;
using SimpleIdServer.CredentialIssuer.Factories;
using SimpleIdServer.CredentialIssuer.Services;
using SimpleIdServer.IdServer.CredentialIssuer;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static CredentialIssuerServer AddCredentialIssuer(this IServiceCollection services, Action<CredentialIssuerOptions> callback = null)
    {
        if (callback != null) services.Configure(callback);
        else services.Configure<CredentialIssuerOptions>(c => { });
        services.AddTransient<IKeyProofTypeValidator, JwtKeyProofTypeValidator>();
        services.AddTransient<ICredentialConfigurationSerializer, CredentialConfigurationSerializer>();
        services.AddTransient<ICredentialFormatter, JwtVcJsonFormatter>();
        services.AddTransient<ICredentialFormatter, JwtVcJsonLdFormatter>();
        services.AddTransient<ICredentialFormatter, LdpVcFormatter>();
        services.AddTransient<IPreAuthorizedCodeService, PreAuthorizedCodeService>();
        services.AddTransient<IHttpClientFactory, HttpClientFactory>();
        services.AddTransient<ICreateCredentialOfferCommandHandler, CreateCredentialOfferCommandHandler>();
        services.AddTransient<IGetCredentialOfferQueryHandler, GetCredentialOfferQueryHandler>();
        return new CredentialIssuerServer(services);
    }
}
