// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management
{
    [Route(Constants.EndPoints.Management)]
    public partial class ManagementController : Controller
    {
        private readonly IOAuthClientQueryRepository _oauthClientQueryRepository;
        private readonly IOAuthScopeQueryRepository _oauthScopeQueryRepository;

        public ManagementController(IOAuthClientQueryRepository oauthClientQueryRepository, IOAuthScopeQueryRepository oAuthScopeQueryRepository)
        {
            _oauthClientQueryRepository = oauthClientQueryRepository;
            _oauthScopeQueryRepository = oAuthScopeQueryRepository;
        }

        [HttpPost("clients/.search")]
        [Authorize("ManageClients")]
        public virtual Task<IActionResult> SearchClients([FromBody] JObject request)
        {
            var queries = request.ToEnumerable();
            return InternalSearchClients(queries);
        }

        [HttpGet("clients/.search")]
        [Authorize("ManageClients")]
        public virtual Task<IActionResult> SearchClients()
        {
            var queries = Request.Query.ToEnumerable();
            return InternalSearchClients(queries);
        }

        [HttpGet("clients/{id}")]
        [Authorize("ManageClients")]
        public virtual async Task<IActionResult> GetClient(string id)
        {
            var client = await _oauthClientQueryRepository.FindOAuthClientById(id);
            if (client == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(ToDto(client));
        }

        [HttpGet("scopes")]
        [Authorize("ManageScopes")]
        public virtual async Task<IActionResult> GetScopes()
        {
            var result = await _oauthScopeQueryRepository.GetAllOAuthScopes();
            return new OkObjectResult(result.Select(_ => ToDto(_)));
        }

        private async Task<IActionResult> InternalSearchClients(IEnumerable<KeyValuePair<string, string>> queries)
        {
            var parameter = ToParameter(queries);
            var result = await _oauthClientQueryRepository.Find(parameter, CancellationToken.None);
            return new OkObjectResult(ToDto(result));

        }

        private static JObject ToDto(OAuthScope scope)
        {
            return new JObject
            {
                { "name", scope.Name },
                { "is_exposed", scope.IsExposedInConfigurationEdp },
                { "update_datetime", scope.UpdateDateTime },
                { "create_datetime", scope.CreateDateTime }
            };
        }

        private static JObject ToDto(SearchResult<OAuthClient> result)
        {
            return new JObject
            {
                { "start_index", result.StartIndex },
                { "count", result.Count },
                { "total_length", result.TotalLength },
                { "content", new JArray(result.Content.Select(_ => ToDto(_))) }
            };
        }

        private static JObject ToDto(OAuthClient client)
        {
            var result = new JObject
            {
                { "client_id", client.ClientId },
                { "create_datetime", client.CreateDateTime },
                { "update_datetime", client.UpdateDateTime },
                { "preferred_token_profile", client.PreferredTokenProfile },
                { RegisterRequestParameters.TokenEndpointAuthMethod, client.TokenEndPointAuthMethod },
                { RegisterRequestParameters.JwksUri, client.JwksUri },
                { RegisterRequestParameters.SoftwareId, client.SoftwareId },
                { RegisterRequestParameters.SoftwareVersion, client.SoftwareVersion },
                { RegisterRequestParameters.TokenSignedResponseAlg, client.TokenSignedResponseAlg },
                { RegisterRequestParameters.TokenEncryptedResponseAlg, client.TokenEncryptedResponseAlg },
                { RegisterRequestParameters.TokenEncryptedResponseEnc, client.TokenEncryptedResponseEnc }
            };
            if (client.Contacts != null)
            {
                result.Add(RegisterRequestParameters.Contacts, new JArray(client.Contacts));
            }

            if (client.PolicyUris != null)
            {
                result.Add(RegisterRequestParameters.PolicyUri, new JArray(client.PolicyUris.Select(_ => ToDto(_))));
            }

            if (client.RedirectionUrls != null)
            {
                result.Add(RegisterRequestParameters.RedirectUris, new JArray(client.RedirectionUrls));
            }
            
            if (client.GrantTypes != null)
            {
                result.Add(RegisterRequestParameters.GrantTypes, new JArray(client.GrantTypes));
            }

            if (client.ResponseTypes != null)
            {
                result.Add(RegisterRequestParameters.ResponseTypes, new JArray(client.ResponseTypes));
            }

            if (client.ClientNames != null)
            {
                result.Add(RegisterRequestParameters.ClientName, new JArray(client.ClientNames.Select(_ => ToDto(_))));
            }

            if (client.ClientUris != null)
            {
                result.Add(RegisterRequestParameters.ClientUri, new JArray(client.ClientUris.Select(_ => ToDto(_))));
            }

            if (client.LogoUris != null)
            {
                result.Add(RegisterRequestParameters.LogoUri, new JArray(client.LogoUris.Select(_ => ToDto(_))));
            }

            if (client.AllowedScopes != null)
            {
                result.Add(RegisterRequestParameters.Scope, new JArray(client.AllowedScopes.Select(_ => _.Name)));
            }

            if (client.TosUris != null)
            {
                result.Add(RegisterRequestParameters.TosUri, new JArray(client.TosUris.Select(_ => ToDto(_))));
            }

            return result;
        }

        private static JObject ToDto(OAuthTranslation translation)
        {
            return new JObject
            {
                { "language", translation.Language },
                { "value", translation.Value }
            };
        }

        private static SearchClientParameter ToParameter(IEnumerable<KeyValuePair<string, string>> queries)
        {
            var result = new SearchClientParameter();
            result.ExtractSearchParameter(queries);
            return result;
        }
    }
}