// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Email.UI.ViewModels
{
    public class AuthenticateEmailViewModel : BaseAuthenticateViewModel
    {
        public AuthenticateEmailViewModel() { }

        public AuthenticateEmailViewModel(string returnUrl, string realm, string email, string clientName, string logoUri, string tosUri, string policyUri)
        {
            ReturnUrl = returnUrl;
            Realm = realm;
            Email = email;
            ClientName = clientName;
            LogoUri = logoUri;
            TosUri = tosUri;
            PolicyUri = policyUri;
        }

        public string Email { get; set; }
        public string Action { get; set; }
        public long? OTPCode { get; set; }

        public void CheckRequiredFields(ModelStateDictionary modelStateDictionary)
        {
            if (string.IsNullOrWhiteSpace(ReturnUrl))
                modelStateDictionary.AddModelError("missing_return_url", "missing_return_url");

            if (string.IsNullOrWhiteSpace(Email))
                modelStateDictionary.AddModelError("missing_phonenumber", "missing_email");
        }

        public void CheckConfirmationCode(ModelStateDictionary modelStateDictionary)
        {
            if (OTPCode == null)
                modelStateDictionary.AddModelError("missing_confirmationcode", "missing_confirmationcode");
        }
    }
}
