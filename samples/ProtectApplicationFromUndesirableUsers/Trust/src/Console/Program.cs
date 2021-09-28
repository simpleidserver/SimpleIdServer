using System;
using System.Collections.Generic;
using System.Net.Http;

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
                    { "grant_type", "password" },
                    { "username", "sub" },
                    { "password", "password" },
                    { "client_id", "trusted" },
                    { "client_secret", "trustedSecret" }
                };
                var tokenResponse = httpClient.PostAsync("http://localhost:5000/token", new FormUrlEncodedContent(form)).Result;
                var json = tokenResponse.Content.ReadAsStringAsync().Result;
                System.Console.WriteLine(json);
            }
        }
    }
}
