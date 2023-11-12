// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialTemplates;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.Vc.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.CredentialTemplateStore
{
    public class CredentialTemplateEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public CredentialTemplateEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(SearchCredentialTemplatesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetCredentialTemplatesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/.search"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(new SearchRequest
                {
                    Filter = SanitizeExpression(action.Filter),
                    OrderBy = SanitizeExpression(action.OrderBy)
                }), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var searchResult = JsonSerializer.Deserialize<SearchResult<Domains.CredentialTemplate>>(json);
            dispatcher.Dispatch(new SearchCredentialTemplatesSuccessAction { CredentialTemplates = searchResult.Content, Count = searchResult.Count });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedCredentialTemplatesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetCredentialTemplatesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach(var id in action.CredentialTemplateIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{id}"),
                    Method = HttpMethod.Delete
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedCredentialTemplatesSuccessAction { CredentialTemplateIds = action.CredentialTemplateIds });
        }

        [EffectMethod]
        public async Task Handle(AddW3CCredentialTemplateAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetCredentialTemplatesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var request = new AddW3CCredentialTemplateRequest
            {
                LogoUrl = action.LogoUrl,
                Name = action.Name,
                Type = action.Type
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/w3c"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var credentialTemplate = JsonSerializer.Deserialize<Domains.CredentialTemplate>(json);
                dispatcher.Dispatch(new AddCredentialTemplateSuccessAction { Credential = credentialTemplate });
            }
            catch
            {
                var jObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddCredentialTemplateErrorAction { ErrorMessage = jObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(GetCredentialTemplateAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetCredentialTemplatesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.Id}"),
                Method = HttpMethod.Delete
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var credentialTemplate = JsonSerializer.Deserialize<Domains.CredentialTemplate>(json);
            dispatcher.Dispatch(new GetCredentialTemplateSuccessAction { CredentialTemplate = credentialTemplate });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedCredentialTemplateDisplayAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetCredentialTemplatesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach(var id in action.DisplayIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{action.Id}/displays/{id}"),
                    Method = HttpMethod.Delete
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedCredentialTemplateDisplaySuccessAction { Id = action.Id, DisplayIds = action.DisplayIds });
        }

        [EffectMethod]
        public async Task Handle(AddCredentialTemplateDisplayAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetCredentialTemplatesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var req = new AddCredentialTemplateDisplayRequest
            {
                BackgroundColor = action.BackgroundColor,
                Description = action.Description,
                Locale = action.Locale,
                LogoUrl = action.LogoUrl,
                LogoAltText = action.LogoAltText,
                Name = action.Name,
                TextColor = action.TextColor
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.CredentialTemplateId}/displays"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var display = JsonSerializer.Deserialize<Vc.Models.CredentialTemplateDisplay>(json);
            dispatcher.Dispatch(new AddCredentialTemplateDisplaySuccessAction { Display = display });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedCredentialSubjectsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetCredentialTemplatesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach (var id in action.ParameterIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{action.TechnicalId}/parameters/{id}"),
                    Method = HttpMethod.Delete
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedCredentialSubjectsSuccessAction { ParameterIds = action.ParameterIds, TechnicalId = action.TechnicalId });
        }

        [EffectMethod]
        public async Task Handle(UpdateW3CCredentialTemplateTypesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetCredentialTemplatesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var w3cCredentialTemplate = new W3CCredentialTemplate();
            foreach (var type in action.ConcatenatedTypes.Split(';')) w3cCredentialTemplate.AddType(type);
            var req = new UpdateCredentialTemplateParametersRequest
            {
                Parameters = w3cCredentialTemplate.Parameters.ToList()
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.TechnicalId}/parameters"),
                Method = HttpMethod.Put,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UpdateW3CCredentialTemplateTypesSuccessAction { ConcatenatedTypes = action.ConcatenatedTypes, TechnicalId = action.TechnicalId });
        }

        [EffectMethod]
        public async Task Handle(AddW3CCredentialTemplateCredentialSubjectAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetCredentialTemplatesUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var w3cCredentialTemplate = new W3CCredentialTemplate();
            var parameter = w3cCredentialTemplate.AddCredentialSubject(action.ClaimName, action.Subject);
            var req = new UpdateCredentialTemplateParametersRequest
            {
                Parameters =new List<CredentialTemplateParameter>
                {
                    parameter
                }
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.TechnicalId}/parameters"),
                Method = HttpMethod.Put,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<IEnumerable<CredentialTemplateParameter>>(json);
            dispatcher.Dispatch(new AddW3CCredentialTemplateCredentialSubjectSuccessAction { TechnicalId = action.TechnicalId, ClaimName = action.ClaimName, Subject = action.Subject, ParameterId = res.ElementAt(0).Id });
        }

        private async Task<string> GetCredentialTemplatesUrl()
        {
            if (_options.IsReamEnabled)
            {
                var realm = await _sessionStorage.GetAsync<string>("realm");
                var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.IdServerBaseUrl}/{realmStr}/credential_templates";
            }

            return $"{_options.IdServerBaseUrl}/credential_templates";
        }

        private async Task<string> GetRealm()
        {
            if (!_options.IsReamEnabled) return SimpleIdServer.IdServer.Constants.DefaultRealm;
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class SearchCredentialTemplatesAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
    }

    public class SearchCredentialTemplatesSuccessAction
    {
        public IEnumerable<Domains.CredentialTemplate> CredentialTemplates { get; set; } = new List<Domains.CredentialTemplate>();
        public int Count { get; set; }
    }

    public class ToggleAllCredentialTemplatesAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleCredentialTemplateAction
    {
        public bool IsSelected { get; set; }
        public string CredentialTemplateId { get; set; }
    }

    public class RemoveSelectedCredentialTemplatesAction
    {
        public IEnumerable<string> CredentialTemplateIds { get; set; }
    }

    public class RemoveSelectedCredentialTemplatesSuccessAction
    {
        public IEnumerable<string> CredentialTemplateIds { get; set; }
    }

    public class AddW3CCredentialTemplateAction
    {
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string Type { get; set; }
    }

    public class AddCredentialTemplateSuccessAction
    {
        public Domains.CredentialTemplate Credential { get; set; }
    }

    public class AddCredentialTemplateErrorAction
    {
        public string ErrorMessage { get; set; }
    }

    public class GetCredentialTemplateAction
    {
        public string Id { get; set; }
    }

    public class GetCredentialTemplateSuccessAction
    {
        public Domains.CredentialTemplate CredentialTemplate { get; set; }
    }

    public class ToggleAllCredentialTemplateDisplayAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleCredentialTemplateDisplayAction
    {
        public string Id { get; set; }
        public string DisplayId { get; set; }
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedCredentialTemplateDisplayAction
    {
        public string Id { get; set; }
        public IEnumerable<string> DisplayIds { get; set; } 
    }

    public class RemoveSelectedCredentialTemplateDisplaySuccessAction
    {
        public string Id { get; set; }
        public IEnumerable<string> DisplayIds { get; set; }
    }

    public class AddCredentialTemplateDisplayAction
    {
        public string CredentialTemplateId { get; set; }
        public string Name { get; set; } = null!;
        public string Locale { get; set; } = null!;
        public string? Description { get; set; } = null;
        public string? LogoUrl { get; set; } = null;
        public string? LogoAltText { get; set; } = null;
        public string? BackgroundColor { get; set; } = null;
        public string? TextColor { get; set; } = null;
    }

    public class AddCredentialTemplateDisplaySuccessAction
    {
        public Vc.Models.CredentialTemplateDisplay Display { get; set; }
    }

    public class ToggleAllCredentialSubjectsAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleCredentialSubjectAction
    {
        public string ClaimName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedCredentialSubjectsAction
    {
        public string TechnicalId { get; set; }
        public IEnumerable<string> ParameterIds { get; set; }
    }

    public class RemoveSelectedCredentialSubjectsSuccessAction
    {
        public string TechnicalId { get; set; }
        public IEnumerable<string> ParameterIds { get; set; }
    }

    public class UpdateW3CCredentialTemplateTypesAction
    {
        public string TechnicalId { get; set; }
        public string ConcatenatedTypes { get; set; }
    }

    public class UpdateW3CCredentialTemplateTypesSuccessAction
    {
        public string TechnicalId { get; set; }
        public string ConcatenatedTypes { get; set; }
    }

    public class AddW3CCredentialTemplateCredentialSubjectAction
    {
        public string TechnicalId { get; set; }
        public string ClaimName { get; set; }
        public W3CCredentialSubject Subject { get; set; }
    }

    public class AddW3CCredentialTemplateCredentialSubjectSuccessAction
    {
        public string TechnicalId { get; set; }
        public string ParameterId { get; set; }
        public string ClaimName { get; set; }
        public W3CCredentialSubject Subject { get; set; }
    }
}
