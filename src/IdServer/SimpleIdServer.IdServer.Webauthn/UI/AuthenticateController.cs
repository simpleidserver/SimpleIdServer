// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib;
using Fido2NetLib.Objects;
using MassTransit;
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
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Webauthn.UI
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseAuthenticateController
    {
        private const string fidoCookieName = "fido2.assertionOptions";
        private readonly IAuthenticationHelper _authenticationHelper;
        private IFido2 _fido2;

        public AuthenticateController(
            IAuthenticationHelper authenticationHelper,
            IFido2 fido2,
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
        }


        public async Task<IActionResult> Index([FromRoute] string prefix, string returnUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });

            try
            {
                prefix = prefix ?? IdServer.Constants.DefaultRealm;
                var query = ExtractQuery(returnUrl);
                var clientId = query.GetClientIdFromAuthorizationRequest();
                var client = await ClientRepository.Query().Include(c => c.Translations).Include(c => c.Realms).FirstOrDefaultAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == prefix), cancellationToken);
                var amrInfo = await ResolveAmrInfo(query, prefix, client, cancellationToken);
                var authenticatedUser = await FetchAuthenticatedUser(prefix, amrInfo, cancellationToken);
                var loginHint = query.GetLoginHintFromAuthorizationRequest();
                bool isFidoCredentialsMissing = false;
                if (authenticatedUser != null && !authenticatedUser.GetStoredFidoCredentials().Any())
                    isFidoCredentialsMissing = true;
                else if (authenticatedUser != null && authenticatedUser.GetStoredFidoCredentials().Any()) loginHint = _authenticationHelper.GetLogin(authenticatedUser);
                return View(new AuthenticateWebauthnViewModel(returnUrl,
                    prefix,
                    loginHint,
                    client.ClientName,
                    client.LogoUri,
                    client.TosUri,
                    client.PolicyUri,
                    isFidoCredentialsMissing,
                    authenticatedUser != null,
                    amrInfo));

            }
            catch
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromRoute] string prefix, [FromBody] SubmitAuthenticateWebauthnViewModel assertion, CancellationToken cancellationToken)
        {
            if (assertion == null) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, IdServer.ErrorMessages.INVALID_INCOMING_REQUEST);
            string jsonOptions;
            if (!HttpContext.Request.Cookies.TryGetValue(fidoCookieName, out jsonOptions)) return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.OPTIONS_CANNOT_BE_EXTRACTED);
            var authenticatorAssertionRawResponse = JsonSerializer.Deserialize<AuthenticatorAssertionRawResponse>(assertion.SerializedAuthenticatorAssertionRawResponse);
            var options = AssertionOptions.FromJson(jsonOptions);
            var returnUrl = assertion.ReturnUrl;
            var amrInfo = GetAmrInfo();
            var authenticatedUser = await FetchAuthenticatedUser(prefix, amrInfo, cancellationToken);
            if (authenticatedUser == null)
            {
                authenticatedUser = await _authenticationHelper.GetUserByLogin(UserRepository.Query().Include(u => u.Credentials), assertion.Login, prefix, cancellationToken);
                if (authenticatedUser == null) return BuildError(System.Net.HttpStatusCode.Unauthorized, ErrorCodes.ACCESS_DENIED, string.Format(IdServer.ErrorMessages.UNKNOWN_USER, assertion.Login));
            }

            var storedCredentials = authenticatedUser.GetStoredFidoCredentials();
            IsUserHandleOwnerOfCredentialIdAsync callback = (args, cancellationToken) =>
            {
                return Task.FromResult(storedCredentials.Any(c => c.Descriptor.Id.SequenceEqual(args.CredentialId)));
            };
            var fidoCredentials = authenticatedUser.GetFidoCredentials();
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
            return await Authenticate(prefix, assertion.ReturnUrl, Constants.AMR, authenticatedUser, cancellationToken, assertion.RememberLogin);
        }
    }
}
