﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using QRCoder;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.DTOs;
using SimpleIdServer.IdServer.Fido.Extensions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Fido.Apis
{
    public class U2FLoginController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IUserRepository _userRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IEnumerable<IAuthenticationMethodService> _authenticationMethodServices;
        private IFido2 _fido2;

        public U2FLoginController(
            IConfiguration configuration, 
            IAuthenticationHelper authenticationHelper, 
            IUserRepository userRepository, 
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder, 
            IDistributedCache distributedCache,
            ITransactionBuilder transactionBuilder,
            IEnumerable<IAuthenticationMethodService> authenticationMethodServices,
            IFido2 fido2) : base(tokenRepository, jwtBuilder)
        {
            _configuration = configuration;
            _authenticationHelper = authenticationHelper;
            _userRepository = userRepository;
            _distributedCache = distributedCache;
            _transactionBuilder = transactionBuilder;
            _authenticationMethodServices = authenticationMethodServices;
            _fido2 = fido2;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatus([FromRoute] string prefix, string sessionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, nameof(sessionId)));
            var session = await _distributedCache.GetStringAsync(sessionId, cancellationToken);
            if (string.IsNullOrWhiteSpace(session)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Resources.Global.SessionCannotBeExtracted);
            var sessionRecord = JsonSerializer.Deserialize<AuthenticationSessionRecord>(session);
            if (!sessionRecord.IsValidated) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Resources.Global.AuthenticationNotConfirmed);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> BeginQRCode([FromRoute] string prefix, [FromBody] BeginU2FLoginRequest request, CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            var kvp = await CommonBegin(prefix, request, cancellationToken);
            if (kvp.Item2 != null) return kvp.Item2;
            var qrGenerator = new QRCodeGenerator();
            var json = JsonSerializer.Serialize(new QRCodeResult
            {
                Action = "login",
                SessionId = kvp.Item1.SessionId,
                ReadQRCodeURL = $"{issuer}/{prefix}/{Constants.EndPoints.ReadLoginQRCode}/{kvp.Item1.SessionId}"
            });
            var qrCodeData = qrGenerator.CreateQrCode(json, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var payload = qrCode.GetGraphic(20);
            Response.Headers.Add("SessionId", kvp.Item1.SessionId);
            Response.Headers.Add("QRCode", json);
            return File(payload, "image/png");
        }

        [HttpGet]
        public async Task<IActionResult> ReadQRCode([FromRoute] string prefix, string sessionId, CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (string.IsNullOrWhiteSpace(sessionId)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, nameof(sessionId)));
            var session = await _distributedCache.GetStringAsync(sessionId, cancellationToken);
            if (string.IsNullOrWhiteSpace(session)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Resources.Global.SessionCannotBeExtracted);
            var sessionRecord = JsonSerializer.Deserialize<AuthenticationSessionRecord>(session);
            return new OkObjectResult(new BeginU2FLoginResult
            {
                Assertion = sessionRecord.Options,
                SessionId = sessionId,
                EndLoginUrl = $"{issuer}/{prefix}/{Constants.EndPoints.EndLogin}",
                Login = sessionRecord.Login
            });
        }

        [HttpPost]
        public async Task<IActionResult> Begin([FromRoute] string prefix, [FromBody] BeginU2FLoginRequest request, CancellationToken cancellationToken)
        {
            var kvp = await CommonBegin(prefix, request, cancellationToken);
            if (kvp.Item2 != null) return kvp.Item2;
            return new OkObjectResult(kvp.Item1);
        }

        [HttpPost]
        public async Task<IActionResult> End([FromRoute] string prefix, [FromBody] EndU2FLoginRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                prefix = prefix ?? IdServer.Constants.DefaultRealm;
                if (request == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
                var session = await _distributedCache.GetStringAsync(request.SessionId, cancellationToken);
                if (string.IsNullOrWhiteSpace(session)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Resources.Global.SessionCannotBeExtracted);
                JsonWebToken jsonWebToken = null;
                if (!TryGetIdentityToken(prefix, out jsonWebToken))
                    if (string.IsNullOrWhiteSpace(request.Login)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, EndU2FRegisterRequestNames.Login));
                var login = jsonWebToken?.Subject ?? request.Login;
                var authenticatedUser = await _authenticationHelper.GetUserByLogin(login, prefix, cancellationToken);
                if (authenticatedUser == null)
                    return BuildError(System.Net.HttpStatusCode.Unauthorized, ErrorCodes.ACCESS_DENIED, string.Format(SimpleIdServer.IdServer.Resources.Global.UnknownUser, login));

                var sessionRecord = JsonSerializer.Deserialize<AuthenticationSessionRecord>(session);
                var fidoOptions = GetOptions(sessionRecord.CredentialType);
                var options = sessionRecord.Options;
                var storedCredentials = authenticatedUser.GetStoredFidoCredentials(sessionRecord.CredentialType);
                IsUserHandleOwnerOfCredentialIdAsync callback = (args, cancellationToken) =>
                {
                    return Task.FromResult(storedCredentials.Any(c => c.Descriptor.Id.SequenceEqual(args.CredentialId)));
                };

                var fidoCredentials = authenticatedUser.GetFidoCredentials(sessionRecord.CredentialType);
                var credential = fidoCredentials.First(c => c.GetFidoCredential(sessionRecord.CredentialType).Descriptor.Id.SequenceEqual(request.Assertion.Id));
                var creds = credential.GetFidoCredential(sessionRecord.CredentialType);
                var storedCounter = creds.SignatureCounter;
                var res = await _fido2.MakeAssertionAsync(request.Assertion, options, creds.PublicKey, creds.DevicePublicKeys, storedCounter, callback, cancellationToken: cancellationToken);
                creds.SignCount = res.Counter;
                if (res.DevicePublicKey is not null)
                    creds.DevicePublicKeys.Add(res.DevicePublicKey);
                credential.Value = JsonSerializer.Serialize(creds);
                _userRepository.Update(authenticatedUser);
                await transaction.Commit(cancellationToken);
                sessionRecord.IsValidated = true;
                await _distributedCache.SetStringAsync(request.SessionId, JsonSerializer.Serialize(sessionRecord), new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(fidoOptions.U2FExpirationTimeInSeconds)
                }, cancellationToken);
                return NoContent();
            }
        }

        protected async Task<(BeginU2FLoginResult, IActionResult)> CommonBegin(string prefix, BeginU2FLoginRequest request, CancellationToken cancellationToken)
        {
            var fidoOptions = GetOptions(request.CredentialType);
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (request == null) return (null, BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest));
            JsonWebToken jsonWebToken = null;
            if (!TryGetIdentityToken(prefix, out jsonWebToken))
                if (string.IsNullOrWhiteSpace(request.Login)) return (null, BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, BeginU2FLoginRequestNames.Login)));

            var login = jsonWebToken?.Subject ?? request.Login;
            var authenticatedUser = await _authenticationHelper.GetUserByLogin(login, prefix, cancellationToken);
            if (authenticatedUser == null)
                return (null, BuildError(System.Net.HttpStatusCode.Unauthorized, ErrorCodes.ACCESS_DENIED, string.Format(IdServer.Resources.Global.UnknownUser, login)));

            var fidoCredentials = authenticatedUser.GetStoredFidoCredentials(request.CredentialType);
            if (!fidoCredentials.Any())
                return (null, BuildError(System.Net.HttpStatusCode.Unauthorized, ErrorCodes.ACCESS_DENIED, string.Format(Fido.Resources.Global.NoCredential, login)));

            var exts = new AuthenticationExtensionsClientInputs()
            {
                Extensions = true,
                UserVerificationMethod = true,
                DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs()
            };
            var existingCredentials = fidoCredentials.Select(c => c.Descriptor);
            var options = _fido2.GetAssertionOptions(
                existingCredentials,
                UserVerificationRequirement.Discouraged,
                exts
            );
            var sessionId = Guid.NewGuid().ToString();
            var sessionRecord = new AuthenticationSessionRecord(options, request.Login, request.CredentialType);
            await _distributedCache.SetStringAsync(sessionId, JsonSerializer.Serialize(sessionRecord), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(fidoOptions.U2FExpirationTimeInSeconds)
            }, cancellationToken);
            return (new BeginU2FLoginResult
            {
                SessionId = sessionId,
                Assertion = options,
                EndLoginUrl = $"{issuer}/{prefix}/{Constants.EndPoints.EndLogin}",
                Login = request.Login
            }, null);
        }

        private IFidoOptions GetOptions(string credentialType)
        {
            var authenticationMethodService = _authenticationMethodServices.Single(a => a.Amr == credentialType);
            var section = _configuration.GetSection(authenticationMethodService.OptionsType.Name);
            return section.Get(authenticationMethodService.OptionsType) as IFidoOptions;
        }
    }

    public record AuthenticationSessionRecord
    {
        public AuthenticationSessionRecord()
        {

        }

        public AuthenticationSessionRecord(AssertionOptions assertionOptions, string login, string credentialType)
        {
            SerializedOptions = assertionOptions.ToJson();
            Login = login;
            CredentialType = credentialType;
        }

        public string SerializedOptions { get; private set; }
        public bool IsValidated { get; set; } = false;
        public string Login { get; set; } = null!;
        public string CredentialType { get; set; }

        public AssertionOptions Options
        {
            get
            {
                return AssertionOptions.FromJson(SerializedOptions);
            }
            set
            {
                if (value == null) return;
                SerializedOptions = value.ToJson();
            }
        }
    }
}
