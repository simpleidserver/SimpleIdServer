// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.IdServer.Api.Authorization.Validators;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.Api.Authorization;
using SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Formats;
using SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Validators;
using SimpleIdServer.IdServer.CredentialIssuer.Api.Token;
using SimpleIdServer.IdServer.CredentialIssuer.Extractors;
using SimpleIdServer.IdServer.CredentialIssuer.Helpers;
using SimpleIdServer.IdServer.CredentialIssuer.Parsers;
using SimpleIdServer.IdServer.CredentialIssuer.Validators;
using SimpleIdServer.IdServer.Helpers;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdServerBuilderExtensions
    {
        public static IdServerBuilder AddCredentialIssuer(this IdServerBuilder idServerBuilder, Action<CredentialIssuerOptions> callback = null)
        {
            if (callback == null) idServerBuilder.Services.Configure<CredentialIssuerOptions>(o => { });
            else idServerBuilder.Services.Configure(callback);
            idServerBuilder.Services.RemoveAll<IAuthorizationRequestValidator>();
            idServerBuilder.Services.RemoveAll<IPreAuthorizedCodeValidator>();
            idServerBuilder.Services.RemoveAll<IUserHelper>();
            idServerBuilder.Services.AddTransient<ICredentialRequestParser, SignedJWTCredentialRequestParser>();
            idServerBuilder.Services.AddTransient<IAuthorizationDetailsValidator, AuthorizationDetailsValidator>();
            idServerBuilder.Services.AddTransient<ICredentialAuthorizationDetailsValidator, SignedJWTAuthorizationDetailsValidator>();
            idServerBuilder.Services.AddTransient<IAuthorizationRequestValidator, CredIssuerOAuthAuthorizationRequestValidator>();
            idServerBuilder.Services.AddTransient<IPreAuthorizedCodeValidator, CredIssuerPreAuthorizedCodeValidator>();
            idServerBuilder.Services.AddTransient<ICredIssuerTokenHelper, CredIssuerTokenHelper>();
            idServerBuilder.Services.AddTransient<IUserHelper, CredIssuerUserHelper>();
            idServerBuilder.Services.AddTransient<IProofValidator, JwtProofValidator>();
            idServerBuilder.Services.AddTransient<ICredentialTemplateClaimsExtractor, CredentialTemplateClaimsExtractor>();
            idServerBuilder.Services.AddTransient<ICredentialFormat, JwtVcCredentialFormat>();
            var preAuthCodeHandler = idServerBuilder.Services.First(s => s.ImplementationType == typeof(PreAuthorizedCodeHandler));
            var authCodeHandler = idServerBuilder.Services.First(s => s.ImplementationType == typeof(AuthorizationCodeHandler));
            idServerBuilder.Services.Remove(preAuthCodeHandler);
            idServerBuilder.Services.Remove(authCodeHandler);
            idServerBuilder.Services.AddTransient<IGrantTypeHandler, CredIssuerPreAuthorizedCodeHandler>();
            idServerBuilder.Services.AddTransient<IGrantTypeHandler, CredIssuerAuthorizationCodeHandler>();
            return idServerBuilder;
        }
    }
}
