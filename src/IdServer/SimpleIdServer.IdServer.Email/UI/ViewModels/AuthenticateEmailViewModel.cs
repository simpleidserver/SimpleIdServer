// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Email.UI.ViewModels
{
    public class AuthenticateEmailViewModel : BaseAuthenticateViewModel
    {
        public AuthenticateEmailViewModel() { }

        public AuthenticateEmailViewModel(string returnUrl, string realm, string email, string clientName, string logoUri, string tosUri, string policyUri, bool isEmailMissing, bool isAuthInProgress)
        {
            ReturnUrl = returnUrl;
            Realm = realm;
            Email = email;
            ClientName = clientName;
            LogoUri = logoUri;
            TosUri = tosUri;
            PolicyUri = policyUri;
            IsEmailMissing = isEmailMissing;
            IsAuthInProgress = isAuthInProgress;
        }

        public string Email { get; set; }
        public string Action { get; set; }
        public long? OTPCode { get; set; }
        public bool IsEmailMissing { get; set; } = false;
        public bool IsAuthInProgress { get; set; } = false;

        public void CheckEmail(ModelStateDictionary modelStateDictionary, User user)
        {
            if(user != null && user.Email != Email)
                modelStateDictionary.AddModelError("bad_email", "bad_email");
        }

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
