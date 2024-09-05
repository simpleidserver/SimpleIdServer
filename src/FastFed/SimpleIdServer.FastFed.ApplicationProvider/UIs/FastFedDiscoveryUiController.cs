// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.FastFed.ApplicationProvider.Services;
using SimpleIdServer.FastFed.ApplicationProvider.UIs.ViewModels;
using SimpleIdServer.Webfinger.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs
{
    [Authorize("Authenticated")]
    public class FastFedDiscoveryUiController : Controller
    {
        private readonly IWebfingerClientFactory _clientFactory;
        private readonly IFastFedService _fastFedService;

        public FastFedDiscoveryUiController(IWebfingerClientFactory clientFactory, IFastFedService fastFedService)
        {
            _clientFactory = clientFactory;
            _fastFedService = fastFedService;
        }

        public async Task<IActionResult> Discover(string url = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO : CHECK THE PERMISSION - Use must be an administrator.
            var viewModel = new DiscoverIdentityProviderViewModel();
            if(!string.IsNullOrWhiteSpace(url))
            {
                var client = _clientFactory.Build();
                viewModel.WebfingerResult = await client.Get(url, cancellationToken);
            }

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
