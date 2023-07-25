// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.Domains;
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
        public abstract void CheckRequiredFields(User user, ModelStateDictionary modelStateDictionary);
    }

    public record AmrAuthInfo
    {
        public AmrAuthInfo(string userId, IEnumerable<string> allAmr, string currentAmr)
        {
            UserId = userId;
            AllAmr = allAmr;
            CurrentAmr = currentAmr;
        }

        public string UserId { get; private set; }
        public IEnumerable<string> AllAmr { get; set; }
        public string CurrentAmr { get; private set; }
    }
}
