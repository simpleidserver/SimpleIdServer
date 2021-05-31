// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Options;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Metadata
{
    [Route(SIDOpenIdConstants.EndPoints.Metadata)]
    public class MetadataController: Controller
    {
        private readonly OAuthHostOptions _options;

        public MetadataController(IOptions<OAuthHostOptions> options)
        {
            _options = options.Value;
        }

        [HttpGet("languages")]
        public IActionResult GetLanguages()
        {
            var result = new JArray();
            foreach(var supportedUICulture in _options.SupportedUICultures)
            {
                result.Add(new JObject
                {
                    { LanguageResponseParameters.Name, supportedUICulture.Name },
                    { LanguageResponseParameters.DisplayName, supportedUICulture.DisplayName }
                });
            }

            return new OkObjectResult(result);
        }

        [HttpGet("applicationtypes")]
        public async Task<IActionResult> GetApplicationTypes()
        {
            return null;
        }
    }
}
