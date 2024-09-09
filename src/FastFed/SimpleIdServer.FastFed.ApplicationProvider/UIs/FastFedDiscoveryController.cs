// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.ApplicationProvider.Services;
using SimpleIdServer.FastFed.ApplicationProvider.UIs.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs
{
    // [Authorize("Authenticated")]
    public class FastFedDiscoveryController : Controller
    {
        private readonly IFastFedService _fastFedService;

        public FastFedDiscoveryController(IFastFedService fastFedService)
        {
            _fastFedService = fastFedService;
        }

        public IActionResult Index()
        {
            return View(new DiscoverProvidersViewModel());
        }

        [HttpPost]
        [RequireAntiforgeryToken]
        public async Task<IActionResult> Index(DiscoverProvidersViewModel viewModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resolutionResult = await _fastFedService.ResolveProviders(viewModel.Email, cancellationToken);
            if (resolutionResult.HasError)
            {
                ModelState.AddModelError(resolutionResult.ErrorCode, resolutionResult.ErrorDescription);
                return View(viewModel);
            }

            viewModel.WebfingerResult = resolutionResult.Result;
            return View(viewModel);
        }

        public async Task<IActionResult> Select(string url, CancellationToken cancellationToken)
        {
            var validationResult = await _fastFedService.ValidateIdentityProviderMetadata(url, cancellationToken);
            if(validationResult.HasError)
            {
                return View(new SelectIdentityProviderViewModel(validationResult.ErrorCode, validationResult.ErrorDescription));
            }

            return View(new SelectIdentityProviderViewModel(url,  validationResult.Result.metadata, validationResult.Result.federation));
        }

        public async Task<IActionResult> Confirm(string url, CancellationToken cancellationToken)
        {
            var issuer = this.GetAbsoluteUriWithVirtualPath();
            var validationResult = await _fastFedService.StartWhitelist(issuer, url, cancellationToken);
            if (validationResult.HasError)
            {
                return View(new ConfirmIdentityProviderViewModel(validationResult.ErrorCode, validationResult.ErrorDescription));
            }

            var builder = new UriBuilder(validationResult.Result.fastFedHandshakeStartUri);
            builder.Query = validationResult.Result.request.ToQueryParameters();
            return Redirect(builder.Uri.ToString());
        }
    }
}
