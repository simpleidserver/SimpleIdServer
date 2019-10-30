// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jwe.EncHandlers;
using SimpleIdServer.OAuth.Api.Configuration;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Configuration
{
    [Route(SIDOpenIdConstants.EndPoints.OpenIDConfiguration)]
    public class ConfigurationController : OAuth.Api.Configuration.ConfigurationController
    {
        private readonly IEnumerable<ICEKHandler> _cekHandlers;
        private readonly IEnumerable<IEncHandler> _encHandlers;
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;

        public ConfigurationController(IConfigurationRequestHandler configurationRequestHandler, IEnumerable<ICEKHandler> cekHandlers, IEnumerable<IEncHandler> encHandlers,
            IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders) : base(configurationRequestHandler)
        {
            _cekHandlers = cekHandlers;
            _encHandlers = encHandlers;
            _subjectTypeBuilders = subjectTypeBuilders;
        }

        [HttpGet]
        public override async Task<IActionResult> Get()
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var result = await Build();
            result.Add(OpenIDConfigurationNames.UserInfoEndpoint, $"{issuer}/{SIDOpenIdConstants.EndPoints.UserInfo}");
            result.Add(OpenIDConfigurationNames.CheckSessionIframe, $"{issuer}/{SIDOpenIdConstants.EndPoints.CheckSession}");
            result.Add(OpenIDConfigurationNames.EndSessionEndpoint, $"{issuer}/{SIDOpenIdConstants.EndPoints.EndSession}");
            result.Add(OpenIDConfigurationNames.RequestParameterSupported, true);
            result.Add(OpenIDConfigurationNames.RequestUriParameterSupported, true);
            result.Add(OpenIDConfigurationNames.RequestObjectEncryptionAlgValuesSupported, JArray.FromObject(_cekHandlers.Select(r => r.AlgName)));
            result.Add(OpenIDConfigurationNames.RequestObjectEncryptionEncValuesSupported, JArray.FromObject(_encHandlers.Select(r => r.EncName)));
            result.Add(OpenIDConfigurationNames.SubjectTypesSupported, JArray.FromObject(_subjectTypeBuilders.Select(r => r.SubjectType)));
            return new OkObjectResult(result);
        }
    }
}
