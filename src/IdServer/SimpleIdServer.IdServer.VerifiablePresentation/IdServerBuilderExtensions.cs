// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api.Authorization;
using SimpleIdServer.IdServer.VerifiablePresentation;
using SimpleIdServer.IdServer.VerifiablePresentation.Apis.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdServerBuilderExtensions
    {
        /// <summary>
        /// Add vp authentication method.
        /// The amr is vp.
        /// </summary>
        /// <param name="idServerBuilder"></param>
        /// <returns></returns>
        public static IdServerBuilder AddVpAuthentication(this IdServerBuilder idServerBuilder)
        {
            idServerBuilder.Services.AddTransient<IAuthenticationMethodService, VpAuthenticationMethodService>();
            idServerBuilder.Services.AddDid();
            idServerBuilder.Services.AddTransient<IWorkflowLayoutService, VpRegisterWorkflowLayout>();
            return idServerBuilder;
        }
    }
}
