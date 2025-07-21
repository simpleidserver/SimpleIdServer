// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.VerifiablePresentation;
using SimpleIdServer.IdServer.VerifiablePresentation.Migrations;

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
            idServerBuilder.Services.AddTransient<IDataSeeder, InitVpDataseeder>();
            idServerBuilder.Services.AddTransient<IDataSeeder, UpdateTargetsVpWorkflowsDataseeder>();
            idServerBuilder.Services.AddTransient<IDataSeeder, UpdateVpTranslationsDataseeder>();
            idServerBuilder.AddRoute("getPresentationDefinition", SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.PresentationDefinitions + "/{id}", new { controller = "PresentationDefinitions", action = "Get" });
            idServerBuilder.AddRoute("vpAuthorizeCallback", SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.VpAuthorizeCallback, new { controller = "VpAuthorization", action = "Callback" });
            idServerBuilder.AddRoute("vpQrCode", SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.VpAuthorizeQrCode + "/{id}", new { controller = "VpAuthorization", action = "GetQRCode" });
            idServerBuilder.AddRoute("vpRegisterStatus", SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.VpRegisterStatus + "/{id}", new { controller = "VpRegister", action = "Status" });
            idServerBuilder.AddRoute("vpEndRegister", SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.VpEndRegister, new { controller = "VpRegister", action = "EndRegister" });
            idServerBuilder.AddRoute("vpAuthorizePost", SimpleIdServer.IdServer.VerifiablePresentation.Constants.Endpoints.VpAuthorizePost, new { controller = "VpAuthorize", action = "Post" });
            return idServerBuilder;
        }
    }
}
