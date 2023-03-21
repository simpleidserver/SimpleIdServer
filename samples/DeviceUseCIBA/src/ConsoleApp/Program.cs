using IdentityModel.Client;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

var certificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "CN=client.pfx"));
var req = new BackchannelAuthenticationRequest()
{
    Address = "https://localhost:5001/master/mtls/bc-authorize",
    ClientId = "cibaConformance",
    Scope = "openid profile",
    LoginHint = "user",
    BindingMessage = "Message",
    RequestedExpiry = 200
};
var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
handler.CheckCertificateRevocationList = false;
handler.ClientCertificateOptions = ClientCertificateOption.Manual;
handler.SslProtocols = SslProtocols.Tls12;
handler.ClientCertificates.Add(certificate);
var client = new HttpClient(handler);
var response = await client.RequestBackchannelAuthenticationAsync(req);

bool cont = true;
while(cont)
{
    var tokenResponse = await client.RequestBackchannelAuthenticationTokenAsync(new BackchannelAuthenticationTokenRequest
    {
        Address = "https://localhost:5001/master/mtls/token",
        ClientId = "cibaConformance",
        AuthenticationRequestId = response.AuthenticationRequestId
    });
    if(tokenResponse.IsError)
        Console.WriteLine(tokenResponse.Error);
    else
    {
        Console.WriteLine(tokenResponse.AccessToken);
        cont = false;
    }
}