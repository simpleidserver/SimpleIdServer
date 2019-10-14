using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMConstants.SCIMEndpoints.Bulk)]
    public class BulkController : Controller
    {
        private readonly IHttpContextFactory _httpContextFactory;

        public BulkController(IHttpContextFactory httpContextFactory)
        {
            _httpContextFactory = httpContextFactory;
        }

        // [HttpPost]
        public async Task<IActionResult> Index(/*[FromBody] JObject jObj*/)
        {
            var operations = new List<SCIMBulkOperationRequest>
            {
                new SCIMBulkOperationRequest
                {
                    BulkIdentifier = "id",
                    HttpMethod = "GET",
                    Path = "/Users/id",
                    Version = "version"
                }
            };
            var operationsResult = new JArray();
            foreach(var operation in operations)
            {
                operationsResult.Add(await ExecuteBulkOperation(operation));
            }

            return new OkObjectResult(operationsResult);
        }

        /// <summary>
        /// Execute each bulk operation in its own HTTP CONTEXT.
        /// </summary>
        /// <param name="scimBulkOperationRequest"></param>
        /// <returns></returns>
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
                { "method", scimBulkOperationRequest.HttpMethod },
                { "bulkId", scimBulkOperationRequest.BulkIdentifier }
            };
            var statusCode = newHttpContext.Response.StatusCode;
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

                    result.Add("status", new JObject
                    {
                        { "code", statusCode },
                        { "response", response }
                    });
                }
            }
            else
            {
                result.Add("status", new JObject
                {
                    "code", statusCode
                });
            }

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