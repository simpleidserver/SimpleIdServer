using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PicturesController : ControllerBase
    {
        private List<Picture> _pictures = new List<Picture>
        {
            new Picture
            {
                Id = "id",
                Description = "description",
                Name = "name",
                ResourceId = "resourceId"
            }
        };
        private readonly IAuthenticationService _authenticationService;

        public PicturesController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var picture = _pictures.FirstOrDefault(p => p.Id == id);
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


                    var jObj = JsonDocument.Parse(c.Value);
                    var resourceId = jObj.RootElement.GetProperty("resource_id").GetString();
                    if (resourceId != picture.ResourceId)
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
                    var jObj = JsonSerializer.Serialize(new
                    {
                        resource_id = picture.ResourceId,
                        resource_scopes = new string[] { "read" }
                    });
                    var accessToken = await GetAccessTokenWithUMAProtectionScope();
                    var httpRequest = new HttpRequestMessage
                    {
                        RequestUri = new Uri("http://localhost:60003/perm"),
                        Method = HttpMethod.Post,
                        Content = new StringContent(jObj, Encoding.UTF8, "application/json")
                    };
                    httpRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
                    var httpResult = await httpClient.SendAsync(httpRequest);
                    var json = await httpResult.Content.ReadAsStringAsync();
                    return new BadRequestObjectResult(new
                    {
                        ticket = JsonDocument.Parse(json).RootElement.GetProperty("ticket").GetString()
                    });

                }
            }

            return new OkObjectResult(picture);
        }

        private async Task<string> GetAccessTokenWithUMAProtectionScope()
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost:60003/token"),
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
                var jObj = JsonDocument.Parse(json);
                return jObj.RootElement.GetProperty("access_token").GetString();
            }
        }
    }
}
