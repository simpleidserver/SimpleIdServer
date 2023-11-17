// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Register
{
    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc7591
    /// </summary>
    public class RegistrationController : BaseController
    {
        private readonly IClientRepository _clientRepository;
        private readonly IScopeRepository _scopeRepository;
        private readonly IRegisterClientRequestValidator _validator;
        private readonly IRealmRepository _realmRepository;
        private readonly IBusControl _busControl;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IdServerHostOptions _options;

        public RegistrationController(
            IClientRepository clientRepository, 
            IScopeRepository scopeRepository, 
            IRegisterClientRequestValidator validator, 
            IRealmRepository realmRepository, 
            IBusControl busControl, 
            IJwtBuilder jwtBuilder, 
            IOptions<IdServerHostOptions> options)
        {
            _clientRepository = clientRepository;
            _scopeRepository = scopeRepository;
            _validator = validator;
            _realmRepository = realmRepository;
            _busControl = busControl;
            _jwtBuilder = jwtBuilder;
            _options = options.Value;
        }

        /// <summary>
        /// Register a client.
        /// </summary>
        /// <param name="prefix">Realm</param>
        /// <param name="request">Registration request</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] RegisterClientRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Register Client"))
            {
                try
                {
                    CheckAccessToken(prefix, Constants.StandardScopes.Register.Name, _jwtBuilder);
                    var client = await Build(request, cancellationToken);
                    await _validator.Validate(prefix, client, cancellationToken);
                    _clientRepository.Add(client);
                    await _clientRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok, "Client is registered");
                    await _busControl.Publish(new ClientRegisteredSuccessEvent
                    {
                        Realm = prefix,
                        RequestJSON = JsonSerializer.Serialize(request)
                    });
                    return new ContentResult
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Content = client.Serialize(Request.GetAbsoluteUriWithVirtualPath()).ToJsonString(),
                        ContentType = "application/json"
                    };
                }
                catch (OAuthException ex)
                {
                    await _busControl.Publish(new ClientRegisteredFailureEvent
                    {
                        Realm = prefix,
                        ErrorMessage = ex.Message,
                        RequestJSON = JsonSerializer.Serialize(request)
                    });
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                    var jObj = new JsonObject
                    {
                        [ErrorResponseParameters.Error] = ex.Code,
                        [ErrorResponseParameters.ErrorDescription] = ex.Message
                    };
                    return new BadRequestObjectResult(jObj);
                }
            }

            async Task<Client> Build(RegisterClientRequest request, CancellationToken cancellationToken)
            {
                DateTime? expirationDateTime = null;
                if (_options.ClientSecretExpirationInSeconds != null)
                    expirationDateTime = DateTime.UtcNow.AddSeconds(_options.ClientSecretExpirationInSeconds.Value);

                var realm = await _realmRepository.Query().FirstAsync(r => r.Name == prefix, cancellationToken);
                var client = new Client
                {
                    Id = Guid.NewGuid().ToString(),
                    ClientId = Guid.NewGuid().ToString(),
                    RegistrationAccessToken = Guid.NewGuid().ToString(),
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    RefreshTokenExpirationTimeInSeconds = _options.DefaultRefreshTokenExpirationTimeInSeconds,
                    TokenExpirationTimeInSeconds = _options.DefaultTokenExpirationTimeInSeconds,
                    PreferredTokenProfile = _options.DefaultTokenProfile,
                    ClientSecret = Guid.NewGuid().ToString(),
                    ClientSecretExpirationTime = expirationDateTime,
                    Scopes = await GetScopes(prefix, request.Scope, cancellationToken)
                };
                client.Realms.Add(realm);
                AddTranslations(client, request, "client_name");
                AddTranslations(client, request, "logo_uri");
                AddTranslations(client, request, "client_uri");
                AddTranslations(client, request, "tos_uri");
                AddTranslations(client, request, "policy_uri");
                request.Apply(client, _options);
                return client;
            }

            void AddTranslations(Client client, RegisterClientRequest request, string key)
            {
                foreach(var translation in request.Translations.Where(t => t.Name == key))
                {
                    client.Translations.Add(new Translation
                    {
                        Key = key,
                        Language = translation.Language,
                        Value = translation.Value
                    });
                }
            }
        }

        /// <summary>
        /// Get a client
        /// </summary>
        /// <param name="prefix">Realm</param>
        /// <param name="id">Client's identifier.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var res = await GetClient(prefix, id, cancellationToken);
            if (res.HasError) return res.ErrorResult;
            return new OkObjectResult(res.Client.Serialize(Request.GetAbsoluteUriWithVirtualPath()));
        }

        /// <summary>
        /// Delete a client.
        /// </summary>
        /// <param name="prefix">Realm</param>
        /// <param name="id">Client's identifier</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var res = await GetClient(prefix, id, cancellationToken);
            if (res.HasError) return res.ErrorResult;
            var client = res.Client;
            _clientRepository.Delete(client);
            await _clientRepository.SaveChanges(cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Update a client.
        /// </summary>
        /// <param name="prefix">Realm</param>
        /// <param name="id">Client's identifier</param>
        /// <param name="request">Update request</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromRoute] string prefix, string id, RegisterClientRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var res = await GetClient(prefix, id, cancellationToken);
            if (res.HasError) return res.ErrorResult;
            try
            {
                res.Client.Scopes = await GetScopes(prefix, request.Scope, cancellationToken);
                request.Apply(res.Client, _options);
                await _validator.Validate(prefix, res.Client, cancellationToken);
                await _clientRepository.SaveChanges(cancellationToken);
                return new OkObjectResult(res.Client.Serialize(Request.GetAbsoluteUriWithVirtualPath()));
            }
            catch (OAuthException ex)
            {
                var jObj = new JsonObject
                {
                    [ErrorResponseParameters.Error] = ex.Code,
                    [ErrorResponseParameters.ErrorDescription] = ex.Message
                };
                return new BadRequestObjectResult(jObj);
            }
        }

        private async Task<GetClientResult> GetClient(string realm, string id, CancellationToken cancellationToken)
        {
            string accessToken;
            if (!TryExtractAccessToken(out accessToken)) return GetClientResult.Error(Unauthorized());
            var client = await _clientRepository.Query().Include(c => c.Translations).Include(c => c.Realms).AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == id && c.Realms.Any(r => r.Name == realm), cancellationToken);
            if (client == null) return GetClientResult.Error(NotFound());
            if (client.RegistrationAccessToken != accessToken) return GetClientResult.Error(Unauthorized());
            return GetClientResult.Ok(client);
        }

        private bool TryExtractAccessToken(out string accessToken)
        {
            StringValues vals;
            accessToken = null;
            if (!Request.Headers.TryGetValue("Authorization", out vals) || !vals.Any()) return false;
            var splittedFirstVal = vals.First().Split(' ');
            if(splittedFirstVal.Count() != 2 && splittedFirstVal.First() != "Bearer") return false;
            accessToken = splittedFirstVal.Last();
            return true;
        }

        private async Task<ICollection<Domains.Scope>> GetScopes(string realm, string scope, CancellationToken cancellationToken)
        {
            var scopeNames = string.IsNullOrWhiteSpace(scope) ? _options.DefaultScopes : scope.ToScopes();
            return await _scopeRepository.Query().Include(s => s.Realms).Where(s => scopeNames.Contains(s.Name) && s.Realms.Any(r => r.Name == realm)).ToListAsync(cancellationToken);
        }

        private class GetClientResult
        {
            private GetClientResult() { }

            public bool HasError { get; private set; }
            public IActionResult ErrorResult { get; private set; }
            public Client Client { get; private set; }

            public static GetClientResult Error(IActionResult error) => new GetClientResult { HasError = true, ErrorResult = error };

            public static GetClientResult Ok(Client client) => new GetClientResult { HasError = false, Client = client };
        }
    }
}
