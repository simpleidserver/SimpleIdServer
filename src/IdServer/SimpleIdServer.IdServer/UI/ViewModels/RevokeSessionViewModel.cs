// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public class RevokeSessionViewModel : ILayoutViewModel
{
    public RevokeSessionViewModel(
        string revokeSessionCallbackUrl, 
        JsonWebToken idTokenHint, 
        IEnumerable<string> frontChannelLogouts,
        bool redirectToRevokeSessionUI,
        List<Language> languages)
    {
        RevokeSessionCallbackUrl = revokeSessionCallbackUrl;
        IdTokenHint = idTokenHint;
        FrontChannelLogouts = frontChannelLogouts;
        RedirectToRevokeSessionUI = redirectToRevokeSessionUI;
        Languages = languages;
    }

    public string RevokeSessionCallbackUrl { get; set; }
    public JsonWebToken IdTokenHint { get; set; }
    public IEnumerable<string> FrontChannelLogouts { get; set; }
    public bool RedirectToRevokeSessionUI { get; set; }
    public List<Language> Languages { get; set; }
}
