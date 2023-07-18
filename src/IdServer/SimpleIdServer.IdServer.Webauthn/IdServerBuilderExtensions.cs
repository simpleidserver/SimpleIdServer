// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer.Webauthn
{
    public static class IdServerBuilderExtensions
    {
        public static IdServerBuilder AddWebauthnAuthentication(this IdServerBuilder idServerBuilder, Action<Fido2Configuration> fidoAction = null)
        {
            if (fidoAction == null) idServerBuilder.Services.AddFido2(o => { });
            else idServerBuilder.Services.AddFido2(fidoAction);
            idServerBuilder.Services.AddTransient<IAuthenticationMethodService, WebauthnAuthenticationService>();
            return idServerBuilder;
        }
    }
}
