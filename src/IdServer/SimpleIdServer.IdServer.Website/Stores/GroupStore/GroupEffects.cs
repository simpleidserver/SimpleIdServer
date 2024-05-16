// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Groups;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.GroupStore
{
    public interface IGroupService
    {
        Task<GetGroupResult> Get(string id);
    }

    public class GroupEffects : IGroupService
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public GroupEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage) 
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }

        public async Task<GetGroupResult> Get(string id)
        {
            var baseUrl = await GetGroupsUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{id}"),
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var getResult = SidJsonSerializer.Deserialize<GetGroupResult>(json);
            return getResult;
        }

        [EffectMethod]
        public async Task Handle(SearchGroupsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetGroupsUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/.search"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(new SearchGroupsRequest
                {
                    Filter = SanitizeExpression(action.Filter),
                    OrderBy = SanitizeExpression(action.OrderBy),
                    Skip = action.Skip,
                    Take = action.Take,
                    OnlyRoot = action.OnlyRoot
                }), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var searchResult = SidJsonSerializer.Deserialize<SearchResult<Domains.Group>>(json);
            dispatcher.Dispatch(new SearchGroupsSuccessAction { Groups = searchResult.Content, Count = searchResult.Count });

            string SanitizeExpression(string expression) => expression.Replace("Group.", "").Replace("Value", "");
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedGroupsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetGroupsUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach(var fullPath in action.FullPathLst)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/delete"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(JsonSerializer.Serialize(new RemoveGroupRequest
                    {
                        FullPath = fullPath
                    }), Encoding.UTF8, "application/json")
                }; 
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedGroupsSuccessAction { FullPathLst = action.FullPathLst });
        }

        [EffectMethod]
        public async Task Handle(AddGroupAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetGroupsUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(baseUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(new AddGroupRequest
                {
                    Description = action.Description,
                    Name = action.Name,
                    ParentGroupId = action.ParentId
                }), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var newGroup = JsonSerializer.Deserialize<Group>(json);
                dispatcher.Dispatch(new AddGroupSuccessAction
                {
                    Description = action.Description,
                    Id = newGroup.Id,
                    Name = action.Name,
                    ParentGroupId = action.ParentId,
                    FullPath = newGroup.FullPath
                });
            }
            catch
            {
                var jsonObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddGroupFailureAction { ErrorMessage = jsonObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(GetGroupAction action, IDispatcher dispatcher)
        {
            var getResult = await Get(action.Id);
            dispatcher.Dispatch(new GetGroupSuccessAction { Group = getResult.Target, RootGroup = getResult.Root });
        }

        [EffectMethod]
        public async Task Handle(AddGroupRolesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetGroupsUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var roles = new List<Domains.Scope>();
            foreach(var scopeName in action.ScopeNames)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{action.GroupId}/roles"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(JsonSerializer.Serialize(new AddGroupRoleRequest
                    {
                        Scope = scopeName
                    }), Encoding.UTF8, "application/json")
                };
                var httpResult = await httpClient.SendAsync(requestMessage);
                var json = await httpResult.Content.ReadAsStringAsync();
                roles.Add(SidJsonSerializer.Deserialize<Domains.Scope>(json));
            }

            dispatcher.Dispatch(new AddGroupRolesSuccessAction { Roles = roles });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedGroupRolesAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetGroupsUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach (var roleId in action.RoleIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{action.Id}/roles/{roleId}"),
                    Method = HttpMethod.Delete
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedGroupRolesSuccessAction { Id = action.Id, RoleIds = action.RoleIds });
        }

        [EffectMethod]
        public async Task Handle(GetHierarchicalGroupAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetGroupsUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.GroupId}/hierarchy"),
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<GetHierarchicalGroupResult>>(json);
            dispatcher.Dispatch(new GetHierarchicalGroupSuccessAction { GroupId = action.GroupId, Result = result });
        }

        private async Task<string> GetGroupsUrl()
        {
            if (_options.IsReamEnabled)
            {
                var realm = await _sessionStorage.GetAsync<string>("realm");
                var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
                return $"{_options.IdServerBaseUrl}/{realmStr}/groups";
            }

            return $"{_options.IdServerBaseUrl}/groups";
        }
    }

    public class SearchGroupsAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
        public bool OnlyRoot { get; set; } = true;
    }

    public class SearchGroupsSuccessAction
    {
        public IEnumerable<Group> Groups { get; set; } = new List<Group>();
        public int Count { get; set; }
    }

    public class ToggleAllGroupSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleGroupSelectionAction
    {
        public bool IsSelected { get; set; }
        public string GroupId { get; set; }
    }

    public class RemoveSelectedGroupsAction
    {
        public IEnumerable<string> FullPathLst { get; set; }
    }

    public class RemoveSelectedGroupsSuccessAction
    {
        public IEnumerable<string> FullPathLst { get; set; }
    }

    public class AddGroupAction
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentId { get; set; }
    }

    public class AddGroupFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class AddGroupSuccessAction
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentGroupId { get; set; }
        public string FullPath { get; set; }
    }

    public class GetGroupAction
    {
        public string Id { get; set; }
    }

    public class GetGroupSuccessAction
    {
        public Group Group { get; set; }
        public Group RootGroup { get; set; }
    }

    public class GetGroupFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class SelectAllGroupMembersAction
    {
        public bool IsSelected { get; set; }
    }

    public class SelectGroupMemberAction
    {
        public string MemberId { get; set; }
        public bool IsSelected { get; set; }
    }
    
    public class ToggleAllGroupRolesAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleGroupRoleAction
    {
        public bool IsSelected { get; set; }
        public string Id { get; set; }
    }

    public class RemoveSelectedGroupRolesAction
    {
        public string Id { get; set; }
        public IEnumerable<string> RoleIds { get; set; }
    }

    public class RemoveSelectedGroupRolesSuccessAction
    {
        public string Id { get; set; }
        public IEnumerable<string> RoleIds { get; set; }
    }

    public class AddGroupRolesAction
    {
        public string GroupId { get; set; } = null!;
        public IEnumerable<string> ScopeNames { get; set; } = new List<string>();
    }

    public class AddGroupRolesSuccessAction
    {
        public IEnumerable<Scope> Roles { get; set; }
    }

    public class StartAddGroupAction
    {

    }

    public class GetHierarchicalGroupAction
    {
        public string GroupId { get; set; }
    }

    public class GetHierarchicalGroupSuccessAction
    {
        public string GroupId { get; set; }
        public List<GetHierarchicalGroupResult> Result { get; set; }
    }
}
