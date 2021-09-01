using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.Builders;
using SimpleIdServer.Saml.Extensions;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Sp
{
    [Route(Saml.Constants.RouteNames.Metadata)]
    public class MetadataController : Controller
    {
        private readonly AuthenticationOptions _authenticationOptions;
        private readonly IAuthenticationHandlerProvider _authenticationHandlerProvider;

        public MetadataController(
            IOptions<AuthenticationOptions> authenticationOptions,
            IAuthenticationHandlerProvider authenticationHandlerProvider)
        {
            _authenticationOptions = authenticationOptions.Value;
            _authenticationHandlerProvider = authenticationHandlerProvider;
        }

        public async Task<IActionResult> Get()
        {
            var scheme = _authenticationOptions.Schemes.FirstOrDefault(s => s.HandlerType == typeof(SamlSpHandler));
            var schemeName = SamlSpDefaults.AuthenticationScheme;
            if (scheme != null)
            {
                schemeName = scheme.Name;
            }

            var handler = await _authenticationHandlerProvider.GetHandlerAsync(HttpContext, schemeName);
            var samlHandler = handler as SamlSpHandler;
            var options = samlHandler.SamlSpOptions;
            var callbackPath = options.CallbackPath.Value;
            var result = EntityDescriptorBuilder.Instance(options.SPId)
                .AddSpSSODescriptor(cb =>
                {
                    cb.SetAuthnRequestsSigned(options.AuthnRequestSigned);
                    cb.SetWantAssertionsSigned(options.WantAssertionSigned);
                    cb.AddAssertionConsumerService(Constants.Bindings.HttpRedirect, $"{options.BaseUrl}{callbackPath}");
                    if (options.SigningCertificate != null)
                    {
                        cb.AddSigningKey(options.SigningCertificate);
                    }
                }).Build();
            return new ContentResult
            {
                Content = result.SerializeToXmlElement().OuterXml,
                ContentType = "application/xml",
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
