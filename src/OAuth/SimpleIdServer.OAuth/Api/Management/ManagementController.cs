// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
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

            return new OkObjectResult(client.ToDto(Request.GetAbsoluteUriWithVirtualPath()));
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
            return new OkObjectResult(ToDto(result, Request.GetAbsoluteUriWithVirtualPath()));

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

        private static JObject ToDto(SearchResult<OAuthClient> result, string issuer)
        {
            return new JObject
            {
                { "start_index", result.StartIndex },
                { "count", result.Count },
                { "total_length", result.TotalLength },
                { "content", new JArray(result.Content.Select(_ => _.ToDto(issuer))) }
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