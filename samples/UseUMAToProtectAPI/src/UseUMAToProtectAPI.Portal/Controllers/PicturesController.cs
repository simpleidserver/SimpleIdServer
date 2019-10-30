using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UseUMAToProtectAPI.Portal.ViewModels;

namespace UseUMAToProtectAPI.Portal.Controllers
{
    [Authorize("Authenticated")]
    public class PicturesController : Controller
    {
        private const string BASE_API_URL = "https://localhost:5002";
        private const string BASE_PICTURES_API_URL = BASE_API_URL + "/pictures";
        private readonly IAuthenticationService _authenticationService;

        public PicturesController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string idToken = await HttpContext.GetTokenAsync("id_token");
            Func<string, Task<ICollection<string>>> callback = async (relativeUrl) =>
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri($"{BASE_PICTURES_API_URL}{relativeUrl}"),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Add("Authorization", $"Bearer {idToken}");
                    var httpResult = await httpClient.SendAsync(request);
                    return JArray.Parse(await httpResult.Content.ReadAsStringAsync()).ToObject<List<string>>();
                }
            };
            var otherPictures = await callback(string.Empty);
            var myPictures = await callback("/.me");
            return View(new IndexPicturesViewModel(myPictures, otherPictures));
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddPictureViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddPictureViewModel addPictureViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(addPictureViewModel);
            }

            using (var httpClient = new HttpClient())
            {
                var idToken = await HttpContext.GetTokenAsync("id_token");
                var jObj = new JObject
                {
                    { "url", addPictureViewModel.Url },
                    { "description", addPictureViewModel.Description }
                };
                var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{BASE_PICTURES_API_URL}"),
                    Content = new StringContent(jObj.ToString(), Encoding.UTF8, "application/json")
                };
                httpRequest.Headers.Add("Authorization", $"Bearer {idToken}");
                var httpResult = await httpClient.SendAsync(httpRequest);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Display(string id)
        {
            using (var httpClient = new HttpClient())
            {
                var idToken = await HttpContext.GetTokenAsync("id_token");
                var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{BASE_PICTURES_API_URL}/.me/{id}")
                };
                httpRequest.Headers.Add("Authorization", $"Bearer {idToken}");
                var httpResult = await httpClient.SendAsync(httpRequest);
                var json = await httpResult.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(json);
                return View(new DisplayPictureViewModel
                {
                    Url = jObj["url"].ToString(),
                    Description = jObj["description"].ToString()
                });
            }
        }

        public async Task<IActionResult> Visit(string id)
        {
            JObject jObj = null;
            using (var httpClient = new HttpClient())
            {
                var idToken = await HttpContext.GetTokenAsync("id_token");
                var umaToken = await _authenticationService.GetTokenAsync(HttpContext, "uma_token");
                var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{BASE_PICTURES_API_URL}/{id}")
                };
                if (!string.IsNullOrWhiteSpace(umaToken))
                {
                    httpRequest.Headers.Add("Authorization", $"Bearer {umaToken}");
                }

                var httpResult = await httpClient.SendAsync(httpRequest);
                var json = await httpResult.Content.ReadAsStringAsync();
                jObj = JObject.Parse(json);
                if (!httpResult.IsSuccessStatusCode)
                {
                    httpRequest = new HttpRequestMessage
                    {
                        RequestUri = new Uri($"{jObj["location"].ToString()}/token"),
                        Method = HttpMethod.Post,
                        Content = new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { "client_id", "portal" },
                            { "client_secret", "portalSecret" },
                            { "scope", "read" },
                            { "grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket" },
                            { "ticket", jObj["ticket"].ToString() },
                            { "claim_token", idToken },
                            { "claim_token_format", "http://openid.net/specs/openid-connect-core-1_0.html#IDToken" }
                        })
                    };

                    httpResult = await httpClient.SendAsync(httpRequest);
                    json = await httpResult.Content.ReadAsStringAsync();
                    if (!httpResult.IsSuccessStatusCode)
                    {
                        return new ContentResult
                        {
                            ContentType = "text/html",
                            StatusCode = (int)HttpStatusCode.OK,
                            Content = "request submitted"
                        };
                    }

                    jObj = JObject.Parse(json);
                    var authResult = await _authenticationService.AuthenticateAsync(HttpContext, null);
                    var tokens = authResult.Properties.GetTokens().ToList();
                    tokens.Add(new AuthenticationToken { Name = "uma_token", Value = jObj["access_token"].ToString() });
                    authResult.Properties.StoreTokens(tokens);
                    await HttpContext.SignInAsync(authResult.Principal, authResult.Properties);
                    return RedirectToAction("Visit", "Pictures", new { id = id });
                }
            }

            return View(new DisplayPictureViewModel
            {
                Url = jObj["url"].ToString(),
                Description = jObj["description"].ToString()
            });
        }
    }
}