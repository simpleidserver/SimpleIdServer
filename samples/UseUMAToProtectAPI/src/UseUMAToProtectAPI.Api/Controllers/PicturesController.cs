using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UseUMAToProtectAPI.Api.Domains;
using UseUMAToProtectAPI.Api.Persistence;

namespace UseUMAToProtectAPI.Api.Controllers
{
    [Route("pictures")]
    public class PicturesController : Controller
    {
        private const string UMA_BASE_URL = "https://localhost:60001";
        private readonly IPictureRepository _pictureRepository;
        private readonly IAuthenticationService _authenticationService;

        public PicturesController(IPictureRepository pictureRepository, IAuthenticationService authenticationService)
        {
            _pictureRepository = pictureRepository;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        [Authorize("Authenticated")]
        public IActionResult Get()
        {
            var userIdentifier = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var ids = _pictureRepository.FindAllExceptUser(userIdentifier).Select(p => p.Identifier).ToList();
            return new OkObjectResult(ids);
        }

        [HttpGet(".me")]
        [Authorize("Authenticated")]
        public IActionResult GetMe()
        {
            var userIdentifier = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var ids = _pictureRepository.FindByCreator(userIdentifier).Select(p => p.Identifier);
            return new OkObjectResult(ids);
        }

        [HttpPost]
        [Authorize("Authenticated")]
        public async Task<IActionResult> Add([FromBody] JObject receivedRequest)
        {
            var userIdentifier = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var picture = ExtractPictureFromRequest(receivedRequest, userIdentifier);
            var accessToken = await GetAccessTokenWithUMAProtectionScope();
            using (var httpClient = new HttpClient())
            {
                var jObj = new JObject
                {
                    { "resource_scopes", new JArray("read") },
                    { "subject", userIdentifier },
                    { "icon", picture.Url },
                    { "name#en", "Picture" },
                    { "description#en", picture.Description },
                    { "type", "picture" }
                };
                var httpRequest = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{UMA_BASE_URL}/rreguri"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json"),
                };
                httpRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
                var httpResult = await httpClient.SendAsync(httpRequest);
                var json = await httpResult.Content.ReadAsStringAsync();
                picture.ResourceId = JObject.Parse(json)["_id"].ToString();
                _pictureRepository.AddPicture(picture);
            }

            return new NoContentResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var picture = _pictureRepository.FindPictureByIdentifier(id);
            if (picture == null)
            {
                return new NotFoundResult();
            }

            var authResult = await _authenticationService.AuthenticateAsync(HttpContext, JwtBearerDefaults.AuthenticationScheme);
            var isAuthorized = authResult.Succeeded;
            if (isAuthorized)
            {
                isAuthorized = authResult.Principal.Claims.Any(c => {
                    if (c.Type != "permissions")
                    {
                        return false;
                    }

                    var jObj = JObject.Parse(c.Value);
                    var scopes = jObj["resource_scopes"].Values<string>();
                    if (jObj["resource_id"].ToString() != picture.ResourceId || !scopes.Contains("read"))
                    {
                        return false;
                    }

                    return true;
                });
            }

            if (!isAuthorized)
            {
                using (var httpClient = new HttpClient())
                {
                    var jObj = new JObject
                    {
                        { "resource_id", picture.ResourceId },
                        { "resource_scopes", new JArray("read") }
                    };
                    var accessToken = await GetAccessTokenWithUMAProtectionScope();
                    var httpRequest = new HttpRequestMessage
                    {
                        RequestUri = new Uri($"{UMA_BASE_URL}/perm"),
                        Method = HttpMethod.Post,
                        Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
                    };
                    httpRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
                    var httpResult = await httpClient.SendAsync(httpRequest);
                    var json = await httpResult.Content.ReadAsStringAsync();
                    return new BadRequestObjectResult(new JObject
                    {
                        { "ticket", JObject.Parse(json)["ticket"].ToString() },
                        { "location", UMA_BASE_URL }
                    });

                }
            }

            return new OkObjectResult(new JObject
            {
                { "url", picture.Url },
                { "description", picture.Description }
            });
        }

        [HttpGet(".me/{id}")]
        [Authorize("Authenticated")]
        public IActionResult GetMe(string id)
        {
            var picture = _pictureRepository.FindPictureByIdentifier(id);
            if (picture == null)
            {
                return new NotFoundResult();
            }

            var userIdentifier = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (picture.UserId != userIdentifier)
            {
                return new UnauthorizedResult();
            }

            return new OkObjectResult(new JObject
            {
                { "url", picture.Url },
                { "description", picture.Description }
            });
        }

        private static Picture ExtractPictureFromRequest(JObject receivedRequest, string userIdentifier)
        {
            var picture = new Picture(Guid.NewGuid().ToString(), userIdentifier);
            if (receivedRequest.ContainsKey("url"))
            {
                picture.Url = receivedRequest["url"].ToString();
            }

            if (receivedRequest.ContainsKey("description"))
            {
                picture.Description = receivedRequest["description"].ToString();
            }

            return picture;
        }

        private async Task<string> GetAccessTokenWithUMAProtectionScope()
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{UMA_BASE_URL}/token"),
                    Method = HttpMethod.Post,
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "client_id", "api" },
                        { "client_secret", "apiSecret" },
                        { "grant_type", "client_credentials" },
                        { "response_type", "token" },
                        { "scope", "uma_protection" }
                    })
                };

                var result = await httpClient.SendAsync(request);
                var json = await result.Content.ReadAsStringAsync();
                var jObj = JsonConvert.DeserializeObject<JObject>(json);
                return jObj["access_token"].ToString();
            }
        }
    }
}
