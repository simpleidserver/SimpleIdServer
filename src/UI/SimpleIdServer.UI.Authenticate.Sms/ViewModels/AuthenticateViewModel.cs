// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SimpleIdServer.UI.Authenticate.Sms.ViewModels
{
    public class AuthenticateViewModel
    {
        public AuthenticateViewModel() { }

        public AuthenticateViewModel(string returnUrl, string phoneNumber)
        {
            ReturnUrl = returnUrl;
            PhoneNumber = phoneNumber;
        }

        public string ReturnUrl { get; set; }
        public string Action { get; set; }
        public string PhoneNumber { get; set; }
        public string ConfirmationCode { get; set; }
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
            if (string.IsNullOrWhiteSpace(ConfirmationCode))
            {
                modelStateDictionary.AddModelError("missing_confirmationcode", "missing_confirmationcode");
            }
        }
    }
}
