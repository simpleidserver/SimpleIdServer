using (var httpClient = new HttpClient())
{
    var form = new Dictionary<string, string>
    {
        { "grant_type", "client_credentials" },
        { "client_id", "m2m" },
        { "client_secret", "password" },
        { "scope", "read" }
    };
    var tokenResponse = httpClient.PostAsync("https://localhost:5001/master/token", new FormUrlEncodedContent(form)).Result;
    var json = tokenResponse.Content.ReadAsStringAsync().Result;
    System.Console.WriteLine(json);
}