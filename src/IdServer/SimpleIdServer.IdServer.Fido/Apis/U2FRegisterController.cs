// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QRCoder;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Fido.DTOs;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI;
using System.Security.Claims;
using System.Text;

namespace SimpleIdServer.IdServer.Fido.Apis
{
    public class U2FRegisterController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IUserRepository _userRepository;
        private readonly IFido2 _fido2;
        private readonly IDistributedCache _distributedCache;
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IEnumerable<IAuthenticationMethodService> _authenticationMethodServices;
        private readonly IWorkflowHelper _workflowHelper;
        private readonly IdServerHostOptions _idServerHostOptions;

        public U2FRegisterController(
            IConfiguration configuration,
            IAuthenticationHelper authenticationHelper,
            IUserRepository userRepository,
            IFido2 fido2,
            IDistributedCache distributedCache,
            ITransactionBuilder transactionBuilder,
            IEnumerable<IAuthenticationMethodService> authenticationMethodServices,
            IWorkflowHelper workflowHelper,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder,
            IOptions<IdServerHostOptions> idServerHostOptions) : base(tokenRepository, jwtBuilder)
        {
            _configuration = configuration;
            _authenticationHelper = authenticationHelper;
            _userRepository = userRepository;
            _fido2 = fido2;
            _distributedCache = distributedCache;
            _transactionBuilder = transactionBuilder;
            _authenticationMethodServices = authenticationMethodServices;
            _workflowHelper = workflowHelper;
            _idServerHostOptions = idServerHostOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatus([FromRoute] string prefix, string sessionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, nameof(sessionId)));
            var session = await _distributedCache.GetStringAsync(sessionId, cancellationToken);
            if (string.IsNullOrWhiteSpace(session)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Resources.Global.SessionCannotBeExtracted);
            var sessionRecord = System.Text.Json.JsonSerializer.Deserialize<RegistrationSessionRecord>(session);
            if (!sessionRecord.IsValidated) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Resources.Global.RegistrationNotConfirmed);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> BeginQRCode([FromRoute] string prefix, [FromBody] BeginU2FRegisterRequest request, CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            var kvp = await CommonBegin(prefix, request, cancellationToken);
            if (kvp.Item2 != null) return kvp.Item2;
            var qrGenerator = new QRCodeGenerator();
            var json = System.Text.Json.JsonSerializer.Serialize(new QRCodeResult
            {
                Action = "register",
                SessionId = kvp.Item1.SessionId,
                ReadQRCodeURL = $"{issuer}/{prefix}/{Constants.EndPoints.ReadRegisterQRCode}/{kvp.Item1.SessionId}"
            });
            var qrCodeData = qrGenerator.CreateQrCode(json, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var payload = qrCode.GetGraphic(20);
            Response.Headers.Add("SessionId", kvp.Item1.SessionId);
            Response.Headers.Add("QRCode", json);
            if (!string.IsNullOrWhiteSpace(kvp.Item1.NextRegistrationRedirectUrl)) Response.Headers.Add("NextRegistrationRedirectUrl", kvp.Item1.NextRegistrationRedirectUrl);
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
            var sessionRecord = System.Text.Json.JsonSerializer.Deserialize<RegistrationSessionRecord>(session);
            return new OkObjectResult(new BeginU2FRegisterResult
            {
                CredentialCreateOptions = sessionRecord.Options,
                SessionId = sessionId,
                EndRegisterUrl = $"{issuer}/{prefix}/{Constants.EndPoints.EndRegister}",
                Login = sessionRecord.Login
            });
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
            var transaction = _transactionBuilder.Build();
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (request == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
            if (string.IsNullOrWhiteSpace(request.SessionId)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, EndU2FRegisterRequestNames.SessionId));
            if (request.AuthenticatorAttestationRawResponse == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, EndU2FRegisterRequestNames.AuthenticatorAttestationRawResponse));
            var session = await _distributedCache.GetStringAsync(request.SessionId, cancellationToken);
            if (string.IsNullOrWhiteSpace(session)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Resources.Global.SessionCannotBeExtracted);
            var sessionRecord = System.Text.Json.JsonSerializer.Deserialize<RegistrationSessionRecord>(session);
            var fidoOptions = GetOptions(sessionRecord.CredentialType);
            var login = request.Login;
            var registrationProgress = await GetRegistrationProgress(sessionRecord);
            if (registrationProgress == null) return BuildError(System.Net.HttpStatusCode.Unauthorized, ErrorCodes.INVALID_REQUEST, Resources.Global.NotAllowedToRegister);
            if (string.IsNullOrWhiteSpace(request.Login)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, EndU2FRegisterRequestNames.Login));
            var user = await _authenticationHelper.GetUserByLogin(login, prefix, cancellationToken);
            bool isNewUser = false;
            if (user == null)
            {
                user = BuildUser();
                isNewUser = true;
            }

            var success = await _fido2.MakeNewCredentialAsync(request.AuthenticatorAttestationRawResponse, sessionRecord.Options, async (arg, c) =>
            {
                return true;
            }, cancellationToken: cancellationToken);

            user.AddFidoCredential(sessionRecord.CredentialType, success.Result);
            if (request.DeviceData != null)
            {
                user.Devices.Add(new UserDevice
                {
                    CreateDateTime = DateTime.UtcNow,
                    DeviceType = request.DeviceData.DeviceType,
                    Id = Guid.NewGuid().ToString(),
                    Manufacturer = request.DeviceData.Manufacturer,
                    Model = request.DeviceData.Model,
                    Name = request.DeviceData.Name,
                    PushToken = request.DeviceData.PushToken,
                    PushType = request.DeviceData.PushType,
                    Version = request.DeviceData.Version
                });
                user.NotificationMode = request.DeviceData.PushType ?? "console";
            }

            sessionRecord.IsValidated = true;
            await _distributedCache.SetStringAsync(request.SessionId, System.Text.Json.JsonSerializer.Serialize(sessionRecord), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(fidoOptions.U2FExpirationTimeInSeconds)
            }, cancellationToken);

            return await HandleWorkflowRegistration();

            User BuildUser()
            {
                var result = registrationProgress.User;
                if (result == null)
                {
                    result = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = login,
                        UpdateDateTime = DateTime.UtcNow,
                        CreateDateTime = DateTime.UtcNow
                    };
                    if (_idServerHostOptions.IsEmailUsedDuringAuthentication) user.Email = login;
                }

                return result;
            }

            async Task<IActionResult> HandleWorkflowRegistration()
            {
                var nextAmr = await _workflowHelper.GetNextAmr(registrationProgress.WorkflowId, sessionRecord.CredentialType, cancellationToken);
                if (WorkflowHelper.IsLastStep(nextAmr))
                {
                    if(isNewUser)
                    {
                        user.Realms.Add(new RealmUser
                        {
                            RealmsName = prefix
                        });
                        _userRepository.Add(user);
                    }
                    else
                    {
                        _userRepository.Update(user);
                    }

                    await transaction.Commit(cancellationToken);
                    return new OkObjectResult(new EndU2FRegisterResult
                    {
                        Sig = success.Result.SignCount
                    });
                }

                registrationProgress.User = user;
                await _distributedCache.SetStringAsync(registrationProgress.RegistrationProgressId, Newtonsoft.Json.JsonConvert.SerializeObject(registrationProgress));
                return new OkObjectResult(new EndU2FRegisterResult
                {
                    Sig = success.Result.SignCount
                });
            }
        }

        protected async Task<(BeginU2FRegisterResult, ContentResult)> CommonBegin(string prefix, BeginU2FRegisterRequest request, CancellationToken cancellationToken)
        {
            var cookieName = _idServerHostOptions.GetRegistrationCookieName();
            var cookieValue = string.Empty;
            if(Request.Cookies.ContainsKey(cookieName)) cookieValue = Request.Cookies[cookieName];
            var fidoOptions = GetOptions(request.CredentialType);
            var isAuthenticated = User.Identity.IsAuthenticated;
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (request == null) return (null, BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest));
            var login = request.Login;
            if (isAuthenticated) login = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrWhiteSpace(login)) return (null, BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, BeginU2FRegisterRequestNames.Login)));
            if (string.IsNullOrWhiteSpace(request.DisplayName)) return (null, BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, BeginU2FRegisterRequestNames.DisplayName)));
            var existingKeys = new List<PublicKeyCredentialDescriptor>();
            var user = await _authenticationHelper.GetUserByLogin(login, prefix, cancellationToken);
            if (user != null && !isAuthenticated)
                return (null, BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UserAlreadyExists, login)));

            var authenticatorSelection = new AuthenticatorSelection
            {
                UserVerification = UserVerificationRequirement.Preferred,
                ResidentKey = ResidentKeyRequirement.Discouraged
            };
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
            var sessionRecord = new RegistrationSessionRecord(options, request.Login, request.CredentialType, cookieValue);
            await _distributedCache.SetStringAsync(sessionId, System.Text.Json.JsonSerializer.Serialize(sessionRecord), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(fidoOptions.U2FExpirationTimeInSeconds)
            }, cancellationToken);
            string nextRegistrationRedirectUrl = null;
            var registrationProgress = await GetRegistrationProgress();
            string nextAmr = null;
            if (registrationProgress != null) nextAmr = await _workflowHelper.GetNextAmr(registrationProgress.WorkflowId, request.CredentialType, cancellationToken);
            if (registrationProgress != null && !WorkflowHelper.IsLastStep(nextAmr))
            {
                nextRegistrationRedirectUrl = $"{issuer}/{prefix}/{nextAmr}/register";
            }

            return (new BeginU2FRegisterResult
            {
                CredentialCreateOptions = options,
                SessionId = sessionId,
                EndRegisterUrl = $"{issuer}/{prefix}/{Constants.EndPoints.EndRegister}",
                Login = request.Login,
                NextRegistrationRedirectUrl = nextRegistrationRedirectUrl
            }, null);
        }

        private IFidoOptions GetOptions(string credentialType)
        {
            var authenticationMethodService = _authenticationMethodServices.Single(a => a.Amr == credentialType);
            var section = _configuration.GetSection(authenticationMethodService.OptionsType.Name);
            return section.Get(authenticationMethodService.OptionsType) as IFidoOptions;
        }

        private record RegistrationSessionRecord
        {
            public RegistrationSessionRecord(CredentialCreateOptions options, string login, string credentialType, string registrationCookieKey)
            {
                Options = options;
                Login = login;
                CredentialType = credentialType;
                RegistrationCookieKey = registrationCookieKey;
            }

            public string SerializedOptions { get; private set; }
            public bool IsValidated { get; set; } = false;
            public string Login { get; set; } = null!;
            public string CredentialType { get; set; }
            public string RegistrationCookieKey { get; set; }

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

        private async Task<UserRegistrationProgress> GetRegistrationProgress(RegistrationSessionRecord sessionRecord = null)
        {
            var cookieValue = string.Empty;
            var cookieName = _idServerHostOptions.GetRegistrationCookieName();
            if (!Request.Cookies.ContainsKey(cookieName))
            {
                if (sessionRecord == null || string.IsNullOrWhiteSpace(sessionRecord.RegistrationCookieKey)) return null;
                cookieValue = sessionRecord.RegistrationCookieKey;
            }
            else cookieValue = Request.Cookies[cookieName];
            var json = await _distributedCache.GetStringAsync(cookieValue);
            if (string.IsNullOrWhiteSpace(json)) return null;
            var registrationProgress = JsonConvert.DeserializeObject<UserRegistrationProgress>(json);
            return registrationProgress;
        }
    }
}
