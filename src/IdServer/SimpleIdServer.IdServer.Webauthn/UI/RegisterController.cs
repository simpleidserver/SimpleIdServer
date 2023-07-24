// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib;
using Fido2NetLib.Objects;
using MassTransit;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.Webauthn.UI.ViewModels;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Webauthn.UI
{
    [Area(Constants.AMR)]
    public class RegisterController : BaseAuthenticateController
    {
        private const string fidoCookieName = "fido2.attestationOptions";
        private readonly IFido2 _fido2;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IUserCredentialRepository _userCredentialRepository;

        public RegisterController(
            IFido2 fido2,
            IAuthenticationHelper authenticationHelper,
            IUserCredentialRepository userCredentialRepository,
            IOptions<IdServerHostOptions> options,
            IDataProtectionProvider dataProtectionProvider,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository,
            IUserTransformer userTransformer,
            IBusControl busControl) : base(options, dataProtectionProvider, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _fido2 = fido2;
            _authenticationHelper = authenticationHelper;
            _userCredentialRepository = userCredentialRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            var amrInfo = GetAmrInfo();
            var authenticatedUser = await FetchAuthenticatedUser(prefix, amrInfo, cancellationToken);
            string loginHint = null;
            if(authenticatedUser != null) loginHint = _authenticationHelper.GetLogin(authenticatedUser);
            return View(new RegisterWebauthnViewModel { Login = loginHint });
        }

        [HttpPost]
        public async Task<IActionResult> MakeCredentialsOptions([FromRoute] string prefix, [FromBody] RegisterWebauthnViewModel request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (request == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, IdServer.ErrorMessages.INVALID_INCOMING_REQUEST);
            if (string.IsNullOrWhiteSpace(request.Login)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, nameof(RegisterWebauthnViewModel.Login)));
            var existingKeys = new List<PublicKeyCredentialDescriptor>();
            var amrInfo = GetAmrInfo();
            var user = await FetchAuthenticatedUser(prefix, amrInfo, cancellationToken);
            if (user == null)
            {
                user = await _authenticationHelper.GetUserByLogin(UserRepository.Query().Include(u => u.Credentials), request.Login, prefix, cancellationToken);
                if (user != null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.USER_ALREADY_EXISTS, request.Login));
            }

            var authenticatorSelection = new AuthenticatorSelection
            {
                UserVerification = UserVerificationRequirement.Preferred,
                ResidentKey = ResidentKeyRequirement.Discouraged 
            };
            if (user != null) existingKeys = user.GetStoredFidoCredentials().Select(c => c.Descriptor).ToList();
            var fidoUser = new Fido2User
            {
                Name = request.Login,
                Id = Encoding.UTF8.GetBytes(request.Login)
            };
            var exts = new AuthenticationExtensionsClientInputs()
            {
                Extensions = true,
                UserVerificationMethod = true,
                DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs() { Attestation = "none" }
            };
            var options = _fido2.RequestNewCredential(fidoUser, existingKeys, authenticatorSelection, AttestationConveyancePreference.None, exts);
            HttpContext.Response.Cookies.Append(fidoCookieName, options.ToJson());
            return Json(options);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeCredential([FromRoute] string prefix, [FromBody] RegisterWebauthnViewModel viewModel, CancellationToken cancellationToken)
        {
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (viewModel == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, IdServer.ErrorMessages.INVALID_INCOMING_REQUEST);
            string jsonOptions;
            if (!HttpContext.Request.Cookies.TryGetValue(fidoCookieName, out jsonOptions)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.OPTIONS_CANNOT_BE_EXTRACTED);
            if (string.IsNullOrWhiteSpace(viewModel.Login)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, "login"));
            if (string.IsNullOrWhiteSpace(viewModel.SerializedAuthenticatorAttestationRawResponse)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, "serializedAuthenticatorAttestationRawResponse"));

            var amrInfo = GetAmrInfo();
            var user = await FetchAuthenticatedUser(prefix, amrInfo, cancellationToken);
            if (user == null)
            {
                user = await _authenticationHelper.GetUserByLogin(UserRepository.Query().Include(u => u.Credentials), viewModel.Login, prefix, cancellationToken);
                if (user != null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.USER_ALREADY_EXISTS, viewModel.Login));

                user = new Domains.User
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = viewModel.Login,
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow
                };
                if (Options.IsEmailUsedDuringAuthentication) user.Email = viewModel.Login;
                user.Realms.Add(new RealmUser
                {
                    RealmsName = prefix
                });
                UserRepository.Add(user);
            }

            var attestationResponse = JsonSerializer.Deserialize<AuthenticatorAttestationRawResponse>(viewModel.SerializedAuthenticatorAttestationRawResponse);
            var options = CredentialCreateOptions.FromJson(jsonOptions);
            var success = await _fido2.MakeNewCredentialAsync(attestationResponse, options, async (arg, c) =>
            {
                var credentialId = Convert.ToBase64String(arg.CredentialId);
                var result = !(await _userCredentialRepository.Query().AnyAsync(c => c.CredentialType == Constants.CredentialType && credentialId == c.Id, cancellationToken));
                return result;
            }, cancellationToken: cancellationToken);

            user.AddFidoCredential(success.Result);
            await UserRepository.SaveChanges(cancellationToken);
            return NoContent();
        }
    }
}
