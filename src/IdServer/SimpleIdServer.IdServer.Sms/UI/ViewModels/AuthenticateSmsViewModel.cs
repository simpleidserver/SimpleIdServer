// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Sms.UI.ViewModels
{
    public class AuthenticateSmsViewModel : BaseAuthenticateViewModel
    {
        public AuthenticateSmsViewModel() { }

        public AuthenticateSmsViewModel(string returnUrl, string realm, string phoneNumber, string clientName, string logoUri, string tosUri, string policyUri, bool isPhoneNumberMissing, bool isAuthInProgress)
        {
            ReturnUrl = returnUrl;
            Realm = realm;
            PhoneNumber = phoneNumber;
            ClientName = clientName;
            LogoUri = logoUri;
            TosUri = tosUri;
            PolicyUri = policyUri;
            IsPhoneNumberMissing = isPhoneNumberMissing;
            IsAuthInProgress = isAuthInProgress;
        }

        public string PhoneNumber { get; set; }
        public string Action { get; set; }
        public long? OTPCode { get; set; }
        public bool IsPhoneNumberMissing { get; set; } = false;
        public bool IsAuthInProgress { get; set; } = false;

        public void CheckPhoneNumber(ModelStateDictionary modelStateDictionary, User user)
        {
            UserClaim phoneNumberClaim;
            if (user != null && ((phoneNumberClaim = user.OAuthUserClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.PhoneNumber)) != null) && phoneNumberClaim.Value != PhoneNumber)
                modelStateDictionary.AddModelError("bad_phonenumber", "bad_phonenumber");
        }

        public void CheckRequiredFields(ModelStateDictionary modelStateDictionary)
        {
            if (string.IsNullOrWhiteSpace(ReturnUrl))
                modelStateDictionary.AddModelError("missing_return_url", "missing_return_url");

            if (string.IsNullOrWhiteSpace(PhoneNumber))
                modelStateDictionary.AddModelError("missing_phonenumber", "missing_phonenumber");
        }

        public void CheckConfirmationCode(ModelStateDictionary modelStateDictionary)
        {
            if (OTPCode == null)
                modelStateDictionary.AddModelError("missing_confirmationcode", "missing_confirmationcode");
        }
    }
}
