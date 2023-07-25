// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib;
using Fido2NetLib.Objects;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.Webauthn.Extensions;
using SimpleIdServer.IdServer.Webauthn.UI.ViewModels;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Webauthn.UI
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseAuthenticationMethodController<AuthenticateWebauthnViewModel>
    {
        private const string fidoCookieName = "fido2.assertionOptions";
        private IFido2 _fido2;
        private readonly IAuthenticationHelper _authenticationHelper;

        public AuthenticateController(IFido2 fido2, 
            IAuthenticationHelper authenticationHelper, 
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IOptions<IdServerHostOptions> options, 
            IDataProtectionProvider dataProtectionProvider,
            IClientRepository clientRepository, 
            IAmrHelper amrHelper, 
            IUserRepository userRepository, 
            IUserTransformer userTransformer, 
            IBusControl busControl) : base(options, authenticationSchemeProvider, dataProtectionProvider, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _fido2 = fido2;
            _authenticationHelper = authenticationHelper;
        }

        protected override string Amr => Constants.AMR;

        protected override bool TryGetLogin(User user, out string login)
        {
            login = null;
            if (user == null) return false;
            var res = _authenticationHelper.GetLogin(user);
            if (string.IsNullOrWhiteSpace(res)) return false;
            login = res;
            return true;
        }

        protected override void EnrichViewModel(AuthenticateWebauthnViewModel viewModel, User user)
        {
            if (user != null && !user.GetStoredFidoCredentials().Any()) viewModel.IsFidoCredentialsMissing = true;
        }

        protected override bool IsExternalIdProvidersDisplayed => false;

        protected override async Task<ValidationStatus> ValidateCredentials(AuthenticateWebauthnViewModel viewModel, User user, CancellationToken cancellationToken)
        {
            string jsonOptions;
            if (!HttpContext.Request.Cookies.TryGetValue(fidoCookieName, out jsonOptions))
            {
                ModelState.AddModelError("options_cannot_be_extracted", "options_cannot_be_extracted");
                return ValidationStatus.NOCONTENT;
            }

            var authenticatorAssertionRawResponse = JsonSerializer.Deserialize<AuthenticatorAssertionRawResponse>(viewModel.SerializedAuthenticatorAssertionRawResponse);
            var options = AssertionOptions.FromJson(jsonOptions);
            var storedCredentials = user.GetStoredFidoCredentials();
            IsUserHandleOwnerOfCredentialIdAsync callback = (args, cancellationToken) =>
            {
                return Task.FromResult(storedCredentials.Any(c => c.Descriptor.Id.SequenceEqual(args.CredentialId)));
            };
            var fidoCredentials = user.GetFidoCredentials();
            var fidoCredential = fidoCredentials.First().GetFidoCredential();
            var credential = fidoCredentials.First(c => c.GetFidoCredential().Descriptor.Id.SequenceEqual(authenticatorAssertionRawResponse.Id));
            var creds = credential.GetFidoCredential();
            var storedCounter = creds.SignatureCounter;
            var res = await _fido2.MakeAssertionAsync(authenticatorAssertionRawResponse, options, creds.PublicKey, creds.DevicePublicKeys, storedCounter, callback, cancellationToken: cancellationToken);
            creds.SignCount = res.Counter;
            if (res.DevicePublicKey is not null)
                creds.DevicePublicKeys.Add(res.DevicePublicKey);

            credential.Value = JsonSerializer.Serialize(creds);
            await UserRepository.SaveChanges(cancellationToken);
            return ValidationStatus.AUTHENTICATE;
        }


        protected override async Task<User> AuthenticateUser(string login, string realm, CancellationToken cancellationToken)
        {
            var user = await _authenticationHelper.GetUserByLogin(UserRepository.Query()
                .Include(u => u.Realms)
                .Include(u => u.IdentityProvisioning).ThenInclude(i => i.Properties)
                .Include(u => u.Groups)
                .Include(c => c.OAuthUserClaims)
                .Include(u => u.Credentials), login, realm, cancellationToken);
            return user;
        }

        [HttpPost]
        public async Task<IActionResult> MakeAssertionOptions([FromRoute] string prefix, [FromBody] MakeAssertionOptionsViewModel request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (request == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, IdServer.ErrorMessages.INVALID_INCOMING_REQUEST);
            if (string.IsNullOrWhiteSpace(request.Login)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, "login"));
            var amrInfo = GetAmrInfo();
            var authenticatedUser = await FetchAuthenticatedUser(prefix, amrInfo, cancellationToken);
            if (authenticatedUser == null)
            {
                authenticatedUser = await _authenticationHelper.GetUserByLogin(UserRepository.Query().Include(u => u.Credentials), request.Login, prefix, cancellationToken);
                if(authenticatedUser == null) return BuildError(System.Net.HttpStatusCode.Unauthorized, ErrorCodes.ACCESS_DENIED, string.Format(IdServer.ErrorMessages.UNKNOWN_USER, request.Login));
            }

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
            HttpContext.Response.Cookies.Append(fidoCookieName, options.ToJson());
            return Json(options);
        }
    }
}
