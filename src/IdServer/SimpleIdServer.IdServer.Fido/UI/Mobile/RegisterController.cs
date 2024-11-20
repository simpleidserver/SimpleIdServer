﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Fido.UI.Mobile
{
    [Area(Constants.MobileAMR)]
    public class RegisterController : BaseRegisterController<RegisterMobileViewModel>
    {
        private readonly IConfiguration _configuration;

        public RegisterController(
            IOptions<IdServerHostOptions> options, 
            IDistributedCache distributedCache, 
            IUserRepository userRepository, 
            IConfiguration configuration,
            ITokenRepository tokenRepository,
            ITransactionBuilder transactionBuilder,
            IJwtBuilder jwtBuilder) : base(options, distributedCache, userRepository, tokenRepository, transactionBuilder, jwtBuilder)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] string prefix)
        {
            var viewModel = new RegisterMobileViewModel();
            var fidoOptions = GetOptions();
            var isAuthenticated = User.Identity.IsAuthenticated;
            var userRegistrationProgress = await GetRegistrationProgress();
            if (userRegistrationProgress == null && !isAuthenticated)
            {
                viewModel.IsNotAllowed = true;
                return View(viewModel);
            }

            var login = string.Empty;
            if (!isAuthenticated)
            {
                userRegistrationProgress = await GetRegistrationProgress();
                login = userRegistrationProgress.User?.Name;
            }
            else login = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            if (!string.IsNullOrWhiteSpace(prefix))
                prefix = $"{prefix}/";
            viewModel.Login = login;
            viewModel.BeginRegisterUrl = $"{issuer}/{prefix}{Constants.EndPoints.BeginQRCodeRegister}";
            viewModel.RegisterStatusUrl = $"{issuer}/{prefix}{Constants.EndPoints.RegisterStatus}";
            viewModel.IsDeveloperModeEnabled = fidoOptions.IsDeveloperModeEnabled;
            viewModel.Amr = userRegistrationProgress?.Amr;
            viewModel.Steps = userRegistrationProgress?.Steps;
            viewModel.RedirectUrl = userRegistrationProgress?.RedirectUrl;
            return View(viewModel);
        }

        protected override void EnrichUser(User user, RegisterMobileViewModel viewModel)
        {
        }

        private MobileOptions GetOptions()
        {
            var section = _configuration.GetSection(typeof(MobileOptions).Name);
            return section.Get<MobileOptions>();
        }
    }
}
