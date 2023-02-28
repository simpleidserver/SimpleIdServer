using (var httpClient = new HttpClient())
{
    var form = new Dictionary<string, string>
    {
        { "grant_type", "client_credentials" },
        { "client_id", "m2m" },
        { "client_secret", "password" },
        { "scope", "read" }
    };
    var tokenResponse = httpClient.PostAsync("http://localhost:5001/token", new FormUrlEncodedContent(form)).Result;
    var json = tokenResponse.Content.ReadAsStringAsync().Result;
    System.Console.WriteLine(json);
}