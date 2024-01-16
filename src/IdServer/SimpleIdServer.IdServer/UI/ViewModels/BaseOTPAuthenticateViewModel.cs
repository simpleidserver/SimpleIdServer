// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class BaseOTPAuthenticateViewModel : BaseAuthenticateViewModel
    {
        public string Action { get; set; }
        public long? OTPCode { get; set; }
        public int? TOTPStep { get; set; }

        public override void CheckRequiredFields(ModelStateDictionary modelStateDictionary)
        {
            if (string.IsNullOrWhiteSpace(ReturnUrl))
                modelStateDictionary.AddModelError("missing_return_url", "missing_return_url");

            if (string.IsNullOrWhiteSpace(Login))
                modelStateDictionary.AddModelError("missing_login", "missing_login");
        }


        public void CheckConfirmationCode(ModelStateDictionary modelStateDictionary)
        {
            if (OTPCode == null)
                modelStateDictionary.AddModelError("missing_confirmationcode", "missing_confirmationcode");
        }
    }
}
