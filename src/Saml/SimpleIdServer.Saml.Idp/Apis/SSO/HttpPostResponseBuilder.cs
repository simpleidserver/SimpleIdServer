using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Xsd;

namespace SimpleIdServer.Saml.Idp.Apis.SSO
{
    public class HttpPostResponseBuilder : IResponseBuilder
    {
        public string Binding => Constants.Bindings.HttpPost;

        public SingleSignOnResult Build(string location, ResponseType response, string relayState)
        {
            var samlResponse = MessageEncodingBuilder.EncodeHTTPPostResponse(response.SerializeToXmlElement());
            var html = GetPostResultHtml(location, samlResponse, relayState);
            return SingleSignOnResult.Html(html);
        }

        private string GetPostResultHtml(string url, string samlResponse, string relayState)
        {
            var result =
$@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
    <title>SAML 2.0</title>
</head>
<body onload=""document.forms[0].submit()"">
    <noscript>
        <p>
            <strong>Note:</strong> Since your browser does not support JavaScript, 
            you must press the Continue button once to proceed.
        </p>
    </noscript>
    <form action=""{url}"" method=""post"">
        <div>
        <input type=""hidden"" name=""SAMLResponse""  value=""{samlResponse}""/>";

            if (!string.IsNullOrWhiteSpace(relayState))
            {
                result = result + $@"<input type=""hidden"" name=""RelayState"" value=""{relayState}""/>";
            }

            return result +
@"</div>
        <noscript>
            <div>
                <input type=""submit"" value=""Continue""/>
            </div>
        </noscript>
    </form>
</body>
</html>";

        }
    }
}
