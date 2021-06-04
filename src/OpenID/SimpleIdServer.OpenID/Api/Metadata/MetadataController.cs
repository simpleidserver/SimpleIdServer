// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Metadata
{
    [Route(SIDOpenIdConstants.EndPoints.Metadata)]
    public class MetadataController: Controller
    {
        private readonly OAuthHostOptions _options;
        private readonly IMetadataResultBuilder _metadataResultBuilder;

        public MetadataController(
            IOptions<OAuthHostOptions> options,
            IMetadataResultBuilder metadataResultBuilder)
        {
            _options = options.Value;
            _metadataResultBuilder = metadataResultBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetMetadata(CancellationToken cancellationToken)
        {
            var name = Thread.CurrentThread.CurrentCulture.Name;
            var result = await _metadataResultBuilder
                .AddTranslatedEnum<ApplicationKinds>("applicationKind")
                .Build(name, cancellationToken);
            return new OkObjectResult(result);
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
    }
}
