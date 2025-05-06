// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Configuration;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.OpenIdConfiguration;

public interface IOpenidConfigurationRequestHandler
{
    Task<JsonObject> Handle(string issuer, string prefix, CancellationToken cancellationToken);
}

public class OpenidConfigurationRequestHandler : IOpenidConfigurationRequestHandler
{
    private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;
    private readonly IScopeRepository _scopeRepository;
    private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;
    private readonly IOAuthConfigurationRequestHandler _oauthConfigurationRequestHandler;
    private readonly IdServerHostOptions _options;

    public OpenidConfigurationRequestHandler(
        IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders,
        IScopeRepository scopeRepository,
        IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository,
        IOptions<IdServerHostOptions> options,
        IOAuthConfigurationRequestHandler configurationRequestHandler)
    {
        _subjectTypeBuilders = subjectTypeBuilders;
        _scopeRepository = scopeRepository;
        _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
        _options = options.Value;
        _oauthConfigurationRequestHandler = configurationRequestHandler;
    }

    public virtual async Task<JsonObject> Handle(string issuer, string prefix, CancellationToken cancellationToken)
    {
        var result = await _oauthConfigurationRequestHandler.Handle(prefix, issuer, cancellationToken);
        var acrLst = await _authenticationContextClassReferenceRepository.GetAll(prefix, cancellationToken);
        var claims = (await _scopeRepository.GetAllExposedScopes(prefix, cancellationToken)).SelectMany(s => s.ClaimMappers);

        if (!string.IsNullOrWhiteSpace(prefix))
            prefix = $"{prefix}/";

        result.Add(OpenIDConfigurationNames.UserInfoEndpoint, $"{issuer}/{prefix}{Config.DefaultEndpoints.UserInfo}");
        result.Add(OpenIDConfigurationNames.DeviceAuthorizationEndpoint, $"{issuer}/{prefix}{Config.DefaultEndpoints.DeviceAuthorization}");
        result.Add(OpenIDConfigurationNames.CheckSessionIframe, $"{issuer}/{prefix}{Config.DefaultEndpoints.CheckSession}");
        result.Add(OpenIDConfigurationNames.EndSessionEndpoint, $"{issuer}/{prefix}{Config.DefaultEndpoints.EndSession}");
        result.Add(OpenIDConfigurationNames.BackchannelAuthenticationEndpoint, $"{issuer}/{prefix}{Config.DefaultEndpoints.BCAuthorize}");
        result.Add(OpenIDConfigurationNames.PushedAuthorizationRequestEndpoint, $"{issuer}/{prefix}{Config.DefaultEndpoints.PushedAuthorizationRequest}");
        result.Add(OpenIDConfigurationNames.RequestParameterSupported, true);
        result.Add(OpenIDConfigurationNames.RequestUriParameterSupported, true);
        result.Add(OpenIDConfigurationNames.RequestObjectSigningAlgValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllSigAlgs));
        result.Add(OpenIDConfigurationNames.RequestObjectEncryptionAlgValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllEncAlgs));
        result.Add(OpenIDConfigurationNames.RequestObjectEncryptionEncValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllEncs));
        result.Add(OpenIDConfigurationNames.SubjectTypesSupported, JsonSerializer.SerializeToNode(_subjectTypeBuilders.Select(r => r.SubjectType)));
        result.Add(OpenIDConfigurationNames.AcrValuesSupported, JsonSerializer.SerializeToNode(acrLst.Select(_ => _.Name)));
        result.Add(OpenIDConfigurationNames.IdTokenSigningAlgValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllSigAlgs));
        result.Add(OpenIDConfigurationNames.IdTokenEncryptionAlgValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllEncAlgs));
        result.Add(OpenIDConfigurationNames.IdTokenEncryptionEncValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllEncs));
        result.Add(OpenIDConfigurationNames.UserInfoSigningAlgValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllSigAlgs));
        result.Add(OpenIDConfigurationNames.UserInfoEncryptionAlgValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllEncAlgs));
        result.Add(OpenIDConfigurationNames.UserInfoEncryptionEncValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllEncs));
        result.Add(OpenIDConfigurationNames.ClaimsSupported, JsonSerializer.SerializeToNode(claims.DistinctBy(c => c.TargetClaimPath).Select(c => c.TargetClaimPath)));
        result.Add(OpenIDConfigurationNames.ClaimsParameterSupported, true);
        result.Add(OpenIDConfigurationNames.BackchannelTokenDeliveryModesSupported, JsonSerializer.SerializeToNode(DefaultNotificationModes.All));
        result.Add(OpenIDConfigurationNames.BackchannelAuthenticationRequestSigningAlgValues, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllSigAlgs));
        result.Add(OpenIDConfigurationNames.BackchannelUserCodeParameterSupported, false);
        result.Add(OpenIDConfigurationNames.FrontChannelLogoutSupported, true);
        result.Add(OpenIDConfigurationNames.FrontChannelLogoutSessionSupported, true);
        result.Add(OpenIDConfigurationNames.BackchannelLogoutSupported, true);
        result.Add(OpenIDConfigurationNames.BackchannelLogoutSessionSupported, true);
        result.Add(OpenIDConfigurationNames.GrantManagementActionRequired, _options.GrantManagementActionRequired);
        result.Add(OpenIDConfigurationNames.GrantManagementEndpoint, $"{issuer}/{prefix}{Config.DefaultEndpoints.Grants}");
        result.Add(OpenIDConfigurationNames.GrantManagementActionsSupported, JsonSerializer.SerializeToNode(DefaultGrantManagementActions.All));
        result.Add(OpenIDConfigurationNames.AuthorizationSigningAlgValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllSigAlgs));
        result.Add(OpenIDConfigurationNames.AuthorizationEncryptionAlgValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllEncAlgs));
        result.Add(OpenIDConfigurationNames.AuthorizationEncryptionEncValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllEncs));
        result.Add(OpenIDConfigurationNames.RequirePushedAuthorizationRequests, _options.RequiredPushedAuthorizationRequest);
        result.Add(OpenIDConfigurationNames.AuthorizationDetailsSupported, true);
        result.Add(OpenIDConfigurationNames.DPOPSigningAlgValuesSupported, JsonSerializer.SerializeToNode(DefaultTokenSecurityAlgs.AllSigAlgs));
        if (_options.MtlsEnabled)
        {
            result.Add(OpenIDConfigurationNames.MtlsEndpointAliases, JsonSerializer.SerializeToNode(new JsonObject
                {
                    { OAuthConfigurationNames.TokenEndpoint, $"{issuer}/{prefix}{Config.DefaultEndpoints.MtlsToken}" },
                    { OpenIDConfigurationNames.BackchannelAuthenticationEndpoint, $"{issuer}/{prefix}{Config.DefaultEndpoints.MtlsBCAuthorize}" }
                }));
        }

        return result;
    }
}