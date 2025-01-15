// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI;

public class ErrorsController : Controller
{
    private readonly ILanguageRepository _languageRepository;

    public ErrorsController(ILanguageRepository languageRepository)
    {
        _languageRepository = languageRepository;   
    }

    public async Task<IActionResult> Index(string code, string message, CancellationToken cancellationToken)
    {
        var languages = await _languageRepository.GetAll(cancellationToken);
        return View(new ErrorViewModel(code, message, languages));
    }

    public async Task<IActionResult> Unexpected(CancellationToken cancellationToken)
    {
        var languages = await _languageRepository.GetAll(cancellationToken);
        return View(new UnexpectedErrorViewModel { Languages = languages });
    }
}
