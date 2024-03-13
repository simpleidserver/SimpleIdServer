// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Fido.UI.ViewModels
{
    public class AuthenticateMobileViewModel : BaseAuthenticateViewModel
    {
        public AuthenticateMobileViewModel()
        {

        }

        public string SessionId { get; set; }
        public string BeginLoginUrl { get; set; } = null!;
        public string LoginStatusUrl { get; set; } = null!;
        public bool IsDeveloperModeEnabled { get; set; } = false;

        public override void Validate(ModelStateDictionary modelStateDictionary)
        {
            if (string.IsNullOrWhiteSpace(Login))
                modelStateDictionary.AddModelError("missing_login", "missing_login");

            if (string.IsNullOrWhiteSpace(SessionId))
                modelStateDictionary.AddModelError("missing_session_id", "missing_session_id");
        }
    }
}
