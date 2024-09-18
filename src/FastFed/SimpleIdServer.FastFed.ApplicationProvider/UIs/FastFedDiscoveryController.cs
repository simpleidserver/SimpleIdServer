// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.ApplicationProvider.Services;
using SimpleIdServer.FastFed.ApplicationProvider.UIs.ViewModels;
using SimpleIdServer.FastFed.Client;
using SimpleIdServer.FastFed.Resolvers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs
{
    // [Authorize("Authenticated")]
    public class FastFedDiscoveryController : Controller
    {
        private readonly IFastFedService _fastFedService;
        private readonly IFastFedClientFactory _fastFedClientFactory;
        private readonly IIssuerResolver _issuerResolver;

        public FastFedDiscoveryController(IFastFedService fastFedService, IFastFedClientFactory fastFedClientFactory, IIssuerResolver issuerResolver)
        {
            _fastFedService = fastFedService;
            _fastFedClientFactory = fastFedClientFactory;
            _issuerResolver = issuerResolver;
        }

        public IActionResult Index()
        {
            return View(new DiscoverProvidersViewModel());
        }

        [HttpPost]
        [RequireAntiforgeryToken]
        public async Task<IActionResult> Index(DiscoverProvidersViewModel viewModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(viewModel.Action == DiscoverProviderActions.CONFIRMPROVIDER)
            {
                var issuer = _issuerResolver.Get();
                var validationResult = await _fastFedService.StartWhitelist(issuer, viewModel.Href, cancellationToken);
                if (validationResult.HasError)
                {
                    foreach (var errorDescription in validationResult.ErrorDescriptions)
                        ModelState.AddModelError(validationResult.ErrorCode, errorDescription);

                    return View(viewModel);
                }

                var builder = new UriBuilder(validationResult.Result.fastFedHandshakeStartUri);
                builder.Query = validationResult.Result.request.ToQueryParameters();
                return Redirect(builder.Uri.ToString());
            }

            if(viewModel.Action == DiscoverProviderActions.CONFIRMEMAIL || viewModel.Action == DiscoverProviderActions.SELECTPROVIDER)
            {
                var resolutionResult = await _fastFedService.ResolveProviders(viewModel.Email, cancellationToken);
                if (resolutionResult.HasError)
                {
                    foreach(var errorDescription in resolutionResult.ErrorDescriptions)
                        ModelState.AddModelError(resolutionResult.ErrorCode, errorDescription);

                    return View(viewModel);
                }

                viewModel.WebfingerResult = resolutionResult.Result;
            }

            if (viewModel.Action == DiscoverProviderActions.SELECTPROVIDER)
            {
                var client = _fastFedClientFactory.Build();
                var providerMetadata = await client.GetProviderMetadata(viewModel.Href, false, cancellationToken);
                viewModel.ProviderMetadata = providerMetadata;
            }

            return View(viewModel);
        }
    }
}
