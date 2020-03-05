// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMConstants.SCIMEndpoints.Bulk)]
    public class BulkController : Controller
    {
        private readonly IHttpContextFactory _httpContextFactory;
        private readonly SCIMHostOptions _options;

        public BulkController(IHttpContextFactory httpContextFactory, IOptionsMonitor<SCIMHostOptions> options)
        {
            _httpContextFactory = httpContextFactory;
            _options = options.CurrentValue;
        }

        [HttpPost]
        [Authorize("BulkScimResource")]
        public async Task<IActionResult> Index([FromBody] JObject jObj)
        {
            try
            {
                var patchOperations = ExtractRequest(jObj);
                var size = ASCIIEncoding.ASCII.GetByteCount(jObj.ToString());
                if (patchOperations.Count() > _options.MaxOperations || size > _options.MaxPayloadSize)
                {
                    throw new SCIMTooManyBulkOperationsException();
                }

                var taskLst = new List<Task<JObject>>();
                foreach(var patchOperation in patchOperations)
                {
                    taskLst.Add(ExecuteBulkOperation(patchOperation));
                }

                var taskResult = await Task.WhenAll(taskLst);
                var result = new JObject
                {
                    { SCIMConstants.StandardSCIMRepresentationAttributes.Schemas, new JArray(new [] { SCIMConstants.StandardSchemas.BulkResponseSchemas.Id } ) },
                    { SCIMConstants.StandardSCIMRepresentationAttributes.Operations, new JArray(taskResult) }
                };
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = result.ToString(),
                    ContentType = SCIMConstants.STANDARD_SCIM_CONTENT_TYPE
                };
            }
            catch (SCIMBadSyntaxException)
            {
                return this.BuildError(HttpStatusCode.BadRequest, "Request is unparsable, syntactically incorrect, or violates schema.", "invalidSyntax");
            }
            catch(SCIMTooManyBulkOperationsException)
            {
                return this.BuildError(HttpStatusCode.RequestEntityTooLarge, "{'maxOperations': "+_options.MaxOperations+", 'maxPayloadSize': "+_options.MaxPayloadSize+" }.", "tooLarge");
            }
            catch (Exception ex)
            {
                return this.BuildError(HttpStatusCode.InternalServerError, ex.ToString(), SCIMConstants.ErrorSCIMTypes.InternalServerError);
            }
        }

        private IEnumerable<SCIMBulkOperationRequest> ExtractRequest(JObject jObj)
        {
            var requestedSchemas = jObj.GetSchemas();
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadSyntaxException($"{SCIMConstants.StandardSCIMRepresentationAttributes.Schemas} attribute is missing");
            }

            if (!new List<string> { SCIMConstants.StandardSchemas.BulkRequestSchemas.Id }.SequenceEqual(requestedSchemas))
            {
                throw new SCIMBadSyntaxException($"some schemas are not recognized by the endpoint");
            }
            var operations = jObj.SelectToken("Operations") as JArray;
            if (operations == null)
            {
                throw new SCIMBadSyntaxException("Operations parameter must be passed");
            }

            var result = new List<SCIMBulkOperationRequest>();
            foreach(JObject operation in operations)
            {
                var record = new SCIMBulkOperationRequest();
                if (operation.ContainsKey(SCIMConstants.StandardSCIMRepresentationAttributes.Method))
                {
                    record.HttpMethod = operation[SCIMConstants.StandardSCIMRepresentationAttributes.Method].ToString();
                }

                if (operation.ContainsKey(SCIMConstants.StandardSCIMRepresentationAttributes.Path))
                {
                    record.Path = operation[SCIMConstants.StandardSCIMRepresentationAttributes.Path].ToString();
                }

                if (operation.ContainsKey(SCIMConstants.StandardSCIMRepresentationAttributes.BulkId))
                {
                    record.BulkIdentifier = operation[SCIMConstants.StandardSCIMRepresentationAttributes.BulkId].ToString();
                }

                if (operation.ContainsKey(SCIMConstants.StandardSCIMRepresentationAttributes.Data))
                {
                    record.Data = JToken.Parse(operation[SCIMConstants.StandardSCIMRepresentationAttributes.Data].ToString());
                }

                result.Add(record);
            }

            return result;
        }

        private async Task<JObject> ExecuteBulkOperation(SCIMBulkOperationRequest scimBulkOperationRequest)
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
            newHttpContext.Request.Method = scimBulkOperationRequest.HttpMethod;
            if (scimBulkOperationRequest.Data != null && (scimBulkOperationRequest.HttpMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase) ||
                scimBulkOperationRequest.HttpMethod.Equals("PUT", StringComparison.InvariantCultureIgnoreCase) ||
                scimBulkOperationRequest.HttpMethod.Equals("PATCH", StringComparison.InvariantCultureIgnoreCase)))
            {
                newHttpContext.Request.ContentType = SCIMConstants.STANDARD_SCIM_CONTENT_TYPE;
                newHttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(scimBulkOperationRequest.Data.ToString()));
            }

            newHttpContext.RequestServices = HttpContext.RequestServices;
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
                { SCIMConstants.StandardSCIMRepresentationAttributes.Method, scimBulkOperationRequest.HttpMethod },
                { SCIMConstants.StandardSCIMRepresentationAttributes.BulkId, scimBulkOperationRequest.BulkIdentifier }
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
                    var scimTypeToken = json.SelectToken("response.scimType");
                    var detailToken = json.SelectToken("response.detail");
                    if (scimTypeToken != null)
                    {
                        response.Add("scimType", scimTypeToken.ToString());
                    }

                    if (detailToken != null)
                    {
                        response.Add("detail", detailToken.ToString());
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