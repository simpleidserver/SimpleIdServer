// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using QRCoder;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.DTOs;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Fido.Apis
{
    public class U2FRegisterController : BaseController
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IUserRepository _userRepository;
        private readonly IFido2 _fido2;
        private readonly IDistributedCache _distributedCache;
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly FidoOptions _options;
        private readonly IdServerHostOptions _idServerHostOptions;

        public U2FRegisterController(IAuthenticationHelper authenticationHelper, IUserRepository userRepository, IFido2 fido2, IDistributedCache distributedCache, IUserCredentialRepository userCredentialRepository, IOptions<FidoOptions> options, IOptions<IdServerHostOptions> idServerHostOptions)
        {
            _authenticationHelper = authenticationHelper;
            _userRepository = userRepository;
            _fido2 = fido2;
            _distributedCache = distributedCache;
            _userCredentialRepository=  userCredentialRepository;
            _options = options.Value;
            _idServerHostOptions = idServerHostOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatus([FromRoute] string prefix, string sessionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, nameof(sessionId)));
            var session = await _distributedCache.GetStringAsync(sessionId, cancellationToken);
            if (string.IsNullOrWhiteSpace(session)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.SESSION_CANNOT_BE_EXTRACTED);
            var sessionRecord = JsonSerializer.Deserialize<SessionRecord>(session);
            if (sessionRecord.IsValidated) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.REGISTRATION_NOT_CONFIRMED);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> BeginQRCode([FromRoute] string prefix, [FromBody] BeginU2FRegisterRequest request, CancellationToken cancellationToken)
        {
            var kvp = await CommonBegin(prefix, request, cancellationToken);
            if (kvp.Item2 != null) return kvp.Item2;
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(JsonSerializer.Serialize(kvp.Item1), QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var payload = qrCode.GetGraphic(20);
            return File(payload, "image/png");
        }

        [HttpPost]
        public async Task<IActionResult> Begin([FromRoute] string prefix, [FromBody] BeginU2FRegisterRequest request, CancellationToken cancellationToken)
        {
            var kvp = await CommonBegin(prefix, request, cancellationToken);
            if (kvp.Item2 != null) return kvp.Item2;
            return new OkObjectResult(kvp.Item1);
        }

        [HttpPost]
        public async Task<IActionResult> End([FromRoute] string prefix, [FromBody] EndU2FRegisterRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (request == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, IdServer.ErrorMessages.INVALID_INCOMING_REQUEST);
            if (string.IsNullOrWhiteSpace(request.SessionId)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, EndU2FRegisterRequestNames.SessionId));
            if (request.AuthenticatorAttestationRawResponse == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, EndU2FRegisterRequestNames.AuthenticatorAttestationRawResponse));
            if (string.IsNullOrWhiteSpace(request.Login)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, EndU2FRegisterRequestNames.Login));
            var login = request.Login;
            var user = await _authenticationHelper.GetUserByLogin(_userRepository.Query().Include(u => u.Credentials), login, prefix, cancellationToken);
            if (user != null)
                return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.USER_ALREADY_EXISTS, login));
            var session = await _distributedCache.GetStringAsync(request.SessionId, cancellationToken);
            if (string.IsNullOrWhiteSpace(session)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.SESSION_CANNOT_BE_EXTRACTED);

            if (user == null)
            {
                user = new Domains.User
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = login,
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow
                };
                if (_idServerHostOptions.IsEmailUsedDuringAuthentication) user.Email = login;
                user.Realms.Add(new RealmUser
                {
                    RealmsName = prefix
                });
                _userRepository.Add(user);
            }

            var sessionRecord = JsonSerializer.Deserialize<SessionRecord>(session);
            var success = await _fido2.MakeNewCredentialAsync(request.AuthenticatorAttestationRawResponse, sessionRecord.Options, async (arg, c) =>
            {
                var credentialId = Convert.ToBase64String(arg.CredentialId);
                var result = !(await _userCredentialRepository.Query().AnyAsync(c => c.CredentialType == Constants.CredentialType && credentialId == c.Id, cancellationToken));
                return result;
            }, cancellationToken: cancellationToken);

            user.AddFidoCredential(success.Result);
            sessionRecord.IsValidated = true;
            await _distributedCache.SetStringAsync(request.SessionId, JsonSerializer.Serialize(sessionRecord), new DistributedCacheEntryOptions
            {
                SlidingExpiration = _options.U2FExpirationTimeInSeconds
            }, cancellationToken);
            await _userRepository.SaveChanges(cancellationToken);
            return NoContent();
        }

        protected async Task<(BeginU2FRegisterResult, ContentResult)> CommonBegin(string prefix, BeginU2FRegisterRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (request == null) return (null, BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, IdServer.ErrorMessages.INVALID_INCOMING_REQUEST));
            if (string.IsNullOrWhiteSpace(request.Login)) return (null, BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, BeginU2FRegisterRequestNames.Login)));
            var login = request.Login;
            if (string.IsNullOrWhiteSpace(request.DisplayName)) return (null, BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, BeginU2FRegisterRequestNames.DisplayName)));
            var existingKeys = new List<PublicKeyCredentialDescriptor>();
            var user = await _authenticationHelper.GetUserByLogin(_userRepository.Query().Include(u => u.Credentials), login, prefix, cancellationToken);
            if (user != null)
                return (null, BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.USER_ALREADY_EXISTS, login)));

            var authenticatorSelection = new AuthenticatorSelection
            {
                UserVerification = UserVerificationRequirement.Preferred,
                ResidentKey = ResidentKeyRequirement.Discouraged
            };
            if (user != null) existingKeys = user.GetStoredFidoCredentials().Select(c => c.Descriptor).ToList();
            var fidoUser = new Fido2User
            {
                Name = request.DisplayName,
                Id = Encoding.UTF8.GetBytes(login)
            };
            var exts = new AuthenticationExtensionsClientInputs()
            {
                Extensions = true,
                UserVerificationMethod = true,
                DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs() { Attestation = "none" }
            };
            var options = _fido2.RequestNewCredential(fidoUser, existingKeys, authenticatorSelection, AttestationConveyancePreference.None, exts);
            var sessionId = Guid.NewGuid().ToString();
            var sessionRecord = new SessionRecord(options);
            await _distributedCache.SetStringAsync(sessionId, JsonSerializer.Serialize(sessionRecord), new DistributedCacheEntryOptions
            {
                SlidingExpiration = _options.U2FExpirationTimeInSeconds
            }, cancellationToken);
            return (new BeginU2FRegisterResult
            {
                CredentialCreateOptions = options,
                SessionId = sessionId
            }, null);
        }

        private record SessionRecord
        {
            public SessionRecord(CredentialCreateOptions options)
            {
                Options = options;
            }

            public string SerializedOptions { get; private set; }
            public bool IsValidated { get; set; } = false;

            public CredentialCreateOptions Options
            {
                get
                {
                    return CredentialCreateOptions.FromJson(SerializedOptions);
                }
                set
                {
                    if (value == null) return;
                    SerializedOptions = value.ToJson();
                }
            }
        }
    }
}
