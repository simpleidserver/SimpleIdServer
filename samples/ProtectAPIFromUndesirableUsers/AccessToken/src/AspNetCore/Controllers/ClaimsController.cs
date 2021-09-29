using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Controllers
{
    public class ClaimsController : Controller
    {
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var authenticateInfo = await HttpContext.AuthenticateAsync("sid");
            var idToken = authenticateInfo.Properties.Items[".Token.access_token"];
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new System.Uri("https://localhost:7000/WeatherForecast"),
                    Method = HttpMethod.Get
                };
                request.Headers.Add("Authorization", $"Bearer {idToken}");
                var httpResult = await client.SendAsync(request);
                var json = await httpResult.Content.ReadAsStringAsync();
                return View(new { json = json });
            }
        }
    }
}
