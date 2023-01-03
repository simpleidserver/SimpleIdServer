// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Api.Configuration;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Persistence;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using SimpleIdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Configuration
{
    public class ConfigurationController : OAuth.Api.Configuration.ConfigurationController
    {
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;
        private readonly ScopeRepository _scopeRepository;
        private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;

        public ConfigurationController(
            IConfigurationRequestHandler configurationRequestHandler, 
            IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders,
            ScopeRepository scopeRepository,
            IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository) : base(configurationRequestHandler)
        {
            _subjectTypeBuilders = subjectTypeBuilders;
            _scopeRepository = scopeRepository;
            _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
        }

        [HttpGet]
        public override async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var acrLst = await _authenticationContextClassReferenceRepository.GetAllACR(cancellationToken);
            var result = await Build(cancellationToken);
            var claims = await _scopeRepository.Query()
                .Include(s => s.Claims)
                .AsNoTracking()
                .Where(s => s.IsExposedInConfigurationEdp)
                .SelectMany(s => s.Claims)
                .Select(c => c.ClaimName)
                .ToListAsync(cancellationToken);
            result.Add(OpenIDConfigurationNames.UserInfoEndpoint, $"{issuer}/{SIDOpenIdConstants.EndPoints.UserInfo}");
            result.Add(OpenIDConfigurationNames.CheckSessionIframe, $"{issuer}/{SIDOpenIdConstants.EndPoints.CheckSession}");
            result.Add(OpenIDConfigurationNames.EndSessionEndpoint, $"{issuer}/{SIDOpenIdConstants.EndPoints.EndSession}");
            result.Add(OpenIDConfigurationNames.BackchannelAuthenticationEndpoint, $"{issuer}/{SIDOpenIdConstants.EndPoints.MTLSBCAuthorize}");
            result.Add(OpenIDConfigurationNames.RequestParameterSupported, true);
            result.Add(OpenIDConfigurationNames.RequestUriParameterSupported, true);
            result.Add(OpenIDConfigurationNames.RequestObjectSigningAlgValuesSupported, JsonSerializer.SerializeToNode(OAuth.Constants.AllSigningAlgs));
            result.Add(OpenIDConfigurationNames.RequestObjectEncryptionAlgValuesSupported, JsonSerializer.SerializeToNode(OAuth.Constants.AllEncAlgs));
            result.Add(OpenIDConfigurationNames.RequestObjectEncryptionEncValuesSupported, JsonSerializer.SerializeToNode(OAuth.Constants.AllEncryptions));
            result.Add(OpenIDConfigurationNames.SubjectTypesSupported, JsonSerializer.SerializeToNode(_subjectTypeBuilders.Select(r => r.SubjectType)));
            result.Add(OpenIDConfigurationNames.AcrValuesSupported, JsonSerializer.SerializeToNode(acrLst.Select(_ => _.Name)));
            result.Add(OpenIDConfigurationNames.IdTokenSigningAlgValuesSupported, JsonSerializer.SerializeToNode(OAuth.Constants.AllSigningAlgs));
            result.Add(OpenIDConfigurationNames.IdTokenEncryptionAlgValuesSupported, JsonSerializer.SerializeToNode(OAuth.Constants.AllEncAlgs));
            result.Add(OpenIDConfigurationNames.IdTokenEncryptionEncValuesSupported, JsonSerializer.SerializeToNode(OAuth.Constants.AllEncryptions));
            result.Add(OpenIDConfigurationNames.UserInfoSigningAlgValuesSupported, JsonSerializer.SerializeToNode(OAuth.Constants.AllSigningAlgs));
            result.Add(OpenIDConfigurationNames.UserInfoEncryptionAlgValuesSupported, JsonSerializer.SerializeToNode(OAuth.Constants.AllEncAlgs));
            result.Add(OpenIDConfigurationNames.UserInfoEncryptionEncValuesSupported, JsonSerializer.SerializeToNode(OAuth.Constants.AllEncryptions));
            result.Add(OpenIDConfigurationNames.ClaimsSupported, JsonSerializer.SerializeToNode(claims));
            result.Add(OpenIDConfigurationNames.ClaimsParameterSupported, true);
            result.Add(OpenIDConfigurationNames.BackchannelTokenDeliveryModesSupported, JsonSerializer.SerializeToNode(SIDOpenIdConstants.AllStandardNotificationModes));
            result.Add(OpenIDConfigurationNames.BackchannelAuthenticationRequestSigningAlgValues, JsonSerializer.SerializeToNode(OAuth.Constants.AllSigningAlgs));
            result.Add(OpenIDConfigurationNames.BackchannelUserCodeParameterSupported, false);
            result.Add(OpenIDConfigurationNames.FrontChannelLogoutSupported, true);
            result.Add(OpenIDConfigurationNames.FrontChannelLogoutSessionSupported, true);
            result.Add(OpenIDConfigurationNames.BackchannelLogoutSupported, true);
            result.Add(OpenIDConfigurationNames.BackchannelLogoutSessionSupported, true);
            return new OkObjectResult(result);
        }
    }
}
