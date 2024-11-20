﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.Extensions.Options;
using SimpleIdServer.CredentialIssuer.Api.CredentialConf;
using SimpleIdServer.CredentialIssuer.Api.CredentialInstance;
using SimpleIdServer.CredentialIssuer.Api.DeferredCredential;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.CredentialIssuer.Store;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.Website.Stores.CredentialIssuer;

public class CredentialIssuerEffects
{
    private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
    private readonly CredentialIssuerWebsiteOptions _options;

    public CredentialIssuerEffects(
        IWebsiteHttpClientFactory websiteHttpClientFactory,
        IOptions<CredentialIssuerWebsiteOptions> options)
    {
        _websiteHttpClientFactory = websiteHttpClientFactory;
        _options = options.Value;
    }

    [EffectMethod]
    public async Task Handle(GetCredentialConfigurationsAction action, IDispatcher dispatcher)
    {
        var baseUrl = GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(baseUrl)
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var credentialConfigurations = JsonSerializer.Deserialize<List<CredentialConfiguration>>(json);
        dispatcher.Dispatch(new GetCredentialConfigurationsSuccessAction { CredentialConfigurations = credentialConfigurations });
    }

    [EffectMethod]
    public async Task Handle(GetCredentialConfigurationAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(baseUrl)
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var credentialConfiguration = JsonSerializer.Deserialize<CredentialConfiguration>(json);
        dispatcher.Dispatch(new GetCredentialConfigurationSuccessAction { Configuration = credentialConfiguration });
    }

    [EffectMethod]
    public async Task Handle(UpdateCredentialDetailsAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(baseUrl),
            Content = new StringContent(JsonSerializer.Serialize(new UpdateCredentialConfigurationDetailsRequest
            {
                BaseUrl = action.BaseUrl,
                Format = action.Format,
                JsonLdContext = action.JsonLdContext,
                Scope = action.Scope,
                Type = action.Type
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var credentialConfiguration = JsonSerializer.Deserialize<CredentialConfiguration>(json);
            dispatcher.Dispatch(new UpdateCredentialDetailsSuccessAction { CredentialConfiguration = credentialConfiguration });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new UpdateCredentialDetailsErrorAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(AddCredentialDisplayAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}/displays";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(baseUrl),
            Content = new StringContent(JsonSerializer.Serialize(new CredentialConfigurationDisplayRequest
            {
                BackgroundColor = action.BackgroundColor,
                Description = action.Description,
                Locale = action.Locale,
                LogoAltText = action.LogoAltText,
                LogoUrl = action.LogoUrl,
                Name = action.Name,
                TextColor = action.TextColor
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var display = JsonSerializer.Deserialize<CredentialConfigurationTranslation>(json);
            dispatcher.Dispatch(new AddCredentialDisplaySuccessAction { Display = display });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new AddCredentialDisplayErrorAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(UpdateCredentialDisplayAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}/displays/{action.DisplayId}";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(baseUrl),
            Content = new StringContent(JsonSerializer.Serialize(new CredentialConfigurationDisplayRequest
            {
                BackgroundColor = action.BackgroundColor,
                Description = action.Description,
                Locale = action.Locale,
                LogoAltText = action.LogoAltText,
                LogoUrl = action.LogoUrl,
                Name = action.Name,
                TextColor = action.TextColor
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            dispatcher.Dispatch(new UpdateCredentialDisplaySuccessAction { BackgroundColor = action.BackgroundColor, Description = action.Description, DisplayId = action.DisplayId, Id = action.Id, Locale = action.Locale, LogoAltText = action.LogoAltText, LogoUrl = action.LogoUrl, Name = action.Name, TextColor = action.TextColor });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new UpdateCredentialDisplayErrorAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(DeleteCredentialDisplayAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}/displays/{action.DisplayId}";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(baseUrl)
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new DeleteCredentialDisplaySuccessAction { Id = action.Id, DisplayId = action.DisplayId });
    }

    [EffectMethod]
    public async Task Handle(AddCredentialClaimAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}/claims";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(baseUrl),
            Content = new StringContent(JsonSerializer.Serialize(new CredentialConfigurationClaimRequest
            {
                Mandatory = action.Mandatory,
                Name = action.Name,
                SourceUserClaimName = action.SourceUserClaimName,
                ValueType = action.ValueType
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var claim = JsonSerializer.Deserialize<CredentialConfigurationClaim>(json);
            dispatcher.Dispatch(new AddCredentialClaimSuccessAction { Claim = claim });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new AddCredentialClaimFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(DeleteCredentialClaimAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}/claims/{action.ClaimId}";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(baseUrl)
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new DeleteCredentialClaimSuccessAction { Id = action.Id, ClaimId = action.ClaimId });
    }

    [EffectMethod]
    public async Task Handle(AddCredentialClaimTranslationAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}/claims/{action.ClaimId}/translations";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(baseUrl),
            Content = new StringContent(JsonSerializer.Serialize(new CredentialConfigurationClaimDisplayRequest
            {
                Name = action.Name,
                Locale = action.Locale
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var translation = JsonSerializer.Deserialize<CredentialConfigurationTranslation>(json);
            dispatcher.Dispatch(new AddCredentialClaimTranslationSuccessAction { ClaimId = action.ClaimId, Translation = translation });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new AddCredentialClaimTranslationFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(UpdateCredentialClaimTranslationAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}/claims/{action.ClaimId}/translations/{action.TranslationId}";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(baseUrl),
            Content = new StringContent(JsonSerializer.Serialize(new CredentialConfigurationClaimDisplayRequest
            {
                Name = action.Name,
                Locale = action.Locale
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            dispatcher.Dispatch(new UpdateCredentialClaimTranslationSuccessAction { ClaimId = action.ClaimId, Id = action.Id, Locale = action.Locale, Name = action.Name, TranslationId = action.TranslationId });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new UpdateCredentialClaimTranslationFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(DeleteCredentialClaimTranslationAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}/claims/{action.ClaimId}/translations/{action.TranslationId}";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(baseUrl)
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new DeleteCredentialClaimTranslationSuccessAction { ClaimId = action.ClaimId, Id = action.Id, TranslationId = action.TranslationId });
    }

    [EffectMethod]
    public async Task Handle(DeleteCredentialConfigurationAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.Id}";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(baseUrl)
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new DeleteCredentialConfigurationSuccessAction { Id = action.Id });
    }

    [EffectMethod]
    public async Task Handle(AddCredentialConfigurationAction action, IDispatcher dispatcher)
    {
        var baseUrl = GetBaseUrl();
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(baseUrl),
            Content = new StringContent(JsonSerializer.Serialize(new UpdateCredentialConfigurationDetailsRequest
            {
                BaseUrl = action.BaseUrl,
                Format = action.Format,
                JsonLdContext = action.JsonLdContext,
                Scope = action.Scope,
                Type = action.Type
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var credentialConfiguration = JsonSerializer.Deserialize<CredentialConfiguration>(json);
            dispatcher.Dispatch(new AddCredentialConfigurationSuccessAction { CredentialConfiguration = credentialConfiguration });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new AddCredentialConfigurationFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(IssueCredentialInstanceAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{GetBaseUrl()}/{action.CredentialConfigurationId}/issue";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(baseUrl),
            Content = new StringContent(JsonSerializer.Serialize(new IssueCredentialRequest
            {
                CredentialId = action.CredentialId,
                ExpirationDateTime = action.ExpirationDateTime,
                IssueDateTime = action.IssueDateTime,
                Subject = action.Subject
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        try
        {
            httpResult.EnsureSuccessStatusCode();
            var credential = JsonSerializer.Deserialize<Domains.Credential>(json);
            dispatcher.Dispatch(new IssueCredentialInstanceSuccessAction { Credential = credential });
        }
        catch
        {
            var jsonObj = JsonObject.Parse(json);
            dispatcher.Dispatch(new IssueCredentialInstanceFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
        }
    }

    [EffectMethod]
    public async Task Handle(GetCredentialInstancesAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{_options.CredentialIssuerUrl}/credential_instances/.search";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(baseUrl),
            Content = new StringContent(JsonSerializer.Serialize(new SearchCredentialInstancesRequest
            {
                CredentialConfigurationId = action.CredentialConfigurationId,
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var credentials = JsonSerializer.Deserialize<List<Domains.Credential>>(json);
        credentials = credentials ?? new List<Credential>();
        dispatcher.Dispatch(new GetCredentialInstancesSuccessAction { Credentials = credentials });
    }

    [EffectMethod]
    public async Task Handle(RemoveCredentialInstanceAction action, IDispatcher dispatcher)
    {
        var baseUrl = $"{_options.CredentialIssuerUrl}/credential_instances/{action.Id}";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(baseUrl)
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new RemoveCredentialInstanceSuccessAction { Id = action.Id });
    }

    [EffectMethod]
    public async Task Handle(GetDeferredCredentialAction action, IDispatcher dispatcher)
    {
        var url = $"{_options.CredentialIssuerUrl}/deferred_credential/{action.Id}";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DeferredCredential>(json);
        dispatcher.Dispatch(new GetDeferredCredentialSuccessAction { DeferredCredential = result });
    }

    [EffectMethod]
    public async Task Handle(IssueDeferredCredentialAction action, IDispatcher dispatcher)
    {
        var url = $"{_options.CredentialIssuerUrl}/deferred_credential/{action.Id}/issue";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Put,
            Content = new StringContent(JsonSerializer.Serialize(new IssueDeferredCredentialRequest
            {
                Claims = action.Claims
            }), Encoding.UTF8, "application/json")
        };
        await httpClient.SendAsync(requestMessage);
        dispatcher.Dispatch(new IssueDeferredCredentialSuccessAction { Claims = action.Claims, Id = action.Id });
    }

    [EffectMethod]
    public async Task Handle(SearchDeferredCredentialsAction action, IDispatcher dispatcher)
    {
        var url = $"{_options.CredentialIssuerUrl}/deferred_credential/.search";
        var httpClient = await _websiteHttpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(new SearchRequest
            {
                Filter = SanitizeExpression(action.Filter),
                OrderBy = SanitizeExpression(action.OrderBy),
                Skip = action.Skip,
                Take = action.Take
            }), Encoding.UTF8, "application/json")
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var searchResult = JsonSerializer.Deserialize<SearchResult<DeferredCredential>>(json);
        dispatcher.Dispatch(new SearchDeferredCredentialsSuccessAction { DeferredCredentials = searchResult });

        string SanitizeExpression(string expression) => expression.Replace("Value.", "");
    }

    private string GetBaseUrl()
        => $"{_options.CredentialIssuerUrl}/credential_configurations";
}

public class GetCredentialConfigurationsAction { }

public class GetCredentialConfigurationsSuccessAction
{
    public List<CredentialConfiguration> CredentialConfigurations { get; set; }
}

public class RemoveCredentialConfigurationAction
{
    public List<string> Names { get; set; }
}

public class GetCredentialConfigurationAction
{
    public string Id { get; set; }
}

public class GetCredentialConfigurationSuccessAction
{
    public CredentialConfiguration Configuration { get; set; }
}

public class UpdateCredentialDetailsAction
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Format { get; set; }
    public string Scope { get; set; }
    public string JsonLdContext { get; set; }
    public string BaseUrl { get; set; }
}

public class UpdateCredentialDetailsSuccessAction
{
    public CredentialConfiguration CredentialConfiguration { get; set; }
}
 
public class UpdateCredentialDetailsErrorAction
{
    public string ErrorMessage { get; set; }
}

public class AddCredentialDisplayAction
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Locale { get; set; } = null;
    public string? LogoUrl { get; set; } = null;
    public string? LogoAltText { get; set; } = null;
    public string? Description { get; set; } = null;
    public string? BackgroundColor { get; set; } = null;
    public string? TextColor { get; set; } = null;
}

public class AddCredentialDisplaySuccessAction
{
    public CredentialConfigurationTranslation Display { get; set; }
}

public class AddCredentialDisplayErrorAction
{
    public string ErrorMessage { get; set; }
}

public class UpdateCredentialDisplayAction
{
    public string Id { get; set; }
    public string DisplayId { get; set; }
    public string Name { get; set; }
    public string? Locale { get; set; } = null;
    public string? LogoUrl { get; set; } = null;
    public string? LogoAltText { get; set; } = null;
    public string? Description { get; set; } = null;
    public string? BackgroundColor { get; set; } = null;
    public string? TextColor { get; set; } = null;
}

public class UpdateCredentialDisplaySuccessAction
{
    public string Id { get; set; }
    public string DisplayId { get; set; }
    public string Name { get; set; }
    public string? Locale { get; set; } = null;
    public string? LogoUrl { get; set; } = null;
    public string? LogoAltText { get; set; } = null;
    public string? Description { get; set; } = null;
    public string? BackgroundColor { get; set; } = null;
    public string? TextColor { get; set; } = null;
}
public class UpdateCredentialDisplayErrorAction
{
    public string ErrorMessage { get; set; }
}

public class DeleteCredentialDisplayAction
{
    public string Id { get; set; }
    public string DisplayId { get; set; }
}

public class DeleteCredentialDisplaySuccessAction
{
    public string Id { get; set; }
    public string DisplayId { get; set; }
}

public class AddCredentialClaimAction
{
    public string Id { get; set; }
    public string SourceUserClaimName { get; set; }
    public string Name { get; set; }
    public bool? Mandatory { get; set; }
    public string? ValueType { get; set; }
}

public class AddCredentialClaimSuccessAction
{
    public CredentialConfigurationClaim Claim { get; set; }
}

public class AddCredentialClaimFailureAction
{
    public string ErrorMessage { get; set; }
}

public class DeleteCredentialClaimAction
{
    public string Id { get; set; }
    public string ClaimId { get; set; }
}

public class DeleteCredentialClaimSuccessAction
{
    public string Id { get; set; }
    public string ClaimId { get; set; }
}

public class AddCredentialClaimTranslationAction
{
    public string Id { get; set; }
    public string ClaimId { get; set; }
    public string Locale { get; set; }
    public string Name { get; set; }
}

public class AddCredentialClaimTranslationSuccessAction
{
    public string ClaimId { get; set; }
    public CredentialConfigurationTranslation Translation { get; set; }
}

public class AddCredentialClaimTranslationFailureAction
{
    public string ErrorMessage { get; set; }
}

public class UpdateCredentialClaimTranslationAction
{
    public string Id { get; set; }
    public string ClaimId { get; set; }
    public string TranslationId { get; set; }
    public string Locale { get; set; }
    public string Name { get; set; }
}

public class UpdateCredentialClaimTranslationSuccessAction
{
    public string Id { get; set; }
    public string ClaimId { get; set; }
    public string TranslationId { get; set; }
    public string Locale { get; set; }
    public string Name { get; set; }
}

public class UpdateCredentialClaimTranslationFailureAction
{
    public string ErrorMessage { get; set; }
}

public class DeleteCredentialClaimTranslationAction
{
    public string Id { get; set; }
    public string ClaimId { get; set; }
    public string TranslationId { get; set; }
}

public class DeleteCredentialClaimTranslationSuccessAction
{
    public string Id { get; set; }
    public string ClaimId { get; set; }
    public string TranslationId { get; set; }
}

public class DeleteCredentialConfigurationAction
{
    public string Id { get; set; }
}

public class DeleteCredentialConfigurationSuccessAction
{
    public string Id { get; set; }
}

public class AddCredentialConfigurationAction
{
    public string Type { get; set; }
    public string Format { get; set; }
    public string Scope { get; set; }
    public string JsonLdContext { get; set; }
    public string BaseUrl { get; set; }
}

public class AddCredentialConfigurationSuccessAction
{
    public CredentialConfiguration CredentialConfiguration { get; set; }
}

public class AddCredentialConfigurationFailureAction
{
    public string ErrorMessage { get; set; }
}

public class IssueCredentialInstanceAction
{
    public string CredentialConfigurationId { get; set; }
    public string Subject { get; set; }
    public DateTime IssueDateTime { get; set; }
    public DateTime? ExpirationDateTime { get; set; }
    public string CredentialId { get; set; }
}

public class IssueCredentialInstanceSuccessAction
{
    public Domains.Credential Credential { get; set; }
}

public class IssueCredentialInstanceFailureAction
{
    public string ErrorMessage { get; set; }
}

public class GetCredentialInstancesAction
{
    public string CredentialConfigurationId { get; set; }
}

public class GetCredentialInstancesSuccessAction
{
    public List<Domains.Credential> Credentials { get; set; }
}

public class RemoveCredentialInstanceAction
{
    public string Id { get; set; }
}

public class RemoveCredentialInstanceSuccessAction
{
    public string Id { get; set; }
}

public class SearchDeferredCredentialsAction
{
    public string? Filter { get; set; } = null;
    public string? OrderBy { get; set; } = null;
    public int? Skip { get; set; } = null;
    public int? Take { get; set; } = null;
}

public class SearchDeferredCredentialsSuccessAction
{
    public SearchResult<DeferredCredential> DeferredCredentials { get; set; }
}

public class GetDeferredCredentialAction
{
    public string Id { get; set; }
}

public class GetDeferredCredentialSuccessAction
{
    public DeferredCredential DeferredCredential { get; set; }
}

public class IssueDeferredCredentialAction
{
    public string Id { get; set; }
    public Dictionary<string, string> Claims { get; set; }
}

public class IssueDeferredCredentialSuccessAction
{
    public string Id { get; set; }
    public Dictionary<string, string> Claims { get; set; }
}