// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Api.Configuration;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Extensions;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.OpenIdConfiguration
{
    public class OpenIdConfigurationController : OAuthConfigurationController
    {
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;
        private readonly IScopeRepository _scopeRepository;
        private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;

        public OpenIdConfigurationController(IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders, IScopeRepository scopeRepository, IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository, IOAuthConfigurationRequestHandler configurationRequestHandler) : base(configurationRequestHandler)
        {
            _subjectTypeBuilders = subjectTypeBuilders;
            _scopeRepository = scopeRepository;
            _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
        }

        [HttpGet]
        public override async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var acrLst = await _authenticationContextClassReferenceRepository.Query().AsNoTracking().ToListAsync(cancellationToken);
            var result = await Build(cancellationToken);
            var claims = await _scopeRepository.Query()
                .Include(s => s.Claims)
                .AsNoTracking()
                .Where(s => s.IsExposedInConfigurationEdp)
                .SelectMany(s => s.Claims)
                .Select(c => c.ClaimName)
                .ToListAsync(cancellationToken);
            result.Add(OpenIDConfigurationNames.UserInfoEndpoint, $"{issuer}/{Constants.EndPoints.UserInfo}");
            result.Add(OpenIDConfigurationNames.CheckSessionIframe, $"{issuer}/{Constants.EndPoints.CheckSession}");
            result.Add(OpenIDConfigurationNames.EndSessionEndpoint, $"{issuer}/{Constants.EndPoints.EndSession}");
            result.Add(OpenIDConfigurationNames.BackchannelAuthenticationEndpoint, $"{issuer}/{Constants.EndPoints.BCAuthorize}");
            result.Add(OpenIDConfigurationNames.RequestParameterSupported, true);
            result.Add(OpenIDConfigurationNames.RequestUriParameterSupported, true);
            result.Add(OpenIDConfigurationNames.RequestObjectSigningAlgValuesSupported, JsonSerializer.SerializeToNode(Constants.AllSigningAlgs));
            result.Add(OpenIDConfigurationNames.RequestObjectEncryptionAlgValuesSupported, JsonSerializer.SerializeToNode(Constants.AllEncAlgs));
            result.Add(OpenIDConfigurationNames.RequestObjectEncryptionEncValuesSupported, JsonSerializer.SerializeToNode(Constants.AllEncryptions));
            result.Add(OpenIDConfigurationNames.SubjectTypesSupported, JsonSerializer.SerializeToNode(_subjectTypeBuilders.Select(r => r.SubjectType)));
            result.Add(OpenIDConfigurationNames.AcrValuesSupported, JsonSerializer.SerializeToNode(acrLst.Select(_ => _.Name)));
            result.Add(OpenIDConfigurationNames.IdTokenSigningAlgValuesSupported, JsonSerializer.SerializeToNode(Constants.AllSigningAlgs));
            result.Add(OpenIDConfigurationNames.IdTokenEncryptionAlgValuesSupported, JsonSerializer.SerializeToNode(Constants.AllEncAlgs));
            result.Add(OpenIDConfigurationNames.IdTokenEncryptionEncValuesSupported, JsonSerializer.SerializeToNode(Constants.AllEncryptions));
            result.Add(OpenIDConfigurationNames.UserInfoSigningAlgValuesSupported, JsonSerializer.SerializeToNode(Constants.AllSigningAlgs));
            result.Add(OpenIDConfigurationNames.UserInfoEncryptionAlgValuesSupported, JsonSerializer.SerializeToNode(Constants.AllEncAlgs));
            result.Add(OpenIDConfigurationNames.UserInfoEncryptionEncValuesSupported, JsonSerializer.SerializeToNode(Constants.AllEncryptions));
            result.Add(OpenIDConfigurationNames.ClaimsSupported, JsonSerializer.SerializeToNode(claims));
            result.Add(OpenIDConfigurationNames.ClaimsParameterSupported, true);
            result.Add(OpenIDConfigurationNames.BackchannelTokenDeliveryModesSupported, JsonSerializer.SerializeToNode(Constants.AllStandardNotificationModes));
            result.Add(OpenIDConfigurationNames.BackchannelAuthenticationRequestSigningAlgValues, JsonSerializer.SerializeToNode(Constants.AllSigningAlgs));
            result.Add(OpenIDConfigurationNames.BackchannelUserCodeParameterSupported, false);
            result.Add(OpenIDConfigurationNames.FrontChannelLogoutSupported, true);
            result.Add(OpenIDConfigurationNames.FrontChannelLogoutSessionSupported, true);
            result.Add(OpenIDConfigurationNames.BackchannelLogoutSupported, true);
            result.Add(OpenIDConfigurationNames.BackchannelLogoutSessionSupported, true);
            return new OkObjectResult(result);
        }
    }
}
