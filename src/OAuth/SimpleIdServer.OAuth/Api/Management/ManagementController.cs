// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Management.Handlers;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly IAddOAuthClientHandler _addOAuthClientHandler;
        private readonly IDeleteOAuthClientHandler _deleteOAuthClientHandler;
        private readonly ISearchOAuthScopesHandler _searchOAuthScopesHandler;
        private readonly IUpdateOAuthScopeHandler _updateOAuthScopeHandler;
        private readonly IAddOAuthScopeHandler _addOAuthScopeHandler;
        private readonly IDeleteOAuthScopeHandler _deleteOAuthScopeHandler;
        private readonly IUpdateUserBySCIMIdHandler _updateUserBySCIMIdHandler;
        private readonly IGetUserBySCIMIdHandler _getUserBySCIMIdHandler;
        private readonly IUpdateUserPasswordHandler _updateUserPasswordHandler;
        private readonly IAddOAuthUserBySCIMIdHandler _addOAuthUserBySCIMIdHandler;
        private readonly OAuthHostOptions _options;

        public ManagementController(
            IOAuthScopeRepository oauthScopeRepository,
            IGetOAuthClientHandler getOAuthClientHandler,
            ISearchOauthClientsHandler searchOauthClientsHandler,
            IUpdateOAuthClientHandler updateOAuthClientHandler,
            IAddOAuthClientHandler addOAuthClientHandler,
            IDeleteOAuthClientHandler deleteOAuthClientHandler,
            ISearchOAuthScopesHandler searchOAuthScopesHandler,
            IUpdateOAuthScopeHandler updateOAuthScopeHandler,
            IAddOAuthScopeHandler addOAuthScopeHandler,
            IDeleteOAuthScopeHandler deleteOAuthScopeHandler,
            IUpdateUserBySCIMIdHandler updateUserBySCIMIdHandler,
            IGetUserBySCIMIdHandler getUserBySCIMIdHandler,
            IUpdateUserPasswordHandler updateUserPasswordHandler,
            IAddOAuthUserBySCIMIdHandler addOAuthUserBySCIMIdHandler,
            IOptions<OAuthHostOptions> options)
        {
            _oauthScopeRepository = oauthScopeRepository;
            _getOAuthClientHandler = getOAuthClientHandler;
            _searchOauthClientsHandler = searchOauthClientsHandler;
            _updateOAuthClientHandler = updateOAuthClientHandler;
            _addOAuthClientHandler = addOAuthClientHandler;
            _deleteOAuthClientHandler = deleteOAuthClientHandler;
            _searchOAuthScopesHandler = searchOAuthScopesHandler;
            _updateOAuthScopeHandler = updateOAuthScopeHandler;
            _addOAuthScopeHandler = addOAuthScopeHandler;
            _deleteOAuthScopeHandler = deleteOAuthScopeHandler;
            _updateUserBySCIMIdHandler = updateUserBySCIMIdHandler;
            _getUserBySCIMIdHandler = getUserBySCIMIdHandler;
            _updateUserPasswordHandler = updateUserPasswordHandler;
            _addOAuthUserBySCIMIdHandler = addOAuthUserBySCIMIdHandler;
            _options = options.Value;
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

        [HttpPost("clients")]
        [Authorize("ManageClients")]
        public virtual async Task<IActionResult> AddClient([FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            var language = this.GetLanguage(_options);
            var clientId = await _addOAuthClientHandler.Handle(language, jObj, cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JObject.FromObject(new { id = clientId }).ToString(),
                ContentType = "application/json"
            };
        }

        [HttpDelete("clients/{id}")]
        [Authorize("ManageClients")]
        public virtual async Task<IActionResult> DeleteClient(string id, CancellationToken cancellationToken)
        {
            try
            {
                await _deleteOAuthClientHandler.Handle(id, cancellationToken);
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
        public virtual async Task<IActionResult> GetAllScopes(CancellationToken cancellationToken)
        {
            var result = await _oauthScopeRepository.GetAllOAuthScopes(cancellationToken);
            return new OkObjectResult(result.Select(s => s.ToDto()));
        }

        [HttpGet("scopes/.search")]
        [Authorize("ManageScopes")]
        public virtual async Task<IActionResult> SearchScopes(CancellationToken cancellationToken)
        {
            var queries = Request.Query.ToEnumerable();
            var parameter = ToSearchScopeParameter(queries);
            return new OkObjectResult(await _searchOAuthScopesHandler.Handle(parameter, cancellationToken));
        }

        [HttpGet("scopes/{id}")]
        [Authorize("ManageScopes")]
        public virtual async Task<IActionResult> GetScope(string id, CancellationToken cancellationToken)
        {
            var scope = await _oauthScopeRepository.GetOAuthScope(id, cancellationToken);
            if (scope == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(scope.ToDto());
        }

        [HttpPut("scopes/{id}")]
        [Authorize("ManageScopes")]
        public virtual async Task<IActionResult> UpdateScope(string id, [FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            try
            {
                await _updateOAuthScopeHandler.Handle(id, jObj, cancellationToken);
                return new NoContentResult();
            }
            catch(OAuthScopeNotFoundException)
            {
                return new NotFoundResult();
            }
        }

        [HttpPost("scopes")]
        [Authorize("ManageScopes")]
        public virtual async Task<IActionResult> AddScope([FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            try
            {
                var scopeName = await _addOAuthScopeHandler.Handle(jObj, cancellationToken);
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JObject.FromObject(new { id = scopeName }).ToString(),
                    ContentType = "application/json"
                };
            }
            catch(OAuthException ex)
            {
                var content = new JObject
                {
                    { ErrorResponseParameters.Error, ex.Code },
                    { ErrorResponseParameters.ErrorDescription, ex.Message }
                };
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = content.ToString(),
                    ContentType = "application/json"
                };
            }
        }

        [HttpDelete("scopes/{id}")]
        [Authorize("ManageScopes")]
        public virtual async Task<IActionResult> DeleteScope(string id, CancellationToken cancellationToken)
        {
            try
            {
                var scopeName = await _deleteOAuthScopeHandler.Handle(id, cancellationToken);
                return new NoContentResult();
            }
            catch (OAuthScopeNotFoundException)
            {
                return new NotFoundResult();
            }
        }

        #endregion

        #region Update user

        [HttpPut("users/{id}/password")]
        [Authorize("ManageUsers")]
        public virtual async Task<IActionResult> UpdateUser(string id, [FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            try
            {
                await _updateUserPasswordHandler.Handle(id, jObj, cancellationToken);
                return new NoContentResult();
            }
            catch(OAuthUserNotFoundException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.NotFound, ex.Code, ex.Message);
            }
        }

        [HttpGet("users/scim/{id}")]
        [Authorize("ManageUsers")]
        public virtual async Task<IActionResult> GetUserByScimId(string id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _getUserBySCIMIdHandler.Handle(id, cancellationToken);
                return new OkObjectResult(result);
            }
            catch(OAuthUserNotFoundException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.NotFound, ex.Code, ex.Message);
            }
        }

        [HttpPost("users/scim")]
        [Authorize("ManageUsers")]
        public virtual async Task<IActionResult> AddUserByScimId([FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _addOAuthUserBySCIMIdHandler.Handle(jObj, cancellationToken);
                return new NoContentResult();
            }
            catch(OAuthException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        [HttpPut("users/scim/{id}")]
        [Authorize("ManageUsers")]
        public virtual async Task<IActionResult> UpdateUserByScimId(string id, [FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            try
            {
                await _updateUserBySCIMIdHandler.Handle(id, jObj, cancellationToken);
                return new NoContentResult();
            }
            catch(OAuthUserNotFoundException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.NotFound, ex.Code, ex.Message);
            }
        }

        #endregion

        private async Task<IActionResult> InternalSearchClients(IEnumerable<KeyValuePair<string, string>> queries, CancellationToken cancellationToken)
        {
            var parameter = ToSearchClientParameter(queries);
            return new OkObjectResult(await _searchOauthClientsHandler.Handle(parameter, Request.GetAbsoluteUriWithVirtualPath(), cancellationToken));
        }

        private static SearchClientParameter ToSearchClientParameter(IEnumerable<KeyValuePair<string, string>> queries)
        {
            var result = new SearchClientParameter();
            result.ExtractSearchParameter(queries);
            return result;
        }

        private static SearchScopeParameter ToSearchScopeParameter(IEnumerable<KeyValuePair<string, string>> queries)
        {
            var result = new SearchScopeParameter();
            result.ExtractSearchParameter(queries);
            return result;
        }
    }
}