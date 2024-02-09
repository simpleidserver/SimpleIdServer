# Grant Management

Grant Management was introduced by FAPI 2.0. This standard aims to replace the various consent management APIs that have been developed in open banking markets, such as [OPEN BANKING UK](https://www.openbanking.org.uk/).

These capabilities include 

* Granting access and returning a `grant_id`.
* Querying the details of a grant throught a `GET`.
* Update of a grant.
* Deletion of a grant.

The standard also incorporates the use of `Rich Authorization Request (RAR)`. It enables clients to specify their detailed authorization requirements using the expressive nature of JSON data structures. For example:

```
{
   "type":"account_information",
   "actions":[
      "list_accounts",
      "read_balances",
      "read_transactions"
   ],
   "locations":[
      "https://example.com/accounts"
   ]
}
```

For more information please refer to the [official documentation](https://openid.net/specs/fapi-grant-management-01.html).

The OPENBANKING UK Standard identifies two types of Third-Party Providers (`TPPs`):

* Offer Account Information Services (`AISPs`) : gather read-only financial information.. 
* Payment Initiation Services (`PISPs`) : can access and present financial information, but also move money from a user's bank account.

In this tutorial, we will explain how to create an `AISP website` that fetches all the bank accounts of the authenticated user from an `Account REST.API Service`.

* The authorization policy implemented by the REST API is basic. It checks if the access token contains at least one authorization data type equal to list_accounts.
* The Third-Party Website requires the user's consent to access the `list_accounts` Authorization Data as well as the `openid` and `profile` scopes. When the consent is accepted, the Website stores the `grant_id` returned by the Identity Server in an in-memory store.

The `grant_id` can be used by the website to manage the lifecycle of a grant, which includes the following actions:

* Querying the grant.
* Revoke the grant.

The TPP website will have the following configuration:

| Configuration                            | Value                                                         |
| ---------------------------------------- | ------------------------------------------------------------- |
| Client Authentication Method             | tls_client_auth                                               |
| Authorization Signed Response Algorithm  | ES256                                                         | 
| Identity Token Signed Response Algorithm | ES256                                                         |
| Request Object Signed Response Algorithm | ES256                                                         |
| Pushed Authorization Request             | Yes                                                           |
| Response Mode                            | jwt                                                           |
| Authorization Data Types                 | account_information                                           |
| Scopes                                   | grant_management_query grant_management_revoke openid profile |

:::info
The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/FapiGrantManagement).
:::

## 1. Configure client certificate

Utilize the administration UI to create a client certificate.

1. Open the IdentityServer website at [https://localhost:5002](https://localhost:5002).
2. In the Certificate Authorities screen, choose a Certificate Authority from the available options. Remember that the selected Certificate Authority should be trusted by your machine. You can download the certificate and import it into the appropriate Certificate Store.
3. Click on the `Client Certificates` tab and then proceed to click on the `Add Client Certificate` button.
4. Set the value of the Subject Name to `CN=fapiGrant` and click on the `Add` button.
5. Click on the `Download` button located next to the certificate.

## 2. Configure an application

Utilize the administration UI to configure a new OpenID client :

1. Open the IdentityServer website at [https://localhost:5002](https://localhost:5002).
2. In the Clients screen, click on `Add client` button.
3. Select `FAPI2.0`.
4. Select `Grant Management` and click on next.
5. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Property                 | Value                             |
| ------------------------ | --------------------------------- |
| Identifier               | fapiGrant                         |
| Secret                   | password                          |
| Name                     | fapiGrant                         |
| Redirection URLs         | http://localhost:7000/callback/*  |
| Authorization Data Types | account_information               |
| Proof of Possession      | Mutual-TLS Client Authentication  |
| Subject Name             | CN=fapiGrant                      |

## 3. Create Account Info REST.API

Create and configure an Account Info REST.API Service.

1. Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir FapiGrantManagement
cd FapiGrantManagement
mkdir src
dotnet new sln -n FapiGrantManagement
```

2. Create a web project named `AccountInfoApi` and install the `Microsoft.AspNetCore.Authentication.JwtBearer` NuGet package.

```
cd src
dotnet new webapi -n AccountInfoApi
cd AccountInfoApi
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

3. Add the `AccountInfoApi` project into your Visual Studio solution.

```
cd ..\..
dotnet sln add ./src/AccountInfoApi/AccountInfoApi.csproj
```

4. In the file `AccountInfoApi\Program.cs`, modify the code to configure JWT authentication. Additionally, add an Authorization policy named `account_information` that verifies the correctness of the Authorization Details.

```
builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001/master";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidIssuers = new List<string>
            {
                "https://localhost:5001/master"
            }
        };
    });
builder.Services.AddAuthorization(b =>
{
    b.AddPolicy("account_information", p => p.RequireAssertion(c =>
    {
        var cl = c.User.Claims.First(c => c.Type == "authorization_details");
        return JsonObject.Parse(cl.Value)["type"].GetValue<string>() == "account_information";
    }));
});
```

5. Add a new controller called `AccountInfoController`. This controller should be responsible for returning the list of bank accounts.

```
[ApiController]
[Route("[controller]")]
[Authorize("account_information")]
public class AccountInfoController : ControllerBase
{
    [HttpGet(Name = "Accounts")]
    public IEnumerable<string> Get()
    {
        return new List<string> { "BE91798829733676", "BE90321175762332", "BE56631811788388" };
    }
}
```

Now that your REST API is configured, you can launch it on port 7001.

```
dotnet run --urls=http://localhost:7001
```


## 4. Create TPP Website

Finally, create and configure a TPP Website.

1. Create a web project named `Website`.

```
cd src
dotnet new mvc -n Website
```

2. Add the `Website` project into your Visual Studio solution.

```
cd ..
dotnet sln add ./src/Website/Website.csproj
```

3. Add a `BankInfo` class into the `Models` directory.

```
namespace Website.Models;

public class BankInfo
{
    public string Name { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string AuthorizationUrl { get; set; } = null!;
    public string TokenUrl { get; set; } = null!;
    public string GrantId { get; set; } = null!;
}
```

4. Create a `BankInfoStore` class that will be utilized to store the information of banks.

```
using Website.Models;

namespace Website.Stores;

public interface IBankInfoStore
{
    IQueryable<BankInfo> GetAll();
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

public class BankInfoStore : IBankInfoStore
{
    private readonly ICollection<BankInfo> _bankInfos;

    public BankInfoStore(ICollection<BankInfo> bankInfos)
    {
        _bankInfos = bankInfos;
    }

    public IQueryable<BankInfo> GetAll() => _bankInfos.AsQueryable();

    public Task<int> SaveChanges(CancellationToken cancellationToken) => Task.FromResult(1);
}
```

5. Create a `AccessTokenStore` class that will be used to store the access token.

```
namespace Website.Stores;

public class AccessTokenStore
{
    private static AccessTokenStore _instance;
    private Dictionary<string, string> _accessTokens = new Dictionary<string, string>();

    private AccessTokenStore() { }

    public string GetAccessToken(string bankName) => _accessTokens[bankName];

    public void Add(string bankName, string token)
    {
        if(_accessTokens.ContainsKey(bankName)) _accessTokens.Remove(bankName);
        _accessTokens.Add(bankName, token);
    }

    public static AccessTokenStore Instance()
    {
        if(_instance == null) _instance = new AccessTokenStore();
        return _instance;
    }
}
```

6. In the `Program.cs` class, modify the code to register the dependencies and the certificate. Make sure to replace the certificate `CN=fapiGrant.pfx` with the one you downloaded earlier (step 1.5).

```
var certificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "CN=fapiGrant.pfx"));
builder.Services.AddControllersWithViews();
builder.Services.Configure<WebsiteOptions>(o =>
{
    o.MTLSCertificate = certificate;
});
builder.Services.AddSingleton<IBankInfoStore>(new BankInfoStore(new List<BankInfo>
{
    new BankInfo { Name = "Bank", ClientId = "fapiGrant", AuthorizationUrl = "https://localhost:5001/master/authorization", ClientSecret = "password", TokenUrl = "https://localhost:5001/master/token" }
}));
```

7. In the `HomeController.cs` class, include a new action named `Callback`. This action will be invoked by the Identity Server once the authorization is granted. Inside this action, retrieve the `access_token` and `grant_id`, and then utilize the `AccessTokenStore`class to store these values.

```
    [Route("callback/{bankName}")]
    public async Task<IActionResult> Callback(string bankName)
    {
        var bankInfo = _bankInfoStore.GetAll().First(b => b.Name == bankName);
        var accessToken = await GetAccessToken();
        AccessTokenStore.Instance().Add(bankName, accessToken);
        await _bankInfoStore.SaveChanges(CancellationToken.None);
        return RedirectToAction("Index");

        async Task<string> GetAccessToken()
        {
            var authorizationCode = Request.Query["code"].First();
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; },
                CheckCertificateRevocationList = false,
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12
            };
            handler.ClientCertificates.Add(_options.MTLSCertificate);
            using (var httpClient = new HttpClient(handler))
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(bankInfo.TokenUrl),
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "client_id", bankInfo.ClientId },
                        { "grant_type", "authorization_code" },
                        { "code", authorizationCode },
                        { "redirect_uri", $"{_options.CallbackUrl}/{bankName}" }
                    })
                };
                var httpResult = await httpClient.SendAsync(requestMessage);
                var json = await httpResult.Content.ReadAsStringAsync();
                var jsonObj = JsonObject.Parse(json).AsObject();
                if (jsonObj.ContainsKey("grant_id"))
                {
                    var grantId = jsonObj["grant_id"].GetValue<string>();
                    bankInfo.GrantId = grantId;
                }

                return jsonObj["access_token"].GetValue<string>();
            }
        }
    }
```

8. Create a controller named `BanksController` and paste the following content into it. This controller enables the end-user to link one or multiple bank accounts.

```
namespace Website.Controllers;

public class BanksController : Controller
{
    private readonly IBankInfoStore _bankInfoStore;
    private readonly WebsiteOptions _options;

    public BanksController(IBankInfoStore bankInfoStore, IOptions<WebsiteOptions> options)
    {
        _bankInfoStore = bankInfoStore;
        _options = options.Value;
    }

    public IActionResult Index()
    {
        var bankInfos = _bankInfoStore.GetAll().Select(b => new BankInfoViewModel
        {
            Name = b.Name
        });
        return View(bankInfos);
    }

    public IActionResult Link(string name)
    {
        var bankInfo = _bankInfoStore.GetAll().First(b => b.Name == name);
        const string authorizationDetails = "{ \"type\" : \"account_information\", \"actions\" : [\"read\"] }";
        var url = $"{bankInfo.AuthorizationUrl}?client_id={bankInfo.ClientId}&redirect_uri={_options.CallbackUrl}/{name}&response_type=code&scope=openid profile&authorization_details={authorizationDetails}&grant_management_action=create";
        return Redirect(url);
    }
}
```

9. Create a controller named `AccountsController`. This controller is responsible for displaying the information of the bank account.

```
namespace Website.Controllers;

public class AccountsController : Controller
{
    private readonly IBankInfoStore _bankInfoStore;
    private readonly WebsiteOptions _options;

    public AccountsController(IBankInfoStore bankInfoStore, IOptions<WebsiteOptions> options)
    {
        _bankInfoStore = bankInfoStore;
        _options = options.Value;
    }

    public IActionResult Index()
    {
        var result = _bankInfoStore.GetAll().Where(b => !string.IsNullOrWhiteSpace(b.GrantId)).Select(b => new GrantedBankInfoViewModel
        {
            GrantId = b.GrantId,
            BankName = b.Name
        });
        return View(result);
    }

    public async Task<IActionResult> Details(string bankName)
    {
        var accessToken = AccessTokenStore.Instance().GetAccessToken(bankName);
        using (var httpClient = new HttpClient())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_options.AccountInfoUrl)
            };
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpResponse = await httpClient.SendAsync(requestMessage);
            var json = await httpResponse.Content.ReadAsStringAsync();
            var viewModel = new BankDetailsViewModel
            {
                Name = bankName,
                Accounts = JsonArray.Parse(json).AsArray().Select(x => x.GetValue<string>())
            };
            return View(viewModel);
        }
    }
}
```

9. Finally add the following views :

**Views\Banks\Index.cshtml**

```
@using Website.ViewModels;

@{
    ViewData["Title"] = "Link a bank account";
}

@model IEnumerable<BankInfoViewModel>

<h1>Request account information</h1>

<div class="row">
    @foreach(var bankInfo in Model)
    {
        <a href="@Url.Action("Link", "Banks", new { name = bankInfo.Name })">
            <div class="col">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">@bankInfo.Name</h5>
                    </div>
                </div>
            </div>
        </a>
    }
</div>
```

**Views\Accounts\Index.cshtml**

```
@using Website.ViewModels;

@{
    ViewData["Title"] = "Link a bank account";
}

@model IEnumerable<GrantedBankInfoViewModel>

<h1>You've access to the following bank accounts</h1>

<div class="row">
    @foreach(var bankInfo in Model)
    {
        <a href="@Url.Action("Details", "Accounts", new { bankName = bankInfo.BankName })">
            <div class="col">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">@bankInfo.BankName</h5>
                        <p class="text-muted">The grant_id is <b>@bankInfo.GrantId</b></p>
                    </div>
                </div>
            </div>
        </a>
    }
</div>
```

**Views\Accounts\Details.cshtml**

```
@using Website.ViewModels;

@{
    ViewData["Title"] = "Bank accounts";
}

@model BankDetailsViewModel

<h1>@Model.Name</h1>

<h3>List of accounts</h3>

<ul class="list-group">
    @foreach(var account in Model.Accounts)
    {
        <li class="list-group-item">
            @account
        </li>
    }
</ul>
```

Now that your TPP website configured, you can launch it on port 7000.

```
dotnet run --urls=http://localhost:7000
```

Browse the website [http://localhost:7000](http://localhost:7000) and then navigate to [http://localhost:7000/Banks](http://localhost:7000/Banks).

Next, click on the `Bank` button, which will redirect you to the Identity Server. Authenticate using the following credentials and confirm the consent. 

Upon successful authentication, proceed to navigate to [http://localhost:7000/Accounts/Details?bankName=Bank](http://localhost:7000/Accounts/Details?bankName=Bank), where you will find the list of bank accounts displayed.

| Credential | Value         |
| ---------- | ------------- |
| Login      | administrator |
| Password   | password      |