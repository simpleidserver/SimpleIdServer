// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Saml.Idp.Factories;
using SimpleIdServer.IdServer.Store;

namespace SimpleIdServer.IdServer.Saml2.Api
{
    public class SamlSSOController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly ISaml2ConfigurationFactory _saml2ConfigurationFactory;

        public SamlSSOController(IClientRepository clientRepository, ISaml2ConfigurationFactory saml2ConfigurationFactory)
        {
            _clientRepository = clientRepository;
            _saml2ConfigurationFactory = saml2ConfigurationFactory;
        }

        [HttpGet]
        public async Task<IActionResult> LoginGet([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            // Read the saml request.
            var requestBinding = new Saml2RedirectBinding();
            var deserializedHttpRequest = Request.ToGenericHttpRequest();
            var saml2AuthnRequest = requestBinding.ReadSamlRequest(deserializedHttpRequest, new Saml2AuthnRequest(new Saml2Configuration()));
            var client = await _clientRepository.Query()
                .Include(c => c.Realms)
                .Include(c => c.SerializedJsonWebKeys)
                .Include(c => c.SerializedParameters)
                .AsNoTracking().SingleOrDefaultAsync(c => c.ClientId == saml2AuthnRequest.Issuer && c.Realms.Any(r => r.Name == prefix), cancellationToken);
            if (client == null) throw new Saml2BindingException($"the client '{saml2AuthnRequest.Issuer}' doesn't exist");

            var spSamlConfiguration = _saml2ConfigurationFactory.BuildSamSpConfiguration(client);
            try
            {
                requestBinding.Unbind(deserializedHttpRequest, saml2AuthnRequest);
                return null;
            }
            catch(Exception)
            {
                return BuildPostResponse(saml2AuthnRequest, Saml2StatusCodes.Responder, requestBinding.RelayState, issuer, prefix, client);
            }
        }

        private IActionResult BuildPostResponse(Saml2Request request, Saml2StatusCodes status, string relayState, string issuer, string realm, Client client)
        {
            var idpSamlConfiguration = _saml2ConfigurationFactory.BuildSamlIdpConfiguration(issuer, realm);
            var responsebinding = new Saml2PostBinding
            {
                RelayState = relayState
            };
            var response = new Saml2AuthnResponse(idpSamlConfiguration)
            {
                InResponseTo = request.Id,
                Status = status,
                Destination = null
            };
            // client => destination !!!
            return responsebinding.Bind(response).ToActionResult();
        }

        // Load metadata from RP !!
        /*
         * 
        private async Task LoadRelyingPartyAsync(RelyingParty rp, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                // Load RP if not already loaded.
                if (string.IsNullOrEmpty(rp.Issuer))
                {
                    var entityDescriptor = new EntityDescriptor();
                    await entityDescriptor.ReadSPSsoDescriptorFromUrlAsync(httpClientFactory, new Uri(rp.Metadata), cancellationTokenSource.Token);
                    if (entityDescriptor.SPSsoDescriptor != null)
                    {
                        rp.Issuer = entityDescriptor.EntityId;
                        rp.AcsDestination = entityDescriptor.SPSsoDescriptor.AssertionConsumerServices.Where(a => a.IsDefault).OrderBy(a => a.Index).First().Location;
                        var singleLogoutService = entityDescriptor.SPSsoDescriptor.SingleLogoutServices.First();
                        rp.SingleLogoutDestination = singleLogoutService.ResponseLocation ?? singleLogoutService.Location;
                        rp.SignatureValidationCertificate = entityDescriptor.SPSsoDescriptor.SigningCertificates.First();
                    }
                    else
                    {
                        throw new Exception($"SPSsoDescriptor not loaded from metadata '{rp.Metadata}'.");
                    }
                }
            }
            catch (Exception exc)
            {
                //log error
#if DEBUG
                Debug.WriteLine($"SPSsoDescriptor error: {exc.ToString()}");
#endif
            }
        }
        */
    }
}
