// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.Uma.Api.Token.Fetchers;
using SimpleIdServer.Uma.Api.Token.Validators;
using SimpleIdServer.Uma.Domains;
using SimpleIdServer.Uma.DTOs;
using SimpleIdServer.Uma.Extensions;
using SimpleIdServer.Uma.Helpers;
using SimpleIdServer.Uma.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api.Token.Handlers
{
    public class UmaTicketHandler : BaseCredentialsHandler
    {
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IUmaTicketGrantTypeValidator _umaTicketGrantTypeValidator;
        private readonly IEnumerable<IClaimTokenFormat> _claimTokenFormatFetchers;
        private readonly IUMAPermissionTicketHelper _umaPermissionTicketHelper;
        private readonly IUMAResourceQueryRepository _umaResourceQueryRepository;
        private readonly IUMAPendingRequestCommandRepository _umaPendingRequestCommandRepository;
        private readonly IUMAPendingRequestQueryRepository _umaPendingRequestQueryRepository;
        private readonly UMAHostOptions _umaHostOptions;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;

        public UmaTicketHandler(IEnumerable<ITokenProfile>  tokenProfiles, IUmaTicketGrantTypeValidator umaTicketGrantTypeValidator, IEnumerable<IClaimTokenFormat> claimTokenFormatFetchers, IUMAPermissionTicketHelper umaPermissionTicketHelper, IUMAResourceQueryRepository umaResourceQueryRepository, IUMAPendingRequestCommandRepository umaPendingRequestCommandRepository, IUMAPendingRequestQueryRepository umaPendingRequestQueryRepository, IEnumerable<ITokenBuilder> tokenBuilders, IOptions<UMAHostOptions> umaHostOptions, IClientAuthenticationHelper clientAuthenticationHelper) : base(clientAuthenticationHelper)
        {
            _tokenProfiles = tokenProfiles;
            _umaTicketGrantTypeValidator = umaTicketGrantTypeValidator;
            _claimTokenFormatFetchers = claimTokenFormatFetchers;
            _umaPermissionTicketHelper = umaPermissionTicketHelper;
            _umaResourceQueryRepository = umaResourceQueryRepository;
            _umaPendingRequestCommandRepository = umaPendingRequestCommandRepository;
            _umaPendingRequestQueryRepository = umaPendingRequestQueryRepository;
            _umaHostOptions = umaHostOptions.Value;
            _tokenBuilders = tokenBuilders;
        }

        public const string GRANT_TYPE = "urn:ietf:params:oauth:grant-type:uma-ticket";
        public override string GrantType => GRANT_TYPE;

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                _umaTicketGrantTypeValidator.Validate(context);
                var oauthClient = await AuthenticateClient(context, cancellationToken);
                context.SetClient(oauthClient);
                var ticket = context.Request.Data.GetTicket();
                var claimTokenFormat = context.Request.Data.GetClaimTokenFormat();
                if (string.IsNullOrWhiteSpace(claimTokenFormat))
                {
                    claimTokenFormat = _umaHostOptions.DefaultClaimTokenFormat;
                }

                var scopes = context.Request.Data.GetScopesFromAuthorizationRequest();
                var permissionTicket = await _umaPermissionTicketHelper.GetTicket(ticket);
                if (permissionTicket == null)
                {
                    throw new OAuthException(ErrorCodes.INVALID_GRANT, UMAErrorMessages.INVALID_TICKET);
                }

                ClaimTokenFormatFetcherResult claimTokenFormatFetcherResult = null;
                if (!string.IsNullOrWhiteSpace(claimTokenFormat))
                {
                    var claimTokenFormatFetcher = _claimTokenFormatFetchers.FirstOrDefault(c => c.Name == claimTokenFormat);
                    if (claimTokenFormatFetcher == null)
                    {
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(UMAErrorMessages.BAD_TOKEN_FORMAT, claimTokenFormat));
                    }

                    claimTokenFormatFetcherResult = await claimTokenFormatFetcher.Fetch(context);
                }

                if (claimTokenFormatFetcherResult == null)
                {
                    return BuildError(HttpStatusCode.Unauthorized, UMAErrorCodes.REQUEST_DENIED, UMAErrorMessages.REQUEST_DENIED);
                }

                var invalidScopes = permissionTicket.Records.Any(rec => !scopes.All(sc => rec.Scopes.Contains(sc)));
                if (invalidScopes)
                {
                    throw new OAuthException(ErrorCodes.INVALID_SCOPE, UMAErrorMessages.INVALID_SCOPE);
                }

                var umaResources = await _umaResourceQueryRepository.FindByIdentifiers(permissionTicket.Records.Select(r => r.ResourceId));
                var requiredClaims = new List<UMAResourcePermissionClaim>();
                foreach (var umaResource in umaResources)
                {
                    foreach (var permission in umaResource.Permissions)
                    {
                        if (permission.Scopes.Any(sc => scopes.Contains(sc)))
                        {
                            var unknownClaims = permission.Claims.Where(cl => !claimTokenFormatFetcherResult.Payload.Any(c => c.Key == cl.Name));
                            requiredClaims.AddRange(unknownClaims);
                        }
                    }
                }

                if (requiredClaims.Any())
                {
                    var needInfoResult = new JObject
                    {
                        { "need_info",  new JObject
                        {
                            { UMATokenRequestParameters.Ticket, permissionTicket.Id },
                            { "required_claims", new JArray(requiredClaims.Select(rc => new JObject
                            {
                                { UMAResourcePermissionNames.ClaimTokenFormat, _umaHostOptions.DefaultClaimTokenFormat },
                                { UMAResourcePermissionNames.ClaimType, rc.ClaimType },
                                { UMAResourcePermissionNames.ClaimFriendlyName, rc.FriendlyName },
                                { UMAResourcePermissionNames.ClaimName, rc.Name }
                            })) },
                            { "redirect_uri", _umaHostOptions.OpenIdRedirectUrl }
                        }}
                    };
                    return new ContentResult
                    {
                        Content = needInfoResult.ToString(),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Unauthorized
                    };
                }

                var isNotAuthorized = umaResources.Any(ua => ua.Permissions.Where(p => p.Scopes.Any(sc => scopes.Contains(sc)))
                    .All(pr => pr.Claims.All(cl => claimTokenFormatFetcherResult.Payload.Any(c => c.Key == cl.Name && !c.Value.ToString().Equals(cl.Value, StringComparison.InvariantCultureIgnoreCase)))));
                if (isNotAuthorized)
                {
                    var pendingRequests = await _umaPendingRequestQueryRepository.FindByTicketIdentifier(permissionTicket.Id);
                    if (pendingRequests.Any())
                    {
                        return BuildError(HttpStatusCode.Unauthorized, UMAErrorCodes.REQUEST_DENIED, UMAErrorMessages.REQUEST_DENIED);
                    }

                    foreach(var umaResource in umaResources)
                    {
                        var permissionTicketRecord = permissionTicket.Records.First(r => r.ResourceId == umaResource.Id);
                        var umaPendingRequest = new UMAPendingRequest(permissionTicket.Id, umaResource.Subject, DateTime.UtcNow)
                        {
                            Requester = claimTokenFormatFetcherResult.Subject,
                            Scopes = umaResource.Scopes,
                            Resource = umaResource
                        };
                        _umaPendingRequestCommandRepository.Add(umaPendingRequest);
                    }

                    await _umaPendingRequestCommandRepository.SaveChanges(cancellationToken);
                    return new ContentResult
                    {
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Content = new JObject
                        {
                            { "request_submitted", new JObject
                            {
                                { UMATokenRequestParameters.Ticket, permissionTicket.Id },
                                { "interval", _umaHostOptions.RequestSubmittedInterval }
                            } }
                        }.ToString()
                    };
                }

                var jArr = new JArray();
                foreach (var permission in permissionTicket.Records)
                {
                    jArr.Add(new JObject()
                    {
                        { UMAPermissionNames.ResourceId, permission.ResourceId },
                        { UMAPermissionNames.ResourceScopes, new JArray(permission.Scopes) }
                    });
                }

                var result = BuildResult(context, scopes);
                foreach (var tokenBuilder in _tokenBuilders)
                {
                    await tokenBuilder.Build(scopes, new JObject
                    {
                        { "permissions", jArr }
                    }, context, cancellationToken);
                }

                _tokenProfiles.First(t => t.Profile == context.Client.PreferredTokenProfile).Enrich(context);
                foreach (var kvp in context.Response.Parameters)
                {
                    result.Add(kvp.Key, kvp.Value);
                }

                return new OkObjectResult(result);
            }
            catch (OAuthException ex)
            {
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }
}