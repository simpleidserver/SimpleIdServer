# Protect application from undesirable users

## Recommended flow by application type

There are different grant-types to get tokens, the choice depends on the type of application.

![Choose grant-type](images/openid-5.png)

| Applications                                 | Recommended Configuration                                                    |
| -------------------------------------------- | ---------------------------------------------------------------------------- |
| Server-Side (Web application) - ASP.NET CORE | **Grant-Type** : authorization code                                          |
| Single Page Application (SPA) - Angular      | **Grant-Type** : authorization code, **Client Authentication Method** : PKCE |
| Native - Mobile, WPF application             | **Grant-Type** : authorization code, **Client Authentication Method** : PKCE |
| Trusted                                      | **Grant-Type** : password                                                    |

## Server-Side application

**Example** : ASP.NET CORE application.

Server-Side application should use *authorization code* grant-type.

> [!WARNING]
> Before you start, Make sure there is a Visual Studio Solution with a configured OpenId server.
	
### Source Code

The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ProtectApplicationFromUndesirableUsers/AspNetCore).
 
### Configure OpenId Server

The first step consists to configure the OPENID client.

* Open the Visual Studio Solution and edit the `OpenIdDefaultConfiguration.cs` file.
* Add a new OpenId client :

```
new OpenIdClient
{
    ClientId = "website",
    ClientSecret = "websiteSecret",
    ApplicationKind = ApplicationKinds.Web,
    TokenEndPointAuthMethod = "client_secret_post",
    ApplicationType = "web",
    UpdateDateTime = DateTime.UtcNow,
    CreateDateTime = DateTime.UtcNow,
    TokenExpirationTimeInSeconds = 60 * 30,
    RefreshTokenExpirationTimeInSeconds = 60 * 30,
    TokenSignedResponseAlg = "RS256",
    IdTokenSignedResponseAlg = "RS256",
    AllowedScopes = new List<OAuthScope>
    {
        SIDOpenIdConstants.StandardScopes.OpenIdScope,
        SIDOpenIdConstants.StandardScopes.Profile,
        SIDOpenIdConstants.StandardScopes.Email
    },
    GrantTypes = new List<string>
    {
        "authorization_code",
    },
    RedirectionUrls = new List<string>
    {
        "https://localhost:7000/signin-oidc"
    },
    PreferredTokenProfile = "Bearer",
    ResponseTypes = new List<string>
    {
        "token",
        "id_token"
    }
}
```

* Run the OPENID server.

```
cd src\OpenId
dotnet run
```

### Create ASP.NET CORE application

The last step consists to create and configure an ASP.NET CORE project.

* Open a command and navigate to the `src` subfolder of your project.
* Create a directory `AspNetCore` and create an ASP.NET CORE project in it :

```
mkdir AspNetCore

dotnet new mvc -n AspNetCore
```

* Navigate to the directory `AspNetCore` and install the Nuget package `Microsoft.AspNetCore.Authentication.OpenIdConnect`.

```
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
```

* Add the `AspNetCore` project into your Visual Studio solution.

```
cd ..\..
dotnet sln add ./src/AspNetCore/AspNetCore.csproj
```

* Edit the `Startup.cs` file and configure the OpenId authentication. In the `ConfigureServices` procedure, add the following code :

```
services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "sid";
})
    .AddCookie("Cookies")
    .AddOpenIdConnect("sid", options =>
    {
        options.SignInScheme = "Cookies";

        options.Authority = "http://localhost:5000";
        options.RequireHttpsMetadata = false;

        options.ClientId = "website";
        options.SaveTokens = true;
    });
```

* To ensure the authentication services execute on each request, add `UseAuthentication` in the `Configure` procedure. The procedure should look like to something like this :

```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	app.UseHttpsRedirection();
	app.UseStaticFiles();
	
	app.UseRouting();
	
	app.UseAuthentication();
	app.UseAuthorization();
	
	app.UseEndpoints(endpoints =>
	{
		endpoints.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");
	});
}
```

* Add a `ClaimsController` with one protected operation :

```
public class ClaimsController : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }
}
```

* Create a new view `Views\Claims\Index.cshtml`. It will display all the claims of the authenticated user.

```
<ul>
    @foreach (var claim in User.Claims)
    {
        <li>@claim.Type : @claim.Value</li>
    }
</ul>
```

* In a command prompt, navigate to the directory `src\AspNetCore` and run the application under the port `7000`.

```
dotnet run --urls=https://localhost:7000
```

* Browse this URL [https://localhost:7000/claims](https://localhost:7000/claims), the User-Agent is automatically redirected to the OPENID server. 
  Submit the credentials - login : `sub`, password : `password` and confirm the consent. You'll be redirected to the following screen where your claims will be displayed.

![Claims](images/openid-6.png)

## Single Page Application (SPA)

**Example**: Angular application.

SPA application use *authorization code* grant-type with *PKCE* client authentication method.

> [!WARNING]
> Before you start, Make sure there is a Visual Studio Solution with a configured OpenId server.
	
### Source Code

The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ProtectApplicationFromUndesirableUsers/Spa).
 
### Configure OpenId Server

The first step consists to configure the OPENID client.

* Open the Visual Studio solution and edit `OpenIdDefaultConfiguration.cs` file.
* Add a new OpenId client:

```
new OpenIdClient
{
    ClientId = "website",
    ClientSecret = "websiteSecret",
    ApplicationKind = ApplicationKinds.SPA,
    TokenEndPointAuthMethod = "pkce",
    ApplicationType = "web",
    UpdateDateTime = DateTime.UtcNow,
    CreateDateTime = DateTime.UtcNow,
    TokenExpirationTimeInSeconds = 60 * 30,
    RefreshTokenExpirationTimeInSeconds = 60 * 30,
    TokenSignedResponseAlg = "RS256",
    IdTokenSignedResponseAlg = "RS256",
    AllowedScopes = new List<OAuthScope>
    {
        SIDOpenIdConstants.StandardScopes.OpenIdScope,
        SIDOpenIdConstants.StandardScopes.Profile,
        SIDOpenIdConstants.StandardScopes.Email,
        SIDOpenIdConstants.StandardScopes.Role
    },
    GrantTypes = new List<string>
    {
        "authorization_code"
    },
    RedirectionUrls = new List<string>
    {
        "http://localhost:4200"
    },
    PreferredTokenProfile = "Bearer",
    ResponseTypes = new List<string>
    {
        "token",
        "id_token",
        "code"
    }
}
```

* Run the OPENID server.

```
cd src\OpenId
dotnet run
```

### Create angular application

The last step consists to create and configure an Angular project.

* Open a command prompt and navigate to the `src` subfolder of your project.
* Create an ASP.NET CORE with angular project, its name must be `Spa`.

```
mkdir Spa

dotnet new angular -n Spa
```

* Navigate to the directory `Spa\ClientApp` and install the npm package `angular-oauth2-oidc`.

```
cd Spa\ClientApp

npm i angular-oauth2-oidc --save
```

* Add the `Spa` project into your Visual Studio solution.

```
cd ..\..\..
dotnet sln add ./src/Spa/Spa.csproj
```

* Edit the file `ClientApp\src\app\app.module.ts` and import the `OAuthModule` module.

```
@NgModule({
  declarations: [
	// etc.
  ],
  imports: [
	// etc.
    OAuthModule.forRoot()
  ],
  providers: [],
  bootstrap: []
})
export class AppModule { }
```

* Create an `auth-config.ts` file in the directory `ClientApp\src\app`, replace its content with the following code. This file contains the authentication settings : Url of the identity provider or the Client Identifier.

```
import { AuthConfig } from 'angular-oauth2-oidc';

export const authCodeFlowConfig: AuthConfig = {
  issuer: 'http://localhost:5000',
  redirectUri: window.location.origin,
  clientId: 'website',
  responseType: 'code',
  scope: 'openid profile email role',
  showDebugInformation: true,
};
```

* Edit the `ClientApp\src\app\nav-menu\nav-menu.component.ts` file, import the `authCodeFlowConfig` JSON object, inject the `OAuthService` into the constructor and add a `login` procedure. This procedure will be called to initiate the authentication workflow.

```
import { OAuthService } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from '../auth-config';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {  
  isConnected: boolean = false;
  name: string;

  constructor(private oauthService: OAuthService) {
    this.oauthService.configure(authCodeFlowConfig);
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
    var claims: any = this.oauthService.getIdentityClaims();
    if (!claims) {
      return;
    }

    this.isConnected = true;
    this.name = claims["sub"];
  }

  login(evt: any) {
    evt.preventDefault();
    this.oauthService.initImplicitFlow();
  }
}
```

* Edit the `ClientApp\src\app\nav-menu\nav-menu.component.html` file and add a login button.

```
<li class="nav-item" *ngIf="!isConnected">
  <a class="nav-link text-dark" (click)="login($event)">Authenticate</a>
</li>
<li class="nav-item" *ngIf="isConnected">
  <a class="nav-link text-dark">Welcome {{name}}</a>
</li>
```

* In a command prompt, navigate to the `src\Spa` directory and launch the project.

```
dotnet run --urls=http://localhost:4200
```

* Navigate to the website [http://localhost:4200](http://localhost:4200) and authenticate with the login : `sub` and password : `password`.

![SPA website](images/openid-7.png)