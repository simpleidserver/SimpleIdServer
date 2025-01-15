using FormBuilder;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using FormBuilder.UIs;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Fido.UI.Mobile
{
    [Area(Constants.MobileAMR)]
    public class RegisterController : BaseRegisterController<RegisterMobileViewModel>
    {
        private readonly IConfiguration _configuration;

        public RegisterController(
            IOptions<IdServerHostOptions> options, 
            IOptions<FormBuilderOptions> formOptions,
            IDistributedCache distributedCache, 
            IUserRepository userRepository, 
            IConfiguration configuration,
            ITokenRepository tokenRepository,
            ITransactionBuilder transactionBuilder,
            IJwtBuilder jwtBuilder,
            IAntiforgery antiforgery,
            IFormStore formStore,
            IWorkflowStore workflowStore,
            ILanguageRepository languageRepository) : base(options, formOptions, distributedCache, userRepository, tokenRepository, transactionBuilder, jwtBuilder, antiforgery, formStore, workflowStore, languageRepository)
        {
            _configuration = configuration;
        }

        protected override string Amr => Constants.MobileAMR;

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            var fidoOptions = GetOptions();
            var isAuthenticated = User.Identity.IsAuthenticated;
            var userRegistrationProgress = await GetRegistrationProgress();
            if (userRegistrationProgress == null && !isAuthenticated)
            {
                var res = new SidWorkflowViewModel();
                res.SetErrorMessage(Global.NotAllowedToRegister);
                return View(res);
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
            var viewModel = new RegisterMobileViewModel
            {
                Login = login,
                BeginRegisterUrl = $"{issuer}/{prefix}{Constants.EndPoints.BeginQRCodeRegister}",
                RegisterStatusUrl = $"{issuer}/{prefix}{Constants.EndPoints.RegisterStatus}",
                IsDeveloperModeEnabled = fidoOptions.IsDeveloperModeEnabled,
                RedirectUrl = userRegistrationProgress?.RedirectUrl
            };
            var result = await BuildViewModel(userRegistrationProgress, viewModel, prefix,  cancellationToken);
            result.SetInput(viewModel);
            return View(result);
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
