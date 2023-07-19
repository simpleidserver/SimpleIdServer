// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Webauthn.Models;
using SimpleIdServer.IdServer.Webauthn.UI.ViewModels;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Webauthn.UI
{
    [Area(Constants.AMR)]
    public class RegisterController : BaseController
    {
        private const string fidoCookieName = "fido2.attestationOptions";
        private readonly IdServerHostOptions _options;
        private readonly IFido2 _fido2;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IUserRepository _userRepository;
        private readonly IUserCredentialRepository _userCredentialRepository;

        public RegisterController(IOptions<IdServerHostOptions> options, IFido2 fido2, IAuthenticationHelper authenticationHelper, IUserRepository userRepository, IUserCredentialRepository userCredentialRepository)
        {
            _options = options.Value;
            _fido2= fido2;
            _authenticationHelper = authenticationHelper;
            _userRepository = userRepository;
            _userCredentialRepository = userCredentialRepository;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> MakeCredentialsOptions([FromRoute] string prefix, [FromBody] RegisterWebauthnViewModel request, CancellationToken cancellationToken)
        {
            if (request == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, IdServer.ErrorMessages.INVALID_INCOMING_REQUEST);
            if (string.IsNullOrWhiteSpace(request.Login)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, nameof(RegisterWebauthnViewModel.Login)));

            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            var authenticatorSelection = new AuthenticatorSelection
            {
                UserVerification = UserVerificationRequirement.Preferred,
                ResidentKey = ResidentKeyRequirement.Discouraged 
            };
            var existingKeys = new List<PublicKeyCredentialDescriptor>();
            var user = await _authenticationHelper.GetUserByLogin(_userRepository.Query().Include(u => u.Credentials), request.Login, prefix, cancellationToken);
            if(user != null) existingKeys = user.Credentials.Where(c => c.CredentialType == Constants.CredentialType).Select(c => JsonSerializer.Deserialize<StoredFidoCredential>(c.Value)).Select(c => c.Descriptor).ToList();
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
        public async Task<IActionResult> MakeCredential([FromRoute] string prefix, [FromBody] AuthenticatorAttestationRawResponse attestationResponse, CancellationToken cancellationToken)
        {
            if (attestationResponse == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, IdServer.ErrorMessages.INVALID_INCOMING_REQUEST);
            string jsonOptions;
            if (!HttpContext.Request.Cookies.TryGetValue(fidoCookieName, out jsonOptions)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.OPTIONS_CANNOT_BE_EXTRACTED);

            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            var options = CredentialCreateOptions.FromJson(jsonOptions);
            var success = await _fido2.MakeNewCredentialAsync(attestationResponse, options, async (arg, c) =>
            {
                var credentialId = Convert.ToBase64String(arg.CredentialId);
                return !(await _userCredentialRepository.Query().AnyAsync(c => c.CredentialType == Constants.CredentialType && credentialId == c.Id, cancellationToken));
            }, cancellationToken: cancellationToken);

            var name = options.User.Name;
            var user = await _authenticationHelper.GetUserByLogin(_userRepository.Query().Include(u => u.Credentials), name, prefix, cancellationToken);
            if(user == null)
            {
                user = new Domains.User
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow
                };
                if(_options.IsEmailUsedDuringAuthentication) user.Email = name;
                user.Realms.Add(new RealmUser
                {
                    RealmsName = prefix
                });
                _userRepository.Add(user);
            }

            user.AddFidoCredential(success.Result);
            await _userRepository.SaveChanges(cancellationToken);
            return Json(success);
        }
    }
}
