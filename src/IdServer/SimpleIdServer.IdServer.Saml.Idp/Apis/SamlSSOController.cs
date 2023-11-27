// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Saml2;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Extractors;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Saml.Idp.DTOs;
using SimpleIdServer.IdServer.Saml.Idp.Extensions;
using SimpleIdServer.IdServer.Saml.Idp.Factories;
using SimpleIdServer.IdServer.Store;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SimpleIdServer.IdServer.Saml2.Api
{
    public class SamlSSOController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly ISaml2ConfigurationFactory _saml2ConfigurationFactory;
        private readonly IDataProtector _dataProtector;
        private readonly IScopeClaimsExtractor _scopeClaimsExtractor;
        private readonly IDistributedCache _distributedCache;
        private readonly IUserRepository _userRepository;
        private readonly IdServerHostOptions _options;
        private readonly ILogger<SamlSSOController> _logger;

        public SamlSSOController(
            IClientRepository clientRepository, 
            ISaml2ConfigurationFactory saml2ConfigurationFactory, 
            IDataProtectionProvider dataProtectionProvider, 
            IScopeClaimsExtractor scopeClaimsExtractor, 
            IDistributedCache distributedCache,
            IUserRepository userRepository,
            IOptions<IdServerHostOptions> options,
            ILogger<SamlSSOController> logger)
        {
            _clientRepository = clientRepository;
            _saml2ConfigurationFactory = saml2ConfigurationFactory;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _scopeClaimsExtractor = scopeClaimsExtractor;
            _distributedCache = distributedCache;
            _userRepository = userRepository;
            _options = options.Value;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> LoginGet([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var requestBinding = new Saml2RedirectBinding();
            var deserializedHttpRequest = Request.ToGenericHttpRequest();
            ClientResult clientResult = null;
            var requestedIssuer = requestBinding.ReadSamlRequest(deserializedHttpRequest, new Saml2AuthnRequest(new Saml2Configuration())).Issuer;
            try
            {
                clientResult = await GetClient(requestedIssuer, prefix, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(ex);
            }

            if (User == null || User.Identity == null || User.Identity.IsAuthenticated == false)
                return RedirectToLoginPage();
             
            Saml2AuthnRequest saml2AuthnRequest = new Saml2AuthnRequest(clientResult.SpSamlConfiguration);
            try
            {
                requestBinding.Unbind(deserializedHttpRequest, saml2AuthnRequest);
                return await BuildLoginResponse(saml2AuthnRequest, clientResult, Saml2StatusCodes.Success, requestBinding.RelayState, issuer, prefix, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return await BuildLoginResponse(saml2AuthnRequest, clientResult, Saml2StatusCodes.Responder, requestBinding.RelayState, issuer, prefix, cancellationToken);
            }

            IActionResult RedirectToLoginPage()
            {
                var queryStr = Request.QueryString.Value.TrimStart('?');
                var returnUrl = $"{issuer}/{prefix}/{Saml.Idp.Constants.RouteNames.SingleSignOnHttpRedirect}?{AuthorizationRequestParameters.ClientId}={clientResult.Client.ClientId}&{queryStr}";
                var url = Url.Action("Index", "Authenticate", new
                {
                    returnUrl = _dataProtector.Protect(returnUrl),
                    area = Constants.Areas.Password
                });
                return Redirect(url);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoginArtifact([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var soapEnvelope = new Saml2SoapEnvelope();
            var httpRequest = await Request.ToGenericHttpRequestAsync(readBodyAsString: true);
            var issuer = soapEnvelope.ReadSamlRequest(httpRequest, new Saml2ArtifactResolve(new Saml2Configuration())).Issuer;
            ClientResult clientResult = null;
            try
            {
                clientResult = await GetClient(issuer, prefix, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(ex);
            }

            var saml2ArtifactResolve = new Saml2ArtifactResolve(clientResult.SpSamlConfiguration);
            try
            {
                var idpSamlConfiguration = _saml2ConfigurationFactory.BuildSamlIdpConfiguration(Request.GetAbsoluteUriWithVirtualPath(), Request.GetAbsoluteUriWithVirtualPath(), prefix);
                soapEnvelope.Unbind(httpRequest, saml2ArtifactResolve);
                var base64 = await _distributedCache.GetStringAsync(saml2ArtifactResolve.Artifact, cancellationToken);
                if (string.IsNullOrWhiteSpace(base64)) throw new OAuthException(string.Empty, $"Saml2AuthnResponse not found by Artifact '{saml2ArtifactResolve.Artifact}' in the cache.");
                await _distributedCache.RemoveAsync(saml2ArtifactResolve.Artifact, cancellationToken);
                var cachedSaml2AuthnResponse = ReadAuthnResponse(base64);
                var saml2ArtifactResponse = new Saml2ArtifactResponse(idpSamlConfiguration, cachedSaml2AuthnResponse)
                {
                    InResponseTo = saml2ArtifactResolve.Id
                };
                soapEnvelope.Bind(saml2ArtifactResponse);
                return soapEnvelope.ToActionResult();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(ex);
            }

            Saml2AuthnResponse ReadAuthnResponse(string base64)
            {
                var binding = new Saml2PostBinding();
                var saml2AuthnResponse = new Saml2AuthnResponse(new Saml2Configuration
                {
                    AudienceRestricted = false
                });
                var fakeHttpRequest = new ITfoxtec.Identity.Saml2.Http.HttpRequest
                {
                    Method = "POST",
                    Form = new System.Collections.Specialized.NameValueCollection
                    {
                        { "SAMLResponse", base64 }
                    }
                };
                var req = binding.ReadSamlResponse(fakeHttpRequest, saml2AuthnResponse);
                return req as Saml2AuthnResponse;
            }
        }

        private async Task<ClientResult> GetClient(string issuer, string realm, CancellationToken cancellationToken)
        {
            var client = await _clientRepository.Query()
                .Include(c => c.Realms)
                .Include(c => c.Scopes).ThenInclude(s => s.ClaimMappers)
                .Include(c => c.SerializedJsonWebKeys)
                .AsNoTracking().SingleOrDefaultAsync(c => c.ClientId == issuer && c.Realms.Any(r => r.Name == realm), cancellationToken);
            if (client == null) throw new OAuthException(string.Empty, $"the client '{issuer}' doesn't exist");
            var kvp = await _saml2ConfigurationFactory.BuildSamSpConfiguration(client, cancellationToken);
            return new ClientResult { Client = client, SpSamlConfiguration = kvp.Item1, EntityDescriptor = kvp.Item2 };
        }

        private Task<IActionResult> BuildLoginResponse(Saml2Request request, ClientResult clientResult, Saml2StatusCodes status, string relayState, string issuer, string realm, CancellationToken cancellationToken)
        {
            if (clientResult.Client.GetUseAcrsArtifact()) return BuildArtifactLoginResponse(request, clientResult, status, relayState, issuer, realm, cancellationToken);
            else return BuildPostLoginResponse(request, clientResult, status, relayState, issuer, realm, cancellationToken);
        }

        private async Task<IActionResult> BuildPostLoginResponse(Saml2Request request, ClientResult clientResult, Saml2StatusCodes status, string relayState, string issuer, string realm, CancellationToken cancellationToken)
        {
            var destination = clientResult.EntityDescriptor.SPSsoDescriptor.AssertionConsumerServices.Where(a => a.IsDefault && a.Binding == ProtocolBindings.HttpPost).OrderBy(a => a.Index).First().Location;
            var idpSamlConfiguration = _saml2ConfigurationFactory.BuildSamlIdpConfiguration(issuer, issuer, realm);
            var responsebinding = new Saml2PostBinding
            {
                RelayState = relayState
            };
            var response = await BuildAuthnSamlResponse(request, clientResult, idpSamlConfiguration, status, destination, issuer, realm, cancellationToken);
            return responsebinding.Bind(response).ToActionResult();
        }

        private async Task<IActionResult> BuildArtifactLoginResponse(Saml2Request request, ClientResult clientResult, Saml2StatusCodes status, string relayState, string issuer, string realm, CancellationToken cancellationToken)
        {
            var destination = clientResult.EntityDescriptor.SPSsoDescriptor.AssertionConsumerServices.Where(a => a.IsDefault && a.Binding == ProtocolBindings.HttpArtifact).OrderBy(a => a.Index).First().Location;
            var idpSamlConfiguration = _saml2ConfigurationFactory.BuildSamlIdpConfiguration(issuer, issuer, realm);
            var responsebinding = new Saml2ArtifactBinding
            {
                RelayState = realm
            };
            var saml2ArtifactResolve = new Saml2ArtifactResolve(idpSamlConfiguration)
            {
                Destination = destination
            };
            responsebinding.Bind(saml2ArtifactResolve);
            var response = await BuildAuthnSamlResponse(request, clientResult, idpSamlConfiguration, status, destination, issuer, realm, cancellationToken);
            await _distributedCache.SetStringAsync(saml2ArtifactResolve.Artifact, Convert.ToBase64String(Encoding.UTF8.GetBytes(response.ToXml().OuterXml)));
            return responsebinding.ToActionResult();
        }

        private async Task<Saml2AuthnResponse> BuildAuthnSamlResponse(Saml2Request request, ClientResult clientResult, Saml2Configuration idpSamlConfiguration, Saml2StatusCodes status, Uri destination, string issuer, string realm, CancellationToken cancellationToken)
        {
            var response = new Saml2AuthnResponse(idpSamlConfiguration)
            {
                InResponseTo = request.Id,
                Status = status,
                Destination = destination
            };
            if (status == Saml2StatusCodes.Success)
            {
                var claimsIdentity = await BuildSubject(clientResult.Client, issuer, realm, cancellationToken);
                response.SessionIndex = Guid.NewGuid().ToString();
                response.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single(), NameIdentifierFormats.Persistent);
                response.ClaimsIdentity = claimsIdentity;
                response.CreateSecurityToken(clientResult.Client.ClientId, subjectConfirmationLifetime: 5, issuedTokenLifetime: 60);
            }

            return response;
        }

        private async Task<ClaimsIdentity> BuildSubject(Client client, string issuer, string realm, CancellationToken cancellationToken)
        {
            var nameIdentifier = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetBySubject(nameIdentifier, realm ?? Constants.DefaultRealm, cancellationToken);
            var context = new HandlerContext(new HandlerContextRequest(issuer, string.Empty, null, null, null, (X509Certificate2)null, null), realm, _options);
            context.SetUser(user, null);
            var claims = (await _scopeClaimsExtractor.ExtractClaims(context, client.Scopes, ScopeProtocols.SAML)).Select(c => new Claim(c.Key, c.Value.ToString())).ToList();
            if (claims.Count(t => t.Type == ClaimTypes.NameIdentifier) == 0)
                throw new OAuthException(string.Empty, "token cannot be generated if there is no claim, please specify one or more scope in the client");

            if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Name));

            return new ClaimsIdentity(claims, "idserver");
        }

        private IActionResult BuildError(Exception ex) => BuildError(ex.ToString());

        private IActionResult BuildError(string errorMessage)
        {
            var error = new SamlSSOError { ErrorMessage = errorMessage };
            using (var stream = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    var serializer = new XmlSerializer(typeof(SamlSSOError));
                    serializer.Serialize(writer, error);
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        Content = stream.ToString(),
                    };
                }
            }            
        }

        private record ClientResult
        {
            public Client Client { get; set; } = null!;
            public Saml2Configuration SpSamlConfiguration { get; set; } = null!;
            public EntityDescriptor EntityDescriptor { get; set; } = null!;
        }
    }
}
