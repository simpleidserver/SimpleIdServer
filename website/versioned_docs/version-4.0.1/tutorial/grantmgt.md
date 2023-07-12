# Grant Management

Grant Management has been introduced by FAPI 2.0. This standard aims to replace the various consent management APIs that have been developed in open banking markets.

These capabilities include 

* Granting access and returning a `grant_id`.
* Querying the details of a grant throught a `GET`.
* Update of a grant.
* Deletion of a grant.

The standard also incorporates the use of Rich Authorization Request (RAR). It enables clients to specify their detailed authorization requirements using the expressive nature of JSON data structures. For example:

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

In this tutorial, we will explain how to create a `Third Party Website (Server Site Web App)` that fetches all the bank accounts of the authenticated user from a `REST.API service`.

* The authorization policy implemented by the REST API is basic. It checks if the access token contains at least one authorization data type equal to list_accounts.
* The Third-Party Website requires the user's consent to access the `list_accounts` Authorization Data as well as the `openid` and `profile` scopes. When the consent is accepted, the Website stores the `grant_id` returned by the Identity Server in an in-memory store.

The `grant_id` can be used by the website to manage the lifecycle of a grant, which includes the following actions:

* Querying the grant.
* Revoke the grant.

The Third-Party website will have the following configuration:

| Configuration                            | Value                                                         |
| ---------------------------------------- | ------------------------------------------------------------- |
| Client Authentication Method             | Client Secret Post                                            |
| Authorization Data Types                 | account_information                                           |
| Scopes                                   | grant_management_query grant_management_revoke openid profile |

:::info
The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/FapiGrantManagement).
:::

## 1. Configure an application

Utilize the administration UI to configure a new OpenID client :

1. Open the IdentityServer website at [https://localhost:5002](https://localhost:5002).
2. In the Clients screen, click on `Add client` button.
3. Select `Web application` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Property                 | Value                           |
| ------------------------ | ------------------------------- |
| Identifier               | fapiGrant                       |
| Secret                   | password                        |
| Name                     | fapiGrant                       |
| Redirection URLs         | http://localhost:7000/callback  |
| Has access to grant API  | true                            |         
| Authorization Data Types | account_information             |

## 2. Create REST.API

Create and configure a REST.API

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
        return new List<string> { "account" };
    }
}
```

Now that your REST API is configured, you can launch it on port 7001.

```
dotnet run --urls=http://localhost:7001
```


## 3. Create Third Party Website

Finally, create and configure a Third Party Website.

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

3. Add a `User` class into the `Models` directory.

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

4. Add a `CallbackViewModel` into the `Models` directory.

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

5. Add a `UserStore` class that will be used to store the `grant_id` of the user.

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

6. In the `Program.cs` class, modify the code to register the `UserStore` as a singleton.

```
builder.Services.AddSingleton<IUserStore>(new UserStore(new List<User>
{
    new User { Id = "userId" }
}));
```

7. In the `HomeController.cs` class, add an action named `AuthenticateBank`. When this action is invoked, it should redirect the user-agent to the Identity Server of your bank.

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

8. Add an action named `Callback` to the corresponding controller. This action will be called by the Identity Server after the authorization is granted. Within the action, retrieve the `access_token` and `grant_id`, make a request to the `AccountInfoApi`, and display the resulting view.

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

9. Add a view named `Callback.cshtml` to the `Views\Home` directory. This view should display the `grant_id` as well as the list of bank accounts.

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

10. Modify the view `Views\Home\Index.cshtml` and replace the existing content with the following code snippet.

```
@{
    ViewData["Title"] = "Home Page";
}

<div>
    <a href="@Url.Action("AuthenticateBank", "Home")">Bank Account information</a>
</div>
```

Now that your REST API is configured, you can launch it on port 7000.

```
dotnet run --urls=http://localhost:7000
```

Browse the website [http://localhost:7000](http://localhost:7000) and click on `Bank Account Information`.
You will be redirected to the Identity Server. Please authenticate using the following credentials and confirm the consent. Upon completion, you will see the `grant_id` along with the list of bank accounts.

| Credential | Value         |
| ---------- | ------------- |
| Login      | administrator |
| Password   | password      |