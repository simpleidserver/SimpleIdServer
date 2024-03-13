// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Sms.UI.ViewModels
{
    public class AuthenticateSmsViewModel : BaseOTPAuthenticateViewModel
    {
        public AuthenticateSmsViewModel() { }

        public override void SpecificValidate(ModelStateDictionary modelStateDictionary)
        {
        }
    }
}
