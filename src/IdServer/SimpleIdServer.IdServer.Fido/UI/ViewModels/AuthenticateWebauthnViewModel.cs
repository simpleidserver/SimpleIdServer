// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Fido.UI.ViewModels
{
    public class AuthenticateWebauthnViewModel : BaseAuthenticateViewModel
    {
        public AuthenticateWebauthnViewModel()
        {

        }

        public string SessionId { get; set; }
        public string BeginLoginUrl { get; set; } = null!;
        public string EndLoginUrl { get; set; } = null!;

        public override void CheckRequiredFields(ModelStateDictionary modelStateDictionary)
        {
            if (string.IsNullOrWhiteSpace(ReturnUrl))
                modelStateDictionary.AddModelError("missing_return_url", "missing_return_url");

            if (string.IsNullOrWhiteSpace(Login))
                modelStateDictionary.AddModelError("missing_login", "missing_login");

            if (string.IsNullOrWhiteSpace(SessionId))
                modelStateDictionary.AddModelError("missing_session_id", "missing_session_id");
        }
    }
}
