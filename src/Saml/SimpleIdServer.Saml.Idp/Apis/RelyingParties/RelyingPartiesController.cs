// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Idp.Apis.RelyingParties.Handlers;
using SimpleIdServer.Saml.Idp.DTOs;
using SimpleIdServer.Saml.Idp.Exceptions;
using SimpleIdServer.Saml.Idp.Persistence.Parameters;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Apis.RelyingParties
{
    [Route(Constants.RouteNames.RelyingParties)]
    public class RelyingPartiesController : Controller
    {
        private readonly ISearchRelyingPartiesHandler _searchRelyingPartiesHandler;
        private readonly IAddRelyingPartyHandler _addRelyingPartyHandler;
        private readonly IGetRelyingPartyHandler _getRelyingPartyHandler;
        private readonly IUpdateRelyingPartyHandler _updateRelyingPartyHandler;
        private readonly IDeleteRelyingPartyHandler _deleteRelyingPartyHandler;

        public RelyingPartiesController(
            ISearchRelyingPartiesHandler searchRelyingPartiesHandler,
            IAddRelyingPartyHandler addRelyingPartyHandler,
            IGetRelyingPartyHandler getRelyingPartyHandler,
            IUpdateRelyingPartyHandler updateRelyingPartyHandler,
            IDeleteRelyingPartyHandler deleteRelyingPartyHandler)
        {
            _searchRelyingPartiesHandler = searchRelyingPartiesHandler;
            _addRelyingPartyHandler = addRelyingPartyHandler;
            _getRelyingPartyHandler = getRelyingPartyHandler;
            _updateRelyingPartyHandler = updateRelyingPartyHandler;
            _deleteRelyingPartyHandler = deleteRelyingPartyHandler;
        }

        #region Manage Relying Party

        [HttpPost(".search")]
        [Authorize("ManageRelyingParties")]
        public async Task<IActionResult> Search([FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            var queries = jObj.ToEnumerable();
            return await InternalSearch(queries, cancellationToken);
        }

        [HttpGet(".search")]
        [Authorize("ManageRelyingParties")]
        public async Task<IActionResult> Search(CancellationToken cancellationToken)
        {
            var queries = Request.Query.ToEnumerable();
            return await InternalSearch(queries, cancellationToken);
        }

        [HttpPost]
        [Authorize("ManageRelyingParties")]
        public async Task<IActionResult> Add([FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            var result = await _addRelyingPartyHandler.Handle(jObj, cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = new JObject
                {
                    { "id", result }
                }.ToString(),
                ContentType = "application/json"
            };
        }

        [HttpGet("{id}")]
        [Authorize("ManageRelyingParties")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _getRelyingPartyHandler.Handle(id, cancellationToken);
                return new OkObjectResult(result);
            }
            catch(RelyingPartyNotFoundException ex)
            {
                var content = new JObject
                {
                    { ErrorResponseParameters.Error, ex.Code },
                    { ErrorResponseParameters.ErrorDescription, ex.Message }
                };
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Content = content.ToString(),
                    ContentType = "application/json"
                };
            }
        }

        [HttpPut("{id}")]
        [Authorize("ManageRelyingParties")]
        public async Task<IActionResult> Update(string id, [FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            try
            {
                await _updateRelyingPartyHandler.Handle(id, jObj, cancellationToken);
                return new NoContentResult();
            }
            catch (RelyingPartyNotFoundException ex)
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

        [HttpDelete("{id}")]
        [Authorize("ManageRelyingParties")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            try
            {
                await _deleteRelyingPartyHandler.Handle(id, cancellationToken);
                return new NoContentResult();
            }
            catch (RelyingPartyNotFoundException ex)
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

        #endregion

        private async Task<IActionResult> InternalSearch(IEnumerable<KeyValuePair<string, string>> queries, CancellationToken cancellationToken)
        {
            var parameter = ToSearchRelyingPartyParameter(queries);
            return new OkObjectResult(await _searchRelyingPartiesHandler.Handle(parameter, cancellationToken));
        }

        private static SearchRelyingPartiesParameter ToSearchRelyingPartyParameter(IEnumerable<KeyValuePair<string, string>> queries)
        {
            var result = new SearchRelyingPartiesParameter();
            result.ExtractSearchParameter(queries);
            return result;
        }
    }
}
