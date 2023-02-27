# Protect REST.API from undesirable clients

REST.API services can be protected from undesirable clients (for example : REST.API, WPF or console application) by checking received access tokens.
An access token has two different formats:

* **Json Web Signature (JWS)** : a Json Web Token signed with the certificate coming from the OPENID server.
* **Json Web Encryption (JWE)** : a Json Web Signatured encrypted with the certificate coming from the client.

Once the access token is checked. The REST.API server fetches the `scopes` from the token and checks if the HTTP request is authorized to access to the specific operation.

## Protect REST.API from JWS access token

This tutorial explains how to protect a REST.API from undesirable clients.
The client is going to use the **client credentials** grant type to get an access token, it will be passed in all outgoing HTTP requests.

> [!WARNING]
> Before you start, Make sure there is a Visual Studio Solution with a [configured OpenId server](/documentation/openid/installation.html).
	
### Source Code

The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ProtectAPIFromUndesirableClients).

### Configure OpenId Server

The first step consists to configure the OPENID client.

* Open the Visual Studio Solution and edit the `OpenIdDefaultConfiguration.cs` file.
* Add a new scope `get_weather` :

```
new OAuthScope
{
    Name = "get_weather",
    IsExposedInConfigurationEdp = false,
    IsStandardScope = false
}
```

* Add a new OpenId client :

```
new OpenIdClient
{
    ClientId = "console",
    ClientSecret = "consoleSecret",
    ApplicationKind = ApplicationKinds.Service,
    TokenEndPointAuthMethod = "client_secret_post",
    UpdateDateTime = DateTime.UtcNow,
    CreateDateTime = DateTime.UtcNow,
    TokenExpirationTimeInSeconds = 60 * 30,
    RefreshTokenExpirationTimeInSeconds = 60 * 30,
    TokenSignedResponseAlg = "RS256",
    AllowedScopes = new List<OAuthScope>
    {
        SIDOpenIdConstants.StandardScopes.OpenIdScope,
        SIDOpenIdConstants.StandardScopes.Profile,
        SIDOpenIdConstants.StandardScopes.Email,
        new OAuthScope
        {
            Name = "get_weather",
            IsExposedInConfigurationEdp = false,
            IsStandardScope = false
        }
    },
    GrantTypes = new List<string>
    {
        "client_credentials"
    },
    PreferredTokenProfile = "Bearer"
}
```

* Run the OPENID server.

```
cd src\OpenId
dotnet run --urls=https://localhost:5001
```

### Create REST.API service

The second step consists to create and protect a REST.API service.

* Open a command and navigate to the `src` subfolder of your project.
* Create a directory `WebApi` and create an ASP.NET CORE Web API project in it :

```
mkdir WebApi

dotnet new webapi -n WebApi
```

* Navigate to the directory `WebApi` and install the Nuget package `Microsoft.AspNetCore.Authentication.JwtBearer`. It will be used to check received access tokens.

```
cd WebApi

dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

* Add the `WebApi` project into your Visual Studio solution.

```
cd ..\..
dotnet sln add ./src/WebApi/WebApi.csproj
```

* Edit the `Controllers\WeatherForecastController.cs` and decorate the operation `Get()` with `Authorize` attribute.

```
[HttpGet]
[Authorize("GetWeather")]
public IEnumerable<WeatherForecast> Get()
{
    var rng = new Random();
    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Date = DateTime.Now.AddDays(index),
        TemperatureC = rng.Next(-20, 55),
        Summary = Summaries[rng.Next(Summaries.Length)]
    })
    .ToArray();
}
```

* Edit the `Startup.cs` file and configure the authentication. Copy the following content in the `ConfigureServices` method. The `TokenValidationParameters` class contains important properties used during access token validation.

  * **IssuerSigningKey** : Certificate public key used to check the signature of the access token. Exponent and modulus are part of the public key.
  * **ValidIssuer** : URL of the OPENID server.
  * **ValidAudiences** : List of authorized client identifiers.

```
public void ConfigureServices(IServiceCollection services)
{
	const string modulus = "7jyP7WVsRx9WRj/nvLODxpfWrqtITHtssFc6DC8+FBjwcUAsJE+BOiwbGFoMN6aFgnug3T+EWb4g6UcBrkLlLMNhLLAnE1MvvO5elsaTmIdRNaRKq5W2N1nYZM/Ad17gV5XoXsr82Zl92tHHSbhRTRYIAWUevXA8IOMEw+Q1TeBtIGGAjweclkliNb2T69PitHC4AD1CjuHkrEO7LbmZgfsj+F/RjnD+/6MJ0E9KSiJPJ0RFxzsC72NR2uquDDOBxWluUEgXRFgqd1s/D/t/FehPEgfc5Iy88xOQkD/k3SN8xqeopaZD8OdMwxdGNMjwyD5cw80jlH0lXRLTYK0aiQ==";
	const string exponent = "AQAB";
	var rsaParameters = new RSAParameters
	{
		Modulus = Convert.FromBase64String(modulus),
		Exponent = Convert.FromBase64String(exponent)
	};
	var oauthRsaSecurityKey = new RsaSecurityKey(rsaParameters);
	services.AddAuthentication()
		.AddJwtBearer(cfg =>
		{
			cfg.TokenValidationParameters = new TokenValidationParameters
			{
				ValidIssuer = "https://localhost:5001",
				ValidAudiences = new List<string>
				{
					"console"
				},
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = oauthRsaSecurityKey
			};
		});
	services.AddAuthorization(o => o.AddPolicy("GetWeather", p => p.RequireClaim("scope", "get_weather")));
	services.AddControllers();
	services.AddSwaggerGen(c =>
	{
		c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
	});
}
```

* In the `Configure` procedure add the following statement to enable the authentication.

```
app.UseAuthentication();
```

* In a command prompt, navigate to the directory `src\WebApi` and run the application under the port `7000`.

```
dotnet run --urls=https://localhost:7000
```

### Create Console client

When the REST.API is configured and started, a console application client can be developed to access to the protected operation.

* Open a command prompt and navigate to the `src` subfolder of your project.
* Create a console application, its name must be `Console`.

```
mkdir Console

dotnet new console -n Console
```

* Add the `Console` project into your Visual Studio solution.

```
cd ..

dotnet sln add ./src/Console/Console.csproj
```

* Edit the `Program.cs` file and replace its content with the following code. The grant type `password` is used to authenticate the user and get his access token.

```
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
            var tokenResponse = httpClient.PostAsync("https://localhost:5001/token", new FormUrlEncodedContent(form)).Result;
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
```

* Run the console application. Weather information should be displayed.

![Weather Information](images/openid-11.png)

