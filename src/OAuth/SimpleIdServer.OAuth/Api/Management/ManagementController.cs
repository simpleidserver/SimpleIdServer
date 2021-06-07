// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Management.Handlers;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management
{
    [Route(Constants.EndPoints.Management)]
    public partial class ManagementController : Controller
    {
        private readonly IOAuthScopeRepository _oauthScopeRepository;
        private readonly IGetOAuthClientHandler _getOAuthClientHandler;
        private readonly ISearchOauthClientsHandler _searchOauthClientsHandler;
        private readonly IUpdateOAuthClientHandler _updateOAuthClientHandler;

        public ManagementController(
            IOAuthScopeRepository oauthScopeRepository,
            IGetOAuthClientHandler getOAuthClientHandler,
            ISearchOauthClientsHandler searchOauthClientsHandler,
            IUpdateOAuthClientHandler updateOAuthClientHandler)
        {
            _oauthScopeRepository = oauthScopeRepository;
            _getOAuthClientHandler = getOAuthClientHandler;
            _searchOauthClientsHandler = searchOauthClientsHandler;
            _updateOAuthClientHandler = updateOAuthClientHandler;
        }

        #region Manage clients

        [HttpPost("clients/.search")]
        [Authorize("ManageClients")]
        public virtual Task<IActionResult> SearchClients([FromBody] JObject request, CancellationToken cancellationToken)
        {
            var queries = request.ToEnumerable();
            return InternalSearchClients(queries, cancellationToken);
        }

        [HttpGet("clients/.search")]
        [Authorize("ManageClients")]
        public virtual Task<IActionResult> SearchClients(CancellationToken cancellationToken)
        {
            var queries = Request.Query.ToEnumerable();
            return InternalSearchClients(queries, cancellationToken);
        }

        [HttpGet("clients/{id}")]
        [Authorize("ManageClients")]
        public virtual async Task<IActionResult> GetClient(string id, CancellationToken cancellationToken)
        {
            var result = await _getOAuthClientHandler.Handle(id, Request.GetAbsoluteUriWithVirtualPath(), cancellationToken);
            if (result == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(result);
        }

        [HttpPut("clients/{id}")]
        [Authorize("ManageClients")]
        public virtual async Task<IActionResult> UpdateClient(string id, [FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            try
            {
                await _updateOAuthClientHandler.Handle(id, jObj, cancellationToken);
                return new NoContentResult();
            }
            catch(OAuthClientNotFoundException)
            {
                return new NotFoundResult();
            }
        }

        #endregion

        #region Manage scopes

        [HttpGet("scopes")]
        [Authorize("ManageScopes")]
        public virtual async Task<IActionResult> GetScopes(CancellationToken cancellationToken)
        {
            var result = await _oauthScopeRepository.GetAllOAuthScopes(cancellationToken);
            return new OkObjectResult(result.Select(_ => ToDto(_)));
        }

        #endregion

        private async Task<IActionResult> InternalSearchClients(IEnumerable<KeyValuePair<string, string>> queries, CancellationToken cancellationToken)
        {
            var parameter = ToParameter(queries);
            return new OkObjectResult(await _searchOauthClientsHandler.Handle(parameter, Request.GetAbsoluteUriWithVirtualPath(), cancellationToken));
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

        private static SearchClientParameter ToParameter(IEnumerable<KeyValuePair<string, string>> queries)
        {
            var result = new SearchClientParameter();
            result.ExtractSearchParameter(queries);
            return result;
        }
    }
}