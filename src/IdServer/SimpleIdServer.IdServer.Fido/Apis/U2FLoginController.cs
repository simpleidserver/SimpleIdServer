// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.DTOs;
using SimpleIdServer.IdServer.Fido.Extensions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Fido.Apis
{
    public class U2FLoginController : BaseController
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IUserRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IDistributedCache _distributedCache;
        private IFido2 _fido2;

        public U2FLoginController(IAuthenticationHelper authenticationHelper, IUserRepository userRepository, IJwtBuilder jwtBuilder, IDistributedCache distributedCache, IFido2 fido2)
        {
            _authenticationHelper = authenticationHelper;
            _userRepository = userRepository;
            _jwtBuilder = jwtBuilder;
            _distributedCache = distributedCache;
            _fido2 = fido2;
        }

        [HttpPost]
        public async Task<IActionResult> Begin([FromRoute] string prefix, [FromBody] BeginU2FLoginRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (request == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, IdServer.ErrorMessages.INVALID_INCOMING_REQUEST);
            JsonWebToken jsonWebToken = null;
            if (!TryGetIdentityToken(prefix, _jwtBuilder, out jsonWebToken))
                if (string.IsNullOrWhiteSpace(request.Login)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, BeginU2FLoginRequestNames.Login));

            var login = jsonWebToken?.Subject ?? request.Login;
            var authenticatedUser = await _authenticationHelper.GetUserByLogin(_userRepository.Query().Include(u => u.Credentials), login, prefix, cancellationToken);
            if (authenticatedUser == null)
                return BuildError(System.Net.HttpStatusCode.Unauthorized, ErrorCodes.ACCESS_DENIED, string.Format(IdServer.ErrorMessages.UNKNOWN_USER, login));

            var exts = new AuthenticationExtensionsClientInputs()
            {
                Extensions = true,
                UserVerificationMethod = true,
                DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs()
            };
            var existingCredentials = authenticatedUser.GetStoredFidoCredentials().Select(c => c.Descriptor);
            var options = _fido2.GetAssertionOptions(
                existingCredentials,
                UserVerificationRequirement.Discouraged,
                exts
            );
            var sessionId = Guid.NewGuid().ToString();
            await _distributedCache.SetStringAsync(sessionId, options.ToJson());
            return new OkObjectResult(new BeginU2FLoginResult
            {
                SessionId = sessionId,
                Assertion = options
            });
        }

        [HttpPost]
        public async Task<IActionResult> End([FromRoute] string prefix, [FromBody] EndU2FLoginRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (request == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, IdServer.ErrorMessages.INVALID_INCOMING_REQUEST);
            var session = await _distributedCache.GetStringAsync(request.SessionId, cancellationToken);
            if (string.IsNullOrWhiteSpace(session)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.SESSION_CANNOT_BE_EXTRACTED);
            JsonWebToken jsonWebToken = null;
            if (!TryGetIdentityToken(prefix, _jwtBuilder, out jsonWebToken))
                if (string.IsNullOrWhiteSpace(request.Login)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, EndU2FRegisterRequestNames.Login));
            var login = jsonWebToken?.Subject ?? request.Login;
            var authenticatedUser = await _authenticationHelper.GetUserByLogin(_userRepository.Query().Include(u => u.Credentials), login, prefix, cancellationToken);
            if (authenticatedUser == null)
                return BuildError(System.Net.HttpStatusCode.Unauthorized, ErrorCodes.ACCESS_DENIED, string.Format(IdServer.ErrorMessages.UNKNOWN_USER, login));

            var options = AssertionOptions.FromJson(session);
            var storedCredentials = authenticatedUser.GetStoredFidoCredentials();
            IsUserHandleOwnerOfCredentialIdAsync callback = (args, cancellationToken) =>
            {
                return Task.FromResult(storedCredentials.Any(c => c.Descriptor.Id.SequenceEqual(args.CredentialId)));
            };

            var fidoCredentials = authenticatedUser.GetFidoCredentials();
            var fidoCredential = fidoCredentials.First().GetFidoCredential();
            var credential = fidoCredentials.First(c => c.GetFidoCredential().Descriptor.Id.SequenceEqual(request.Assertion.Id));
            var creds = credential.GetFidoCredential();
            var storedCounter = creds.SignatureCounter;
            var res = await _fido2.MakeAssertionAsync(request.Assertion, options, creds.PublicKey, creds.DevicePublicKeys, storedCounter, callback, cancellationToken: cancellationToken);
            creds.SignCount = res.Counter;
            if (res.DevicePublicKey is not null)
                creds.DevicePublicKeys.Add(res.DevicePublicKey);
            credential.Value = JsonSerializer.Serialize(creds);
            await _userRepository.SaveChanges(cancellationToken);
            return NoContent();
        }
    }
}
