// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public interface IPreAuthorizedCodeValidator
    {
        Task<ValidationResult> Validate(HandlerContext context, CancellationToken cancellationToken);
    }

    public record ValidationResult
    {
        public ValidationResult(Client client, User user)
        {
            Client = client;
            User = user;
        }

        public Client Client { get; private set; }
        public User User { get; private set; }
    }

    public class PreAuthorizedCodeValidator : IPreAuthorizedCodeValidator
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IUserRepository _userRepository;
        private readonly IUserClaimsService _userClaimsService;

        public PreAuthorizedCodeValidator(IClientAuthenticationHelper clientAuthenticationHelper, IClientRepository clientRepository, IGrantedTokenHelper grantedTokenHelper, IUserRepository userRepository, IUserClaimsService userClaimsService)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _clientRepository = clientRepository;
            _grantedTokenHelper = grantedTokenHelper;
            _userRepository = userRepository;
            _userClaimsService = userClaimsService;
        }

        public async virtual Task<ValidationResult> Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            string clientId;
            if (!_clientAuthenticationHelper.TryGetClientId(context.Realm, context.Request.HttpHeader, context.Request.RequestData, context.Request.Certificate, out clientId)) throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.MISSING_CLIENT_ID);
            var client = await _clientRepository.Query().AsNoTracking().Include(c => c.Scopes).SingleOrDefaultAsync(c => c.ClientId == clientId, cancellationToken);
            if (client == null) throw new OAuthException(ErrorCodes.INVALID_CLIENT, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));
            if (!client.GrantTypes.Contains(PreAuthorizedCodeHandler.GRANT_TYPE)) throw new OAuthException(ErrorCodes.INVALID_GRANT, string.Format(ErrorMessages.UNSUPPORTED_GRANT_TYPE, PreAuthorizedCodeHandler.GRANT_TYPE));
            context.SetClient(client);
            var preAuthorizedCode = context.Request.RequestData.GetPreAuthorizedCode();
            var userPin = context.Request.RequestData.GetUserPin();
            if (string.IsNullOrWhiteSpace(preAuthorizedCode)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.PreAuthorizedCode));
            if (client.UserPinRequired && string.IsNullOrWhiteSpace(userPin)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.UserPin));
            var preAuth = await _grantedTokenHelper.GetPreAuthCode(preAuthorizedCode, cancellationToken);
            if (preAuth == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_PREAUTHORIZEDCODE);
            var user = await _userRepository.Query().AsNoTracking().SingleAsync(u => u.Id == preAuth.UserId, cancellationToken);
            var userClaims = await _userClaimsService.Get(user.Id, context.Realm, cancellationToken);
            context.SetUser(user, userClaims);
            return new ValidationResult(client, user);
        }
    }
}
