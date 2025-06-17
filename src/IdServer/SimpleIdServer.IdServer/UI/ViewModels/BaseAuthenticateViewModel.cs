// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public abstract class BaseAuthenticateViewModel : ISidStepViewModel
{
    public string ReturnUrl { get; set; }
    public string AuthUrl { get; set; }
    public string Login { get; set; }
    public string ClientName { get; set; }
    public string LogoUri { get; set; }
    public string TosUri { get; set; }
    public string PolicyUri { get; set; }
    public bool RememberLogin { get; set; }
    public string Realm { get; set; }
    public bool IsAuthInProgress { get; set; } = false;
    public ICollection<ExternalIdProvider> ExternalIdsProviders { get; set; } = new List<ExternalIdProvider>();
    public string StepId { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentLink { get; set; }
    public string CaptchaValue { get; set; }
    public string CaptchaType { get; set; }

    public abstract List<string> Validate();
}

public record AcrAuthInfo
{
    public AcrAuthInfo(string userId, string login, string email, string currentAcr, string currentAmr, List<KeyValuePair<string, string>> claims, bool rememberLogin)
    {
        UserId = userId;
        Login = login;
        Email = email;
        CurrentAcr = currentAcr;
        CurrentAmr = currentAmr;
        Claims = claims;
        RememberLogin = rememberLogin;
    }

    public string UserId { get; private set; }
    public string Login { get; private set; }
    public string Email { get; private set; }
    public List<KeyValuePair<string, string>> Claims { get; set; }
    public string CurrentAcr { get; set; }
    public string CurrentAmr { get; set; }
    public bool RememberLogin { get; set; }
}
