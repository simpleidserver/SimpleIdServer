// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.CredentialIssuer.UI.ViewModels;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.UI;

[Authorize("Authenticated")]
public class CredentialsController : Controller
{
    private readonly ICredentialConfigurationStore _credentialConfigurationStore;
    private readonly ICredentialStore _credentialStore;

    public CredentialsController(
        ICredentialConfigurationStore credentialConfigurationStore,
        ICredentialStore credentialStore)
    {
        _credentialConfigurationStore = credentialConfigurationStore;
        _credentialStore = credentialStore;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // list of credentials...
        return View(new CredentialsViewModel
        {

        });
    }

    public async Task<IActionResult> Share(string configurationId)
    {
        // generate the QR code.
        return null;
    }
}
