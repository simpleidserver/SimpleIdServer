// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Configuration;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Extensions;
using SimpleIdServer.IdServer.Options;
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
        private readonly IdServerHostOptions _options;

        public OpenIdConfigurationController(IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders, IScopeRepository scopeRepository, IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository, IOptions<IdServerHostOptions> options, IOAuthConfigurationRequestHandler configurationRequestHandler) : base(configurationRequestHandler)
        {
            _subjectTypeBuilders = subjectTypeBuilders;
            _scopeRepository = scopeRepository;
            _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
            _options = options.Value;
        }

        [HttpGet]
        public override async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var acrLst = await _authenticationContextClassReferenceRepository.Query().AsNoTracking().ToListAsync(cancellationToken);
            var result = await Build(cancellationToken);
            var claims = await _scopeRepository.Query()
                .Include(s => s.ClaimMappers)
                .AsNoTracking()
                .Where(s => s.IsExposedInConfigurationEdp)
                .SelectMany(s => s.ClaimMappers)
                .ToListAsync(cancellationToken);

            result.Add(OpenIDConfigurationNames.UserInfoEndpoint, $"{issuer}/{Constants.EndPoints.UserInfo}");
            result.Add(OpenIDConfigurationNames.CheckSessionIframe, $"{issuer}/{Constants.EndPoints.CheckSession}");
            result.Add(OpenIDConfigurationNames.EndSessionEndpoint, $"{issuer}/{Constants.EndPoints.EndSession}");
            result.Add(OpenIDConfigurationNames.BackchannelAuthenticationEndpoint, $"{issuer}/{Constants.EndPoints.MtlsBCAuthorize}");
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
            result.Add(OpenIDConfigurationNames.ClaimsSupported, JsonSerializer.SerializeToNode(claims.DistinctBy(c => c.TokenClaimName).Select(c => c.TokenClaimName)));
            result.Add(OpenIDConfigurationNames.ClaimsParameterSupported, true);
            result.Add(OpenIDConfigurationNames.BackchannelTokenDeliveryModesSupported, JsonSerializer.SerializeToNode(Constants.AllStandardNotificationModes));
            result.Add(OpenIDConfigurationNames.BackchannelAuthenticationRequestSigningAlgValues, JsonSerializer.SerializeToNode(Constants.AllSigningAlgs));
            result.Add(OpenIDConfigurationNames.BackchannelUserCodeParameterSupported, false);
            result.Add(OpenIDConfigurationNames.FrontChannelLogoutSupported, true);
            result.Add(OpenIDConfigurationNames.FrontChannelLogoutSessionSupported, true);
            result.Add(OpenIDConfigurationNames.BackchannelLogoutSupported, true);
            result.Add(OpenIDConfigurationNames.BackchannelLogoutSessionSupported, true);
            result.Add(OpenIDConfigurationNames.GrantManagementActionRequired, _options.GrantManagementActionRequired);
            result.Add(OpenIDConfigurationNames.GrantManagementEndpoint, $"{issuer}/{Constants.EndPoints.Grants}");
            result.Add(OpenIDConfigurationNames.GrantManagementActionsSupported, JsonSerializer.SerializeToNode(Constants.AllStandardGrantManagementActions));
            return new OkObjectResult(result);
        }
    }
}
