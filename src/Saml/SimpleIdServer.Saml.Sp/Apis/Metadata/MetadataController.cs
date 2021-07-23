using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Saml.Extensions;
using System.Net;

namespace SimpleIdServer.Saml.Sp.Apis.Metadata
{
    [Route(Saml.Constants.RouteNames.Metadata)]
    public class MetadataController : Controller
    {
        private readonly IMetadataHandler _metadataHandler;

        public MetadataController(IMetadataHandler metadataHandler)
        {
            _metadataHandler = metadataHandler;
        }

        public IActionResult Get()
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var result = _metadataHandler.Get(issuer);
            return new ContentResult
            {
                Content = result.SerializeToXmlElement().OuterXml,
                ContentType = "application/xml",
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
