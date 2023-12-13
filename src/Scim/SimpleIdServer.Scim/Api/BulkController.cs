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
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using SimpleIdServer.Scim.Serialization;
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
    [TypeFilter(typeof(ActivatorActionFilter), Arguments = new object[] { "IsBulkEnabled" })]
    public class BulkController : Controller
    {
        private readonly SCIMHostOptions _options;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ISCIMAttributeMappingQueryRepository _scimAttributeMappingQueryRepository;
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;

        public BulkController(
            IOptionsMonitor<SCIMHostOptions> options, 
            ILogger<BulkController> logger, 
            IServiceScopeFactory serviceScopeFactory, 
            ISCIMAttributeMappingQueryRepository scimAttributeMappingQueryRepository, 
            ISCIMSchemaQueryRepository scimSchemaQueryRepository)
        {
            _options = options.CurrentValue;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _scimAttributeMappingQueryRepository = scimAttributeMappingQueryRepository;
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
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
                if (bulk.Operations.Count() > _options.MaxOperations || size > _options.MaxPayloadSize) throw new SCIMTooManyBulkOperationsException();
                var operationsResult = new JArray();
                var attributeMappings = await _scimAttributeMappingQueryRepository.GetAll();
                var rootSchemas = await _scimSchemaQueryRepository.GetAll();
                var allParameters = new List<(BulkOperationParameter, JToken)>();
                foreach (var patchOperation in bulk.Operations)
                {
                    var updateResult = UpdateBulkIdParameters(patchOperation, allParameters, attributeMappings, rootSchemas);
                    if (!updateResult.Item1)
                    {
                        operationsResult.Add(updateResult.Item2);
                        continue;
                    }

                    var bulkOperationResult = await ExecuteBulkOperation(patchOperation);
                    operationsResult.Add(bulkOperationResult.Item1);
                    allParameters.Add((patchOperation, bulkOperationResult.Item2));
                }

                var result = new JObject
                {
                    { StandardSCIMRepresentationAttributes.Schemas, new JArray(new [] { StandardSchemas.BulkResponseSchemas.Id } ) },
                    { StandardSCIMRepresentationAttributes.Operations, operationsResult }
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

        protected async Task<(JObject, JToken)> ExecuteBulkOperation(BulkOperationParameter scimBulkOperationRequest)
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
                newHttpContext.Response.Body.Position = 0;
                var responseBody = new StreamReader(newHttpContext.Response.Body).ReadToEnd();
                newHttpContext.Response.Body.Position = 0;
                JToken json = null;
                if (!string.IsNullOrWhiteSpace(responseBody)) json = JToken.Parse(responseBody);
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

                result.Add("status", statusContent);
                if (newHttpContext.Response.Headers.ContainsKey("ETag")) result.Add("version", newHttpContext.Response.Headers["ETag"].First());
                if (newHttpContext.Response.Headers.ContainsKey("Location")) result.Add("location", newHttpContext.Response.Headers["Location"].First());
                return (result, json);
            }
        }

        private (bool, JToken) UpdateBulkIdParameters(BulkOperationParameter parameter, List<(BulkOperationParameter, JToken)> processedBulkOperations, IEnumerable<SCIMAttributeMapping> attributeMappings, IEnumerable<SCIMSchema> rootSchemas)
        {
            var path = parameter.Path;
            if (parameter.Data == null) return (true, null);
            var schemasJArr = parameter.Data.SelectToken(StandardSCIMRepresentationAttributes.Schemas) as JArray;
            if (schemasJArr == null) return (true, null);
            var schemas = schemasJArr.Select(s => s.ToString());
            var resourceTypes = rootSchemas.Where(rs => schemas.Any(s => rs.Id == s)).Select(rs => rs.ResourceType);
            var filteredAttributeMappings = attributeMappings.Where(m => resourceTypes.Contains(m.SourceResourceType));
            foreach(var attributeMapping in filteredAttributeMappings)
            {
                var token = parameter.Data.SelectToken($"$..{attributeMapping.SourceAttributeSelector}");
                if (token == null) continue;
                var arr = token as JArray;
                if (token.Type == JTokenType.Object) arr = new JArray() { token };
                if (arr == null) continue;
                foreach(var attr in arr)
                {
                    var value = attr.SelectToken(SCIMConstants.StandardSCIMReferenceProperties.Value) as JValue;
                    if (value == null || !value.ToString().Contains("bulkId")) continue;
                    string bulkId;
                    if (!ValidateBulkId(value.ToString(), out bulkId)) return (false, BuildError(string.Format(Global.BulkIdIsNotWellFormatted, value.ToString())));
                    string externalId;
                    if (!TryExtractBulkId(bulkId, out externalId)) return (false, BuildError(string.Format(Global.UnknownBulkId, bulkId)));
                    value.Value = externalId;
                }
            }

            return (true, null);

            bool ValidateBulkId(string bulkId, out string result)
            {
                result = null;
                var parameters = bulkId.Split(':');
                if (parameters.Count() != 2) return false;
                result = parameters.Last();
                return true;
            }

            bool TryExtractBulkId(string bulkId, out string id)
            {
                id = null;
                var param = processedBulkOperations.FirstOrDefault(p => p.Item1.BulkIdentifier == bulkId);
                if (param.Item1 == null || param.Item2 == null) return false;
                var idToken = param.Item2.SelectToken(StandardSCIMRepresentationAttributes.Id);
                if (idToken == null) return false;
                id = idToken.ToString();
                return true;
            }

            JObject BuildError(string description)
            {
                var serializer = new SCIMSerializer();
                return new JObject
                {
                    { StandardSCIMRepresentationAttributes.Method, parameter.HttpMethod },
                    { StandardSCIMRepresentationAttributes.BulkId, parameter.BulkIdentifier },
                    { "status", (int)HttpStatusCode.BadRequest },
                    { "response", new JObject
                        {
                            { "scimType", SCIMConstants.ErrorSCIMTypes.InvalidSyntax },
                            { "detail", description }
                        } 
                    }
                };

            }
        }
    }
}