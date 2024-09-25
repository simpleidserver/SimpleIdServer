// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.FastFed.ApplicationProvider.Provisioning.Scim;
using SimpleIdServer.FastFed.Domains;
using SimpleIdServer.FastFed.Host.Acceptance.Tests;
using SimpleIdServer.FastFed.Store.EF;
using System.Collections.Generic;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAntiforgery();
builder.Services.AddAuthorization(b =>
{
    b.AddPolicy("IsAdminUser", b =>
    {
        b.RequireAssertion(_ => true);
    });
    b.AddPolicy("IsAdminScope", b =>
    {
        b.RequireAssertion(_ => true);
    });
});
builder.Services.AddFastFed(cb =>
{
    cb.ProviderDomain = "localhost";
    cb.AppProvider = new SimpleIdServer.FastFed.AppProviderOptions
    {
        Capabilities = new SimpleIdServer.FastFed.Domains.Capabilities
        {
            ProvisioningProfiles = new List<string>
        {
            "urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise"
        },
            SchemaGrammars = new List<string>
        {
            "urn:ietf:params:fastfed:1.0:schemas:scim:2.0"
        },
            SigningAlgorithms = new List<string>
        {
            "RS256"
        }
        },
        ContactInformation = new SimpleIdServer.FastFed.Domains.ProviderContactInformation
        {
            Email = "support@example.com",
            Organization = "Example Inc.",
            Phone = "+1-800-555-5555"
        },
        DisplaySettings = new SimpleIdServer.FastFed.Domains.DisplaySettings
        {
            DisplayName = "Example Application Provider",
            LogoUri = "https://play-lh.googleusercontent.com/1-hPxafOxdYpYZEOKzNIkSP43HXCNftVJVttoo4ucl7rsMASXW3Xr6GlXURCubE1tA=w3840-h2160-rw",
            License = "https://openid.net/intellectual-property/licenses/fastfed/1.0/",
        }
    };
    cb.IdProvider = new SimpleIdServer.FastFed.IdProviderOptions
    {
        Capabilities = new SimpleIdServer.FastFed.Domains.Capabilities
        {
            ProvisioningProfiles = new List<string>
        {
            "urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise"
        },
            SchemaGrammars = new List<string>
        {
            "urn:ietf:params:fastfed:1.0:schemas:scim:2.0"
        },
            SigningAlgorithms = new List<string>
        {
            "RS256"
        }
        },
        ContactInformation = new SimpleIdServer.FastFed.Domains.ProviderContactInformation
        {
            Email = "support@example.com",
            Organization = "Example Inc.",
            Phone = "+1-800-555-5555"
        },
        DisplaySettings = new SimpleIdServer.FastFed.Domains.DisplaySettings
        {
            DisplayName = "Example Identity Provider",
            LogoUri = "https://play-lh.googleusercontent.com/1-hPxafOxdYpYZEOKzNIkSP43HXCNftVJVttoo4ucl7rsMASXW3Xr6GlXURCubE1tA=w3840-h2160-rw",
            License = "https://openid.net/intellectual-property/licenses/fastfed/1.0/",
        }
    };
}).AddFastFedApplicationProvider(cbChooser: (t) => t.UseInMemoryEfStore(Constants.ProviderFederations), callback: cb =>
{
    cb.AuthScheme = new SimpleIdServer.FastFed.ApplicationProvider.AuthSchemeOptions
    {
        Cookie = "Cookie1"
    };
})
.AddFastFedIdentityProvider(callback: cb =>
{
    cb.SigningCredentials = new List<Microsoft.IdentityModel.Tokens.SigningCredentials>
    {
        Constants.SigningCredentials
    };
    cb.AuthScheme = new SimpleIdServer.FastFed.IdentityProvider.AuthSchemeOptions
    {
        Cookie = "Cookie2"
    };
})
.AddAppProviderScimProvisioning(o =>
{
    o.ScimServiceUri = "http://localhost/scim";
    o.Scope = "scim";
    o.TokenEndpoint = "http://localhost/token";
});
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapGet("/bad/provider-metadata", () =>
{
    var providerMetadata = new ProviderMetadata
    {
        IdentityProvider = new IdentityProviderMetadata { }
    };
    return JsonSerializer.Serialize(providerMetadata);
});
app.MapGet("/badsuffix/provider-metadata", () =>
{
    var providerMetadata = new ProviderMetadata
    {
        IdentityProvider = new IdentityProviderMetadata
        {
            EntityId = "entityid",
            ProviderDomain = "invalid",
            Capabilities = new SimpleIdServer.FastFed.Domains.Capabilities
            {
                ProvisioningProfiles = new List<string>
                {
                    "urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise"
                },
                SchemaGrammars = new List<string>
                {
                    "urn:ietf:params:fastfed:1.0:schemas:scim:2.0"
                },
                SigningAlgorithms = new List<string>
                {
                    "RS256"
                }
            },
            ContactInformation = new SimpleIdServer.FastFed.Domains.ProviderContactInformation
            {
                Email = "support@example.com",
                Organization = "Example Inc.",
                Phone = "+1-800-555-5555"
            },
            DisplaySettings = new SimpleIdServer.FastFed.Domains.DisplaySettings
            {
                DisplayName = "Example Identity Provider",
                LogoUri = "https://play-lh.googleusercontent.com/1-hPxafOxdYpYZEOKzNIkSP43HXCNftVJVttoo4ucl7rsMASXW3Xr6GlXURCubE1tA=w3840-h2160-rw",
                License = "https://openid.net/intellectual-property/licenses/fastfed/1.0/",
            }
        }
    };
    return JsonSerializer.Serialize(providerMetadata);
});
app.MapGet("/duplicate/provider-metadata", () =>
{
    var providerMetadata = new ProviderMetadata
    {
        IdentityProvider = new IdentityProviderMetadata
        {
            EntityId = "duplicate",
            ProviderDomain = "localhost",
            Capabilities = new SimpleIdServer.FastFed.Domains.Capabilities
            {
                ProvisioningProfiles = new List<string>
                {
                    "urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise"
                },
                SchemaGrammars = new List<string>
                {
                    "urn:ietf:params:fastfed:1.0:schemas:scim:2.0"
                },
                SigningAlgorithms = new List<string>
                {
                    "RS256"
                }
            },
            ContactInformation = new SimpleIdServer.FastFed.Domains.ProviderContactInformation
            {
                Email = "support@example.com",
                Organization = "Example Inc.",
                Phone = "+1-800-555-5555"
            },
            DisplaySettings = new SimpleIdServer.FastFed.Domains.DisplaySettings
            {
                DisplayName = "Example Identity Provider",
                LogoUri = "https://play-lh.googleusercontent.com/1-hPxafOxdYpYZEOKzNIkSP43HXCNftVJVttoo4ucl7rsMASXW3Xr6GlXURCubE1tA=w3840-h2160-rw",
                License = "https://openid.net/intellectual-property/licenses/fastfed/1.0/",
            }
        }
    };
    return JsonSerializer.Serialize(providerMetadata);
});
app.MapGet("/incompatible/provider-metadata", () =>
{
    var providerMetadata = new ProviderMetadata
    {
        IdentityProvider = new IdentityProviderMetadata
        {
            EntityId = "incompatible",
            ProviderDomain = "localhost",
            Capabilities = new SimpleIdServer.FastFed.Domains.Capabilities
            {
                ProvisioningProfiles = new List<string>
                {
                    "invalid:provisioning"
                },
                SchemaGrammars = new List<string>
                {
                    "invalid:schemagrammar"
                },
                SigningAlgorithms = new List<string>
                {
                    "invalid-sigalg"
                }
            },
            ContactInformation = new SimpleIdServer.FastFed.Domains.ProviderContactInformation
            {
                Email = "support@example.com",
                Organization = "Example Inc.",
                Phone = "+1-800-555-5555"
            },
            DisplaySettings = new SimpleIdServer.FastFed.Domains.DisplaySettings
            {
                DisplayName = "Example Identity Provider",
                LogoUri = "https://play-lh.googleusercontent.com/1-hPxafOxdYpYZEOKzNIkSP43HXCNftVJVttoo4ucl7rsMASXW3Xr6GlXURCubE1tA=w3840-h2160-rw",
                License = "https://openid.net/intellectual-property/licenses/fastfed/1.0/",
            }
        },
    };
    return JsonSerializer.Serialize(providerMetadata);
});
app.MapGet("/bad/app-provider-metadata", () =>
{
    var providerMetadata = new ProviderMetadata
    {
        ApplicationProvider = new ApplicationProviderMetadata
        {
            EntityId = "incompatible",
            ProviderDomain = "localhost",
            Capabilities = new SimpleIdServer.FastFed.Domains.Capabilities
            {
                ProvisioningProfiles = new List<string>
                {
                    "invalid:provisioning"
                },
                SchemaGrammars = new List<string>
                {
                    "invalid:schemagrammar"
                },
                SigningAlgorithms = new List<string>
                {
                    "invalid-sigalg"
                }
            },
            ContactInformation = new SimpleIdServer.FastFed.Domains.ProviderContactInformation
            {
                Email = "support@example.com",
                Organization = "Example Inc.",
                Phone = "+1-800-555-5555"
            },
            DisplaySettings = new SimpleIdServer.FastFed.Domains.DisplaySettings
            {
                DisplayName = "Example Identity Provider",
                LogoUri = "https://play-lh.googleusercontent.com/1-hPxafOxdYpYZEOKzNIkSP43HXCNftVJVttoo4ucl7rsMASXW3Xr6GlXURCubE1tA=w3840-h2160-rw",
                License = "https://openid.net/intellectual-property/licenses/fastfed/1.0/",
            }
        },
    };
    return JsonSerializer.Serialize(providerMetadata);
});
app.UseFastFed()
    .UseApplicationProvider()
    .UseIdentityProvider();
app.MapControllerRoute(
    name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

public partial class Program { }