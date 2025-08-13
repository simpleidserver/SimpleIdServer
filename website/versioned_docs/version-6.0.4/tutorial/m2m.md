# Machine to Machine (M2M)

Machine-to-machine communication (M2M communication) refers to the direct interaction and exchange of data between two or more machines without human intervention. 
It involves the automated exchange of information, commands, or instructions between devices or systems.

The appropriate grant type to use is typically the `Client Credentials Grant`.  The Client Credentials Grant allows the machine (client) to authenticate itself directly with the authorization server using its own credentials, such as a client ID and client secret. This grant type is suitable when the machine needs to access protected resources on behalf of itself, without involving any end-user interactions.

:::info
The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/RequestAccessTokenM2M).
:::

To implement the M2M communication in a console application, you'll need to follow the following steps.

## 1. Configure a scope

If a `read` scope is configured, then skip this step.

Utilize the administration UI to configure a new scope :

1. Open the IdentityServer website at [https://localhost:5002/master/clients](https://localhost:5002/master/clients).
2. On the Scopes screen, click on the `Add scope` button.
3. Select `API value` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Parameter   | Value |
| ----------- | ----- |
| Name        | read  |
| Description | Read  |

5. Navigate to the new scope, then select the `API Resources` tab and click on the `Add API resource` button.
6. Fill-in the form like this and click on the `Add` button to confirm the creation.

| Parameter | Value    |
| --------- | -------- |
| Name      | shopApi  |
| Value     | Shop API |

## 2. Configure an application

Utilize the administration UI to configure a new OpenID client :

1. Open the IdentityServer website at [https://localhost:5002/master/clients](https://localhost:5002/master/clients).
2. On the Clients screen, click on the `Add client` button.
3. Select `Machine` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Parameter        | Value                              |
| ---------------- | ---------------------------------- |
| Identifier       | m2m                                |
| Secret           | password                           |
| Name             | m2m                                |

5. Click on the new client, then select the `Client scopes` tab and click on the `Add client` scope button. Choose the `read` scope and click on the `Save` button.

## 3. Create a console application

Finally, create and configure a Console Application project.

1. Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir RequestAccessTokenM2M
cd RequestAccessTokenM2M
mkdir src
dotnet new sln -n RequestAccessTokenM2M
```

2. Create a console application project named `ConsoleApp` 

```
cd src
dotnet new console -n ConsoleApp
```

3. Add the `ConsoleApp` project into your Visual Studio solution.

```
cd ..
dotnet sln add ./src/ConsoleApp/ConsoleApp.csproj
```

4. Edit the `Program.cs` file and copy the following code. This code executes an HTTP request to obtain an Access Token.

```
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
```

When you run the Console Application, the Access Token will be displayed.