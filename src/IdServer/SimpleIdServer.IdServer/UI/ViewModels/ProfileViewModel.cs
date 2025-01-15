// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public class ProfileViewModel : ILayoutViewModel
{
    public string Name { get; set; }
    public bool HasOtpKey { get; set; }
    public string Picture { get; set; }
    public IEnumerable<ConsentViewModel> Consents { get; set; }
    public IEnumerable<PendingRequestViewModel> PendingRequests { get; set; }
    public IEnumerable<ExternalAuthProviderViewModel> Profiles { get; set; }
    public IEnumerable<AuthenticationMethodViewModel> AuthenticationMethods { get; set; }
    public IEnumerable<ExternalIdProvider> ExternalIdProviders { get; set; }
    public List<Language> Languages { get; set; }
}