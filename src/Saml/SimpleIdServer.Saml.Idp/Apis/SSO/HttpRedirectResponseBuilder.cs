using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Xsd;
using System;

namespace SimpleIdServer.Saml.Idp.Apis.SSO
{
    public class HttpRedirectResponseBuilder : IResponseBuilder
    {
        private readonly SamlIdpOptions _options;

        public HttpRedirectResponseBuilder(IOptions<SamlIdpOptions> options)
        {
            _options = options.Value;
        }

        public string Binding => Constants.Bindings.HttpRedirect;

        public SingleSignOnResult Build(string location, ResponseType response, string relayState)
        {
            var uri = new Uri(location);
            var redirectionUrl = MessageEncodingBuilder.EncodeHTTPBindingResponse(uri, response.SerializeToXmlElement(), relayState, _options.SigningCertificate, _options.SignatureAlg);
            return SingleSignOnResult.Redirect(redirectionUrl);
        }
    }
}
