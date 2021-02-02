// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jwe.EncHandlers;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api.Configuration;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Persistence;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Configuration
{
    [Route(SIDOpenIdConstants.EndPoints.OpenIDConfiguration)]
    public class ConfigurationController : OAuth.Api.Configuration.ConfigurationController
    {
        private readonly IEnumerable<ICEKHandler> _cekHandlers;
        private readonly IEnumerable<IEncHandler> _encHandlers;
        private readonly IEnumerable<ISignHandler> _signHandlers;
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;
        private readonly IAuthenticationContextClassReferenceQueryRepository _authenticationContextClassReferenceQueryRepository;

        public ConfigurationController(
            IConfigurationRequestHandler configurationRequestHandler, 
            IEnumerable<ICEKHandler> cekHandlers, 
            IEnumerable<IEncHandler> encHandlers,
            IEnumerable<ISignHandler> signHandlers,
            IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders,
            IAuthenticationContextClassReferenceQueryRepository authenticationContextClassReferenceQueryRepository) : base(configurationRequestHandler)
        {
            _cekHandlers = cekHandlers;
            _encHandlers = encHandlers;
            _signHandlers = signHandlers;
            _subjectTypeBuilders = subjectTypeBuilders;
            _authenticationContextClassReferenceQueryRepository = authenticationContextClassReferenceQueryRepository;
        }

        [HttpGet]
        public override async Task<IActionResult> Get(CancellationToken token)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var acrLst = await _authenticationContextClassReferenceQueryRepository.GetAllACR(token);
            var result = await Build();
            result.Add(OpenIDConfigurationNames.UserInfoEndpoint, $"{issuer}/{SIDOpenIdConstants.EndPoints.UserInfo}");
            result.Add(OpenIDConfigurationNames.CheckSessionIframe, $"{issuer}/{SIDOpenIdConstants.EndPoints.CheckSession}");
            result.Add(OpenIDConfigurationNames.EndSessionEndpoint, $"{issuer}/{SIDOpenIdConstants.EndPoints.EndSession}");
            result.Add(OpenIDConfigurationNames.RequestParameterSupported, true);
            result.Add(OpenIDConfigurationNames.RequestUriParameterSupported, true);
            result.Add(OpenIDConfigurationNames.RequestObjectSigningAlgValuesSupported, JArray.FromObject(_signHandlers.Select(_ => _.AlgName)));
            result.Add(OpenIDConfigurationNames.RequestObjectEncryptionAlgValuesSupported, JArray.FromObject(_cekHandlers.Select(r => r.AlgName)));
            result.Add(OpenIDConfigurationNames.RequestObjectEncryptionEncValuesSupported, JArray.FromObject(_encHandlers.Select(r => r.EncName)));
            result.Add(OpenIDConfigurationNames.SubjectTypesSupported, JArray.FromObject(_subjectTypeBuilders.Select(r => r.SubjectType)));
            result.Add(OpenIDConfigurationNames.AcrValuesSupported, JArray.FromObject(acrLst.Select(_ => _.Name)));
            result.Add(OpenIDConfigurationNames.IdTokenSigningAlgValuesSupported, JArray.FromObject(_signHandlers.Select(_ => _.AlgName)));
            result.Add(OpenIDConfigurationNames.IdTokenEncryptionAlgValuesSupported, JArray.FromObject(_cekHandlers.Select(r => r.AlgName)));
            result.Add(OpenIDConfigurationNames.IdTokenEncryptionEncValuesSupported, JArray.FromObject(_encHandlers.Select(r => r.EncName)));
            result.Add(OpenIDConfigurationNames.UserInfoSigningAlgValuesSupported, JArray.FromObject(_signHandlers.Select(_ => _.AlgName)));
            result.Add(OpenIDConfigurationNames.UserInfoEncryptionAlgValuesSupported, JArray.FromObject(_cekHandlers.Select(r => r.AlgName)));
            result.Add(OpenIDConfigurationNames.UserInfoEncryptionEncValuesSupported, JArray.FromObject(_encHandlers.Select(r => r.EncName)));
            return new OkObjectResult(result);
        }
    }
}
