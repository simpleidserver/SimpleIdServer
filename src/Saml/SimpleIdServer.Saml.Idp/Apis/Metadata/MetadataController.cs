// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Saml.Extensions;
using System.Net;

namespace SimpleIdServer.Saml.Idp.Apis.Metadata
{
    [Route(Saml.Constants.RouteNames.Metadata)]
    public class MetadataController : Controller
    {
        private readonly IMetadataHandler _metadataHandler;

        public MetadataController(IMetadataHandler metadataHandler)
        {
            _metadataHandler = metadataHandler;
        }

        [HttpGet]
        public IActionResult Index()
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
