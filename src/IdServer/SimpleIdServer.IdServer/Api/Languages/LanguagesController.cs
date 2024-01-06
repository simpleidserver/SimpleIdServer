// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Languages;

[AllowAnonymous]
public class LanguagesController : BaseController
{
    private readonly ITranslationRepository _translationRepository;
    private readonly ILanguageRepository _languageRepository;

    public LanguagesController(
        ITranslationRepository translationRepository,
        ILanguageRepository languageRepository,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _translationRepository = translationRepository;
        _languageRepository = languageRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var languages = await _languageRepository.GetAll(cancellationToken);
        foreach (var language in languages)
        {
            var descriptions = await _translationRepository.Query().Where(t => t.Key == language.TranslationKey).ToListAsync(cancellationToken);
            language.Descriptions = descriptions;
            language.Description = GetDescription(language);
        }

        return new OkObjectResult(languages);
    }

    private string GetDescription(Language language)
    {
        var description = language.Descriptions.SingleOrDefault(d => d.Key == language.TranslationKey && d.Language == Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
        return description?.Value;
    }
}
