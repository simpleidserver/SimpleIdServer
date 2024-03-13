// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public abstract class BaseAuthenticateViewModel
    {
        public string ReturnUrl { get; set; }
        public string Login { get; set; }
        public string ClientName { get; set; }
        public string LogoUri { get; set; }
        public string TosUri { get; set; }
        public string PolicyUri { get; set; }
        public bool RememberLogin { get; set; }
        public string Realm { get; set; }
        public bool IsLoginMissing { get; set; }
        public bool IsAuthInProgress { get; set; } = false;
        public ICollection<ExternalIdProvider> ExternalIdsProviders { get; set; } = new List<ExternalIdProvider>();
        public AmrAuthInfo AmrAuthInfo { get; set; } = null;
        public abstract void Validate(ModelStateDictionary modelStateDictionary);
    }

    public record AmrAuthInfo
    {
        public AmrAuthInfo(string userId, string login, string email, List<KeyValuePair<string, string>> claims, IEnumerable<string> allAmr, string currentAmr)
        {
            UserId = userId;
            Login = login;
            Email = email;
            Claims = claims;
            AllAmr = allAmr;
            CurrentAmr = currentAmr;
        }

        public string UserId { get; private set; }
        public string Login { get; private set; }
        public string Email { get; private set; }
        public List<KeyValuePair<string, string>> Claims { get; set; }
        public IEnumerable<string> AllAmr { get; set; }
        public string CurrentAmr { get; private set; }
    }
}
