# Security

By default, when a minimal version of the SCIM server is installed, none of the endpoints are secured. This means that, out of the box, all SCIM operations are publicly accessible—suitable for quick testing or prototyping, but not for production scenarios.

To protect SCIM endpoints using API keys, follow these steps:

## Install the Nuget Package

Add the `SimpleIdServer.Scim.ApiKeyAuth` package to your project. This package provides the middleware and configuration types needed to enforce API key authentication.

```batch title="cmd.exe"
dotnet add package SimpleIdServer.Scim.ApiKeyAuth
```

## Edit Program.cs

In your `Program.cs`, after registering SCIM with `AddScim()`, invoke the `EnableApiKeyAuth()` extension method. This method can be called with or without a parameter of type `ApiKeysConfiguration`.

```csharp title="Program.cs"
services.AddScim()
        .EnableApiKeyAuth()
```

**Without Parameters**: If you call `EnableApiKeyAuth()` without passing an `ApiKeysConfiguration` instance, the SCIM server will use a default set of API keys (see the table below).

**With ApiKeysConfiguration**: You may supply a custom **ApiKeysConfiguration** object that explicitly defines one or more realm-to-key mappings. Each key in the configuration corresponds to a "realm" that you have configured in SCIM. Incoming requests must present one of those valid keys in the Authorization: Bearer KEY header.

| Owner | Value | 
| ----- | ----- |
| IdServer | ba521b3b-02f7-4a37-b03c-58f713bf88e7 |
| AzureAd | 1595a72a-2804-495d-8a8a-2c861e7a736a |