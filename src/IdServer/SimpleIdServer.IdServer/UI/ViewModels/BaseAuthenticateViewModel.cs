// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class BaseAuthenticateViewModel
    {
        public string ReturnUrl { get; set; }
        public string ClientName { get; set; }
        public string LogoUri { get; set; }
        public string TosUri { get; set; }
        public string PolicyUri { get; set; }
        public bool RememberLogin { get; set; }
        public string Realm { get; set; }
        public ICollection<ExternalIdProvider> ExternalIdsProviders { get; set; } = new List<ExternalIdProvider>();
        public AmrAuthInfo AmrAuthInfo { get; set; } = null;
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
