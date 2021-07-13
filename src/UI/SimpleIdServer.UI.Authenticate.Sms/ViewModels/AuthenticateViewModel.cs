// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SimpleIdServer.UI.Authenticate.Sms.ViewModels
{
    public class AuthenticateViewModel
    {
        public AuthenticateViewModel() { }

        public AuthenticateViewModel(string returnUrl, string phoneNumber, string clientName, string logoUri, string tosUri, string policyUri)
        {
            ReturnUrl = returnUrl;
            PhoneNumber = phoneNumber;
            ClientName = clientName;
            LogoUri = logoUri;
            TosUri = tosUri;
            PolicyUri = policyUri;
        }

        public string ReturnUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string ClientName { get; set; }
        public string LogoUri { get; set; }
        public string TosUri { get; set; }
        public string PolicyUri { get; set; }
        public string Action { get; set; }
        public long? OTPCode { get; set; }
        public bool RememberLogin { get; set; }

        public void CheckRequiredFields(ModelStateDictionary modelStateDictionary)
        {
            if (string.IsNullOrWhiteSpace(ReturnUrl))
            {
                modelStateDictionary.AddModelError("missing_return_url", "missing_return_url");
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                modelStateDictionary.AddModelError("missing_phonenumber", "missing_phonenumber");
            }
        }

        public void CheckConfirmationCode(ModelStateDictionary modelStateDictionary)
        {
            if (OTPCode == null)
            {
                modelStateDictionary.AddModelError("missing_confirmationcode", "missing_confirmationcode");
            }
        }
    }
}
