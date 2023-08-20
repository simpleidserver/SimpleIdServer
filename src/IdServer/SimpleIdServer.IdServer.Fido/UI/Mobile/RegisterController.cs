using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;

namespace SimpleIdServer.IdServer.Fido.UI.Mobile
{
    [Area(Constants.MobileAMR)]
    public class RegisterController : BaseController
    {
        private readonly FidoOptions _options;

        public RegisterController(IOptions<FidoOptions> options)
        {
            _options = options.Value;
        }

        [HttpGet]
        public IActionResult Index([FromRoute] string prefix)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            if (!string.IsNullOrWhiteSpace(prefix))
                prefix = $"{prefix}/";
            return View(new RegisterMobileViewModel
            {
                Login = string.Empty,
                BeginRegisterUrl = $"{issuer}/{prefix}{Constants.EndPoints.BeginQRCodeRegister}",
                RegisterStatusUrl = $"{issuer}/{prefix}{Constants.EndPoints.RegisterStatus}",
                IsDeveloperModeEnabled = _options.IsDeveloperModeEnabled
            });
        }
    }
}
