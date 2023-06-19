# Protect an API

In this tutorial, we will explain how to protect a REST API using ASP.NET Core.

The validation of the JWT token passed in the HTTP request typically involves the following steps :

1. **Signature Verification**: The first step is to verify the token's signature to ensure its integrity and authenticity. The signature is generated using a secret key or public/private key pair. The server or verifier needs access to the appropriate key to verify the signature. This step ensures that the token has not been tampered with during transit.
2. **Expiration Check**: The token includes an expiration time (exp) claim that specifies when the token expires. The verifier checks this claim to ensure that the token has not expired. If the current time is after the expiration time, the token is considered invalid and access is denied.
3. **Issuer Validation**: The token includes an issuer claim (iss) that identifies the entity that issued the token. The verifier checks this claim to ensure that the token is issued by a trusted authority. The verifier compares the issuer claim against a list of trusted issuers or a specific issuer that it expects.
4. **Audience Verification**: The token includes an audience claim (aud) that specifies the intended audience or recipient of the token. The verifier checks this claim to ensure that the token is intended for the specific API or application. The verifier compares the audience claim against the expected audience value or a list of valid audiences.

:::info
The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ProtectRESTApiJWT).
:::

## 1. Define permission

In SimpleIdServer, a permission consists of defining a scope along with its associated resources.
A **scope** defines an action, such as `read` or `delete`, which can be executed by any client that has access to this scope.
A **resource** defines a REST API, such as `shopApi`, that can accept the access token. The resource is identified in the `aud` claim of the access token.

In this tutorial, we will configure a `read` permission for the `shopApi`.

Utilize the administration UI to configure a new permission :

1. Open the IdentityServer website at [http://localhost:5002](http://localhost:5002).
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

7. In the table, select the `shopApi` resource, and click on the Update button. This will assign the resource to the scope.

## 2. Create REST.API

Finally, create and configure a REST.API Service.

1. Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir ProtectRESTApiJWT
cd ProtectRESTApiJWT
mkdir src
dotnet new sln -n ProtectRESTApiJWT
```

2. Create a REST.API project named `ShopApi` and install the `Microsoft.AspNetCore.Authentication.JwtBearer` Nuget package.

```
cd src
dotnet new webapi -n ShopApi
cd ShopApi
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

3. Add the `ShopApi` project into your Visual Studio solution.

```
cd ..\..
dotnet sln add ./src/ShopApi/ShopApi.csproj
```

4. Edit the file `ShopApi\Program.cs` and configure the JWT authentication. Additionally, add an Authorization policy named `read` that checks if the scope is equal to `read`.

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
            ValidAudiences = new List<string>
            {
                "shopApi"
            },
            ValidIssuers = new List<string>
            {
                "https://localhost:5001/master"
            }
        };
    });
builder.Services.AddAuthorization(b =>
{
    b.AddPolicy("read", p => p.RequireClaim("scope", "read"));
});
```

Now your REST.API is configured, the controller or action can be decorate by the `AuthorizeAttribute` with the appropriate authorization policy.
For example, the attribute `[Authorize("read")]` checks if the JWT contains a scope equal to  `read`.