// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMEndpoints.Bulk)]
    public class BulkController : Controller
    {
        private readonly SCIMHostOptions _options;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BulkController(IOptionsMonitor<SCIMHostOptions> options, ILogger<BulkController> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _options = options.CurrentValue;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Create multiple representations at once.
        /// </summary>
        /// <response code="200">Representations are created</response>
        /// <response code="400">Request is not valid</response>
        /// <response code="413">Request is too large</response>
        /// <response code="500">Something goes wrong in the server</response>
        /// <param name="bulk"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(413)]
        [ProducesResponseType(500)]
        [HttpPost]
        [Authorize("BulkScimResource")]
        public async virtual Task<IActionResult> Index([FromBody] BulkParameter bulk)
        {
            var json = JsonConvert.SerializeObject(bulk);
            _logger.LogInformation(string.Format(Global.StartBulk, json));
            try
            {
                CheckParameter(bulk);
                var size = ASCIIEncoding.ASCII.GetByteCount(json.ToString());
                if (bulk.Operations.Count() > _options.MaxOperations || size > _options.MaxPayloadSize)
                {
                    throw new SCIMTooManyBulkOperationsException();
                }

                var taskLst = new List<Task<JObject>>();
                foreach(var patchOperation in bulk.Operations)
                {
                    taskLst.Add(ExecuteBulkOperation(patchOperation));
                }

                var taskResult = await Task.WhenAll(taskLst);
                var result = new JObject
                {
                    { StandardSCIMRepresentationAttributes.Schemas, new JArray(new [] { StandardSchemas.BulkResponseSchemas.Id } ) },
                    { StandardSCIMRepresentationAttributes.Operations, new JArray(taskResult) }
                };
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result.ToString(),
                    ContentType = SCIMConstants.STANDARD_SCIM_CONTENT_TYPE
                };
            }
            catch (SCIMBadSyntaxException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, SCIMConstants.ErrorSCIMTypes.InvalidSyntax);
            }
            catch(SCIMTooManyBulkOperationsException ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.RequestEntityTooLarge, "{'maxOperations': "+_options.MaxOperations+", 'maxPayloadSize': "+_options.MaxPayloadSize+" }.", SCIMConstants.ErrorSCIMTypes.TooLarge);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.BuildError(HttpStatusCode.InternalServerError, ex.ToString(), SCIMConstants.ErrorSCIMTypes.InternalServerError);
            }
        }

        protected static void CheckParameter(BulkParameter bulkParameter)
        {
            var requestedSchemas = bulkParameter.Schemas;
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, StandardSCIMRepresentationAttributes.Schemas));
            }

            if (!new List<string> { StandardSchemas.BulkRequestSchemas.Id }.SequenceEqual(requestedSchemas))
            {
                throw new SCIMBadSyntaxException(Global.SchemasNotRecognized);
            }

            if (bulkParameter.Operations == null)
            {
                throw new SCIMBadSyntaxException(string.Format(Global.AttributeMissing, StandardSCIMRepresentationAttributes.Operations));
            }
        }

        protected async Task<JObject> ExecuteBulkOperation(BulkOperationParameter scimBulkOperationRequest)
        {
            var router = RouteData.Routers.OfType<IRouteCollection>().First();
            var features = new FeatureCollection();
            features.Set<IHttpRequestFeature>(new HttpRequestFeature());
            features.Set<IRoutingFeature>(new RoutingFeature
            {
                RouteData = RouteData
            });
            features.Set<IHttpResponseFeature>(new HttpResponseFeature());
            features.Set<IResponseCookiesFeature>(new ResponseCookiesFeature(features));
            var newHttpContext = new DefaultHttpContext(features);
            newHttpContext.Request.Path = scimBulkOperationRequest.Path;
            newHttpContext.Request.PathBase = HttpContext.Request.PathBase;
            newHttpContext.Request.Host = HttpContext.Request.Host;
            newHttpContext.User = this.User;
            newHttpContext.Request.Method = scimBulkOperationRequest.HttpMethod;
            if (scimBulkOperationRequest.Data != null && (scimBulkOperationRequest.HttpMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase) ||
                scimBulkOperationRequest.HttpMethod.Equals("PUT", StringComparison.InvariantCultureIgnoreCase) ||
                scimBulkOperationRequest.HttpMethod.Equals("PATCH", StringComparison.InvariantCultureIgnoreCase)))
            {
                newHttpContext.Request.ContentType = SCIMConstants.STANDARD_SCIM_CONTENT_TYPE;
                newHttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(scimBulkOperationRequest.Data.ToString()));
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                newHttpContext.RequestServices = scope.ServiceProvider;
                newHttpContext.Response.Body = new MemoryStream();
                var routeContext = new RouteContext(newHttpContext)
                {
                    RouteData = RouteData
                };
                await router.RouteAsync(routeContext);
                var ctx = routeContext.HttpContext;
                await routeContext.Handler.Invoke(newHttpContext);
                var result = new JObject
            {
                { StandardSCIMRepresentationAttributes.Method, scimBulkOperationRequest.HttpMethod },
                { StandardSCIMRepresentationAttributes.BulkId, scimBulkOperationRequest.BulkIdentifier }
            };
                var statusCode = newHttpContext.Response.StatusCode;
                var statusContent = new JObject();
                statusContent.Add("code", statusCode);
                if (statusCode >= 400)
                {
                    newHttpContext.Response.Body.Position = 0;
                    using (var reader = new StreamReader(newHttpContext.Response.Body))
                    {
                        var responseBody = await reader.ReadToEndAsync();
                        JObject json = (JObject)JsonConvert.DeserializeObject(responseBody);
                        var response = new JObject();
                        if(json != null)
                        {
                            var scimTypeToken = json.SelectToken("response.scimType");
                            var detailToken = json.SelectToken("response.detail");
                            if (scimTypeToken != null) response.Add("scimType", scimTypeToken.ToString());
                            if (detailToken != null) response.Add("detail", detailToken.ToString());
                        }

                        statusContent.Add("response", response);
                    }
                }

                result.Add("status", statusContent);
                if (newHttpContext.Response.Headers.ContainsKey("ETag"))
                {
                    result.Add("version", newHttpContext.Response.Headers["ETag"].First());
                }

                if (newHttpContext.Response.Headers.ContainsKey("Location"))
                {
                    result.Add("location", newHttpContext.Response.Headers["Location"].First());
                }

                return result;
            }
        }
    }
}