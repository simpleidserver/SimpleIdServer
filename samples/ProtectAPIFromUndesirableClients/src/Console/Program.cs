using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var httpClient = new HttpClient())
            {
                var form = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "scope", "get_weather" },
                    { "client_id", "console" },
                    { "client_secret", "consoleSecret" }
                };
                var tokenResponse = httpClient.PostAsync("http://localhost:5000/token", new FormUrlEncodedContent(form)).Result;
                var json = tokenResponse.Content.ReadAsStringAsync().Result;
                var req = new HttpRequestMessage
                {
                    RequestUri = new Uri("https://localhost:7000/WeatherForecast"),
                    Method = HttpMethod.Get
                };
                req.Headers.Add("Authorization", $"Bearer {JsonDocument.Parse(json).RootElement.GetProperty("access_token")}");
                var resp = httpClient.Send(req);
                json = resp.Content.ReadAsStringAsync().Result;
                System.Console.WriteLine(json);
            }
        }
    }
}
