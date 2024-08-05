# Single-Page Application (SPA)

When it comes to protecting a single page application (SPA) with OpenID, the recommended grant type to use is the **Authorization Code Flow with PKCE (Proof Key for Code Exchange)**. This flow provides a higher level of security for SPAs compared to other grant types, such as the Implicit Flow, which is now considered less secure.

Here's why the Authorization Code Flow with PKCE is recommended for SPAs:

1. **Enhanced Security**: The Authorization Code Flow with PKCE provides an additional layer of security by using a dynamically generated code verifier and code challenge during the authentication process. This mitigates the risks associated with storing sensitive information like client secrets in the SPA.
2. **Support for Refresh Tokens**: With this flow, you have the ability to obtain a refresh token along with the access token. A refresh token allows the SPA to obtain a new access token without requiring user interaction, improving the user experience by reducing the frequency of logins.
3. **Compliance with OAuth 2.0 Standards**: The Authorization Code Flow with PKCE aligns with the OAuth 2.0 standard. It ensures that your authentication implementation is consistent and interoperable across different platforms and providers.

:::info
The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ProjectSPA).
:::

To implement the Authorization Code Flow with PKCE in your SPA, you'll need to follow the following steps.

## 1. Configure an application

Utilize the administration UI to configure a new OpenID client :

1. Open the IdentityServer website at [https://localhost:5002/master/clients](https://localhost:5002/master/clients).
2. On the Clients screen, click on the `Add client` button.
3. Select `User Agent Base application` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Parameter        | Value                         |
| ---------------- | ----------------------------- |
| Identifier       | protectedSpa                  |
| Name             | protectedSpa                  |
| Redirection URLS | https://localhost:44457       |

## 2. Create Angular application

Finally, create and configure an Angular project.

1. Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir ProjectSPA
cd ProjectSPA
mkdir src
dotnet new sln -n ProjectSPA
```

2. Create a web project named `Website` and install the `angular-oauth2-oidc` npm package.

```
cd src
dotnet new angular -n Website
cd Website\ClientApp
npm i angular-oauth2-oidc@14.0.1 --save
```

3. Add the `Website` project into your Visual Studio solution.

```
cd ..\..\..
dotnet sln add ./src/Website/Website.csproj
```

4. Edit the file `ClientApp\src\app\app.module.ts` and import the `OAuthModule` module.

```
@NgModule({
  declarations: [],
  imports: [
    OAuthModule.forRoot()
  ],
  providers: [],
  bootstrap: []
})
export class AppModule { }
```

5. Create an `auth-config.ts` file in the directory `ClientApp\src\app`, and copy the following code. The file contains the authentication settings, such as the URL of the Identity Provider and the client identifier.

```
import { AuthConfig } from 'angular-oauth2-oidc';

export const authCodeFlowConfig: AuthConfig = {
  issuer: 'https://localhost:5001/master',
  redirectUri: window.location.origin,
  clientId: 'protectedSpa',
  responseType: 'code',
  scope: 'openid profile',
  showDebugInformation: true,
};
```

6. Edit the `ClientApp\src\app\nav-menu\nav-menu.component.ts` file. Import the `authCodeFlowConfig` JSON object, inject the `OAuthService` into the constructor, and add a `login` procedure. This procedure will be called to initiate the authentication workflow.

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
  name: string = "";

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

7. Edit the `ClientApp\src\app\nav-menu\nav-menu.component.html` file and add a login button.

```
<li class="nav-item" *ngIf="!isConnected">
  <a class="nav-link text-dark" (click)="login($event)">Authenticate</a>
</li>
<li class="nav-item" *ngIf="isConnected">
  <a class="nav-link text-dark">Welcome {{name}}</a>
</li>
```

8. In a command prompt, navigate to the `src\Website` directory and launch the project.

```
dotnet run --urls=http://localhost:4200
```

9. Navigate to the website at [http://localhost:4200](http://localhost:4200) and authenticate using the following credentials.

| Credential | Value         |
| ---------- | ------------- |
| Login      | administrator |
| Password   | password      |