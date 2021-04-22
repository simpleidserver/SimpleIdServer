// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OpenID;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OpenBankingApi.Api.Token.TokenBuilders
{
    public class OpenBankingApiAuthRequestEnricher : IOpenBankingApiAuthRequestEnricher
    {
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly OpenBankingApiOptions _options;

        public OpenBankingApiAuthRequestEnricher(
            IBCAuthorizeRepository bcAuthorizeRepository,
            IOptions<OpenBankingApiOptions> options)
        {
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _options = options.Value;
        }

        public async Task Enrich(JwsPayload result, JObject queryParameters, CancellationToken cancellationToken)
        {
            var authRequestId = queryParameters.GetAuthRequestId();
            if (!string.IsNullOrWhiteSpace(authRequestId))
            {
                var authorize = await _bcAuthorizeRepository.Get(authRequestId, cancellationToken);
                if (authorize != null && authorize.Permissions.Any())
                {
                    var firstPermission = authorize.Permissions.First();
                    if (result.ContainsKey(_options.OpenBankingApiConsentClaimName))
                    {
                        result[_options.OpenBankingApiConsentClaimName] = firstPermission.ConsentId;
                    }
                    else
                    {
                        result.Add(_options.OpenBankingApiConsentClaimName, firstPermission.ConsentId);
                    }
                }

                if (authorize.Scopes.Contains(SIDOpenIdConstants.StandardScopes.OpenIdScope.Name) && !result.ContainsKey(UserClaims.Subject))
                {
                    result.Add(UserClaims.Subject, authorize.UserId);
                }
            }

            var requestedClaims = queryParameters.GetClaimsFromAuthorizationRequest();
            if (requestedClaims != null)
            {
                var requestedClaim = requestedClaims.FirstOrDefault(c => c.Name == _options.OpenBankingApiConsentClaimName);
                if (requestedClaim != null)
                {
                    result.Add(_options.OpenBankingApiConsentClaimName, requestedClaim.Values.First());
                }
            }
        }
    }
}
