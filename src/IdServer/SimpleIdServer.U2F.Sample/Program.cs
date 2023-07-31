// See https://aka.ms/new-console-template for more information
using SimpleIdServer.IdServer.U2FClient;
using SimpleIdServer.U2F.Sample;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

Console.WriteLine("Enter the login");
var login = Console.ReadLine();
Console.WriteLine("Enter the display name");
var displayName = Console.ReadLine();

var rp = "https://localhost:5001";
var baseHost = "localhost";
var beginRegisterResult = await BeginRegister();
var attestationBuilder = new FIDOU2FAttestationBuilder();
var response = attestationBuilder.BuildEnrollResponse(new EnrollParameter
{
    Challenge = beginRegisterResult.CredentialCreateOptions.Challenge,
    Rp = rp
});
await EndRegister(response, beginRegisterResult.SessionId, login);
Console.WriteLine("User '{0}' has been registered");
Console.WriteLine("Press any key to quit the application...");
Console.ReadKey();

async Task<BeginU2FRegisterResult> BeginRegister()
{
    using (var httpClient = new HttpClient())
    {
        var jObj = new JsonObject
        {
            { "login", login },
            { "display_name", displayName }
        };
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{baseHost}/master/u2f/begin-register"),
            Content = new StringContent(jObj.ToJsonString(), Encoding.UTF8, "application/json")
        };
        var httpResponse = await httpClient.SendAsync(requestMessage);
        var json = await httpResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BeginU2FRegisterResult>(json);
    }
}

async Task EndRegister(EnrollResult enroll, string sessionId, string login)
{
    using (var httpClient = new HttpClient())
    {
        var dic = new Dictionary<string, object>
        {
            { "login", login },
            { "session_id", sessionId },
            { "attestation", enroll.Response }
        };
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{baseHost}/master/u2f/end-register"),
            Content = new StringContent(JsonSerializer.Serialize(dic), Encoding.UTF8, "application/json")
        };
        var httpResponse = await httpClient.SendAsync(requestMessage);
        var json = await httpResponse.Content.ReadAsStringAsync();
    }
}