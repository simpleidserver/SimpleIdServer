// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.VerifiablePresentation.UI.ViewModels;

namespace SimpleIdServer.IdServer.VerifiablePresentation.UI;

[Area(Constants.AMR)]
public class RegisterController : BaseRegisterController<VerifiablePresentationRegisterViewModel>
{
    private readonly IPresentationDefinitionStore _presentationDefinitionStore;

    public RegisterController(
        IPresentationDefinitionStore presentationDefinitionStore,
        IOptions<IdServerHostOptions> options, 
        IDistributedCache distributedCache, 
        IUserRepository userRepository, 
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder) : base(options, distributedCache, userRepository, tokenRepository, jwtBuilder)
    {
        _presentationDefinitionStore = presentationDefinitionStore;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        var presentationDefinitions = await _presentationDefinitionStore.Query()
            .Include(p => p.InputDescriptors)
            .AsNoTracking()
            .Where(p => p.RealmName == prefix)
            .ToListAsync(cancellationToken);
        var verifiablePresentations = presentationDefinitions.Select(d => new VerifiablePresentationViewModel
        {
            Id = d.Id,
            Name = d.Name,
            VcNames = d.InputDescriptors.Select(id => id.Name).ToList()
        });
        return View(verifiablePresentations);
    }

    protected override void EnrichUser(User user, VerifiablePresentationRegisterViewModel viewModel)
    {
        throw new NotImplementedException();
    }
}