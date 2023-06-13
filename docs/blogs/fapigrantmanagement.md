# FAPI Grant Management

> [!WARNING]
> Before you start, Make sure you have an [up and running IdentityServer and IdentityServer website](/documentation/gettingstarted/index.html).
> The goal of this tutorial is to give an overview of the OAUTH2.0 grant management. The sample project is basic and doesn't embrace all the concepts.

Grant Management has been introduced by FAPI 2.0. This standard aims to replace the variety of consent management APIs that have been created in open banking markets.

These capabilities include 

* Granting access and returning a `grant_id`.
* Querying the details of a grant throught a `GET`.
* Update of a grant.
* Deletion of a grant.

The standard also uses Rich Authorization Request (RAR). It allows clients to specify their fine-granted authorization requirements using the expressiveness of JSON data structure, for example 

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

In this tutorial, we are going to explain how to create a `Third Party Website (Server Site Web App)` which will fetch all the Bank Accounts of the authenticated user from a `REST.API service`.

* The Authorization Policy implemented by the REST.API is basic. It checks if the Access Token contains at least one Authorization Data Type equals to `list_accounts`.
* The Third Party Website needs the User's consent to access to the `list_accounts` Authorization Data and to the `openid` and `profile` scopes. When the consent is accepted, the `grant_id` returned by the Identity Server is stored by the Website into an In-Memory store.

The `grant_id` can be used by the website to manage the lifecycle of a grant :

* Querying the grant.
* Revoke the grant.

The Third Party website will have the following configuration

| Configuration                            | Value                                                         |
| ---------------------------------------- | ------------------------------------------------------------- |
| Client Authentication Method             | Client Secret Post                                            |
| Authorization Data Types                 | account_information                                           |
| Scopes                                   | grant_management_query grant_management_revoke openid profile |

## Source code

The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/FapiGrantManagement).

## Add client

The first step consists to configure the OPENID client

* Open the IdentityServer website [http://localhost:5002](http://localhost:5002).
* In the Clients screen, click on `Add client` button.
* Select `web application` and click on next.

![Choose client](images/fapigrant-1.png)

* Fill-in the form like this and click on the `Save` button to confirm the creation. The secret must be equals to `password`.

![Confirm](images/fapigrant-2.png)

## Create REST.API

The second step consists to create and configure a REST.API project.

* Open a command prompt, run the following commands to create the directory structure for the solution.

```
mkdir FapiGrantManagement
cd FapiGrantManagement
mkdir src
dotnet new sln -n FapiGrantManagement
```

* Create a REST.API project and install the `Microsoft.AspNetCore.Authentication.JwtBearer` Nuget Package

```
cd src
dotnet new webapi -n AccountInfoApi
cd AccountInfoApi
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

* Add the `AccountInfoApi` project into your Visual Studio solution.

```
cd ..\..
dotnet sln add ./src/AccountInfoApi/AccountInfoApi.csproj
```

* Edit the file `AccountInfoApi\Program.cs`, configure the JWT authentication and add an Authorization policy named `account_information`, it checks if the Authorization Details are correct.

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

* Add a new controller `AccountInfoController`. It will return the list of Bank Accounts.

```
[ApiController]
[Route("[controller]")]
[Authorize("account_information")]
public class AccountInfoController : ControllerBase
{
    [HttpGet(Name = "Accounts")]
    public IEnumerable<string> Get()
    {
        return new List<string> { "account" };
    }
}
```

Now your REST.API is configured, you can launch it on the port 7001

```
dotnet run --urls=http://localhost:7001
```

## Create Third Party website

Create and configure the Third Party Website 

* Create a web project named `Website`.

```
cd src
dotnet new mvc -n Website
```

* Add the `Website` project into your Visual Studio solution.

```
cd ..
dotnet sln add ./src/Website/Website.csproj
```

* Add a `User` class into the `Models` directory.

```
namespace Website.Models
{
    public class User
    {
        public string Id { get; set; }
        public ICollection<UserGrant> Grants { get; set; } = new List<UserGrant>();

        public UserGrant GetGrant(string bankAccount) => Grants.FirstOrDefault(g => g.BankAccount == bankAccount);
    }

    public class UserGrant
    {
        public string GrantId { get; set; }
        public string BankAccount { get; set; }
    }
}
```

* Add a `CallbackViewModel` into the `Models` directory.

```
namespace Website.Models
{
    public class CallbackViewModel
    {
        public string GrantId { get; set; }
        public IEnumerable<string> Accounts { get; set; }
    }
}
```

* Add a `UserStore` class. It is used to store the `grant_id` of the user.

```
using Website.Models;

namespace Website.Stores
{
    public interface IUserStore
    {
        User Get(string id);
    }

    public class UserStore : IUserStore
    {
        private readonly ICollection<User> _users;

        public UserStore(ICollection<User> users)
        {
            _users = users;
        }

        public User Get(string id) => _users.First(u => u.Id == id);
    }
}
```

* Edit the `Program.cs` class and register the `UserStore` as a Singleton.

```
builder.Services.AddSingleton<IUserStore>(new UserStore(new List<User>
{
    new User { Id = "userId" }
}));
```

* Edit the `HomeController.cs` class and add an action named `AuthenticateBank`. When this action is called, then the user-agent is redirected to the Identity Server of your Bank.

```
public IActionResult AuthenticateBank()
{
    const string bankAccountName = "bank";
    const string userId = "userId";
    const string clientId = "fapiGrant";
    const string callbackUrl = "http://localhost:7000/callback";
    const string authorizationDetails = "{ \"type\" : \"account_information\", \"actions\" : [\"read\"] }";
    var userGrant = _userStore.Get(userId).GetGrant(bankAccountName);
    var url = $"https://localhost:5001/master/authorization?client_id={clientId}&redirect_uri={callbackUrl}&response_type=code&scope=openid profile&authorization_details={authorizationDetails}";
    if (userGrant == null)
        url += "&code&grant_management_action=create";
    return Redirect(url);
}
```

* Add an action named `Callback`. This action is called by the Identity Server after the Authorization is granted. It fetches the `access_token` and `grant_id`, calls the `AccountInfoApi` and displays a view.

```
[Route("callback")]
public async Task<IActionResult> Callback()
{
    const string bankAccountName = "bank";
    const string userId = "userId";
    var user = _userStore.Get(userId);
    var accessToken = await GetAccessToken();
    var accounts = await GetAccounts();
    return View(new CallbackViewModel
    {
        GrantId = user.GetGrant(bankAccountName).GrantId,
        Accounts = accounts
    });

    async Task<string> GetAccessToken()
    {
        var authorizationCode = Request.Query["code"].First();
        using (var httpClient = new HttpClient())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:5001/master/token"),
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", "fapiGrant" },
                { "client_secret", "password" },
                { "grant_type", "authorization_code" },
                { "code", authorizationCode },
                { "redirect_uri", "http://localhost:7000/callback" }
            })
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var jsonObj = JsonObject.Parse(json).AsObject();
            if (jsonObj.ContainsKey("grant_id"))
            {
                var grantId = jsonObj["grant_id"].GetValue<string>();
                user.Grants.Add(new UserGrant
                {
                    BankAccount = bankAccountName,
                    GrantId = grantId
                });
            }
            return jsonObj["access_token"].GetValue<string>();
        }
    }

    async Task<IEnumerable<string>> GetAccounts()
    {
        using(var httpClient = new HttpClient())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:7001/AccountInfo")
            };
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpResponse = await httpClient.SendAsync(requestMessage);
            var json = await httpResponse.Content.ReadAsStringAsync();
            return JsonArray.Parse(json).AsArray().Select(x => x.GetValue<string>());
        }
    }
}
```

* Add a view `Callback.cshtml` into the Directory `Views\Home`. It displays the `grant_id` and also the list of bank accounts.

```
@model Website.Models.CallbackViewModel

<div>The grant identifier is : @Model.GrantId</div>

<h5>List of bank accounts</h5>
<ul>
    @foreach(var bankAccount in Model.Accounts)
    {
        <li>@bankAccount</li>
    }
</ul>
```

* Edit the view `Views\Home\Index.cshtml` and replace the context with the following code

```
@{
    ViewData["Title"] = "Home Page";
}

<div>
    <a href="@Url.Action("AuthenticateBank", "Home")">Bank Account information</a>
</div>
```

Now your REST.API is configured, you can launch it on the port 7000

```
dotnet run --urls=http://localhost:7000
```

Browse the website [http://localhost:7000](http://localhost:7000), click on `Bank Account Information`.
You'll be redirected to the Identity Server, authenticate with the `administrator` credentials and confirm the consent.
At the end you'll see the `grant_id` and the list of Bank Accounts.