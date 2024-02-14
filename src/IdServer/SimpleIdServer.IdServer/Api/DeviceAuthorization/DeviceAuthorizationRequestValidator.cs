// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Store;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.DeviceAuthorization
{
    public interface IDeviceAuthorizationRequestValidator
    {
        Task Validate(HandlerContext context, CancellationToken cancellationToken);
    }

    public class DeviceAuthorizationRequestValidator : IDeviceAuthorizationRequestValidator
    {
        private readonly IClientRepository _clientRepository;

        public DeviceAuthorizationRequestValidator(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public virtual async Task Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            var clientId = context.Request.RequestData.GetClientIdFromAuthorizationRequest();
            var scopes = context.Request.RequestData.GetScopesFromAuthorizationRequest();
            context.SetClient(await AuthenticateClient(context.Realm, clientId, cancellationToken));
            var unsupportedScopes = scopes.Where(s => s != Constants.StandardScopes.OpenIdScope.Name && !context.Client.Scopes.Any(sc => sc.Name == s));
            if (unsupportedScopes.Any())
                throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_SCOPES, string.Join(",", unsupportedScopes)));

            async Task<Client> AuthenticateClient(string realm, string clientId, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.ClientId));

                var client = await _clientRepository.Query().Include(c => c.Scopes).ThenInclude(s => s.ClaimMappers)
                    .Include(c => c.SerializedJsonWebKeys)
                    .Include(c => c.Realms)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == realm), cancellationToken);
                if (client == null)
                    throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CLIENT, string.Format(Global.UnknownClient, clientId));

                return client;
            }
        }
    }
}
