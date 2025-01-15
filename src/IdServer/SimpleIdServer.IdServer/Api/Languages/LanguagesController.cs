// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Languages;

public class LanguagesController : BaseController
{
    private readonly ILanguageRepository _languageRepository;

    public LanguagesController(
        ILanguageRepository languageRepository,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _languageRepository = languageRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var languages = await _languageRepository.GetAll(cancellationToken);
        return new OkObjectResult(languages);
    }
}
