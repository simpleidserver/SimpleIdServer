// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer.Fido
{
    public static class IdServerBuilderExtensions
    {
        public static IdServerBuilder AddFidoAuthentication(this IdServerBuilder idServerBuilder, Action<FidoOptions> callbackFidoOptions = null, Action<Fido2Configuration> fidoAction = null)
        {
            if (callbackFidoOptions == null) idServerBuilder.Services.Configure<FidoOptions>((o) => { });
            else idServerBuilder.Services.Configure(callbackFidoOptions);
            if (fidoAction == null) idServerBuilder.Services.AddFido2(o => 
            {
                o.ServerDomain = "localhost";
                o.Origins = new HashSet<string> { "https://localhost:5001" };
            });
            else idServerBuilder.Services.AddFido2(fidoAction);
            idServerBuilder.Services.AddTransient<IAuthenticationMethodService, WebauthnAuthenticationService>();
            return idServerBuilder;
        }
    }
}
