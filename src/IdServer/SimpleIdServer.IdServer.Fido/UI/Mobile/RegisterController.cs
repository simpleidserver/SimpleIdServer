using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;

namespace SimpleIdServer.IdServer.Fido.UI.Mobile
{
    [Area(Constants.MobileAMR)]
    public class RegisterController : BaseController
    {
        private readonly IConfiguration _configuration;

        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index([FromRoute] string prefix)
        {
            var fidoOptions = GetOptions();
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            if (!string.IsNullOrWhiteSpace(prefix))
                prefix = $"{prefix}/";
            return View(new RegisterMobileViewModel
            {
                Login = string.Empty,
                BeginRegisterUrl = $"{issuer}/{prefix}{Constants.EndPoints.BeginQRCodeRegister}",
                RegisterStatusUrl = $"{issuer}/{prefix}{Constants.EndPoints.RegisterStatus}",
                IsDeveloperModeEnabled = fidoOptions.IsDeveloperModeEnabled
            });
        }

        private FidoOptions GetOptions()
        {
            var section = _configuration.GetSection(typeof(FidoOptions).Name);
            return section.Get<FidoOptions>();
        }
    }
}
