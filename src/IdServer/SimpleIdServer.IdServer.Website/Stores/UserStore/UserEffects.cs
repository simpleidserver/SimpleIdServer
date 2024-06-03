// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using Radzen;
using SimpleIdServer.IdServer.Api.Users;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.Website.Stores.Base;
using SimpleIdServer.IdServer.Website.Stores.ScopeStore;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    public class UserEffects
    {
        private readonly IWebsiteHttpClientFactory _websiteHttpClientFactory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public UserEffects(IWebsiteHttpClientFactory websiteHttpClientFactory, ProtectedSessionStorage sessionStorage, IOptions<IdServerWebsiteOptions> websiteOptions)
        {
            _websiteHttpClientFactory = websiteHttpClientFactory;
            _sessionStorage = sessionStorage;
            _options = websiteOptions.Value;
        }

        [EffectMethod]
        public async Task Handle(SearchUsersAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/.search"),
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
            var searchResult = SidJsonSerializer.Deserialize<SearchResult<Domains.User>>(json);
            dispatcher.Dispatch(new SearchUsersSuccessAction { Users = searchResult.Content, Count = searchResult.Count });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(SearchUserSessionsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}/sessions/.search"),
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
            var searchResult = SidJsonSerializer.Deserialize<SearchResult<Domains.UserSession>>(json);
            dispatcher.Dispatch(new SearchUserSessionsSuccessAction { UserSessions = searchResult.Content, Count = searchResult.Count });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(GetUserAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}"),
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var user = SidJsonSerializer.Deserialize<Domains.User>(json);
            dispatcher.Dispatch(new GetUserSuccessAction { User = user });
        }

        [EffectMethod]
        public async Task Handle(UpdateUserDetailsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var req = new UpdateUserRequest
            {
                Email = action.Email,
                Lastname = action.Lastname,
                Name = action.Firstname,
                NotificationMode = action.NotificationMode
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}"),
                Method = HttpMethod.Put,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            try
            {
                httpResult.EnsureSuccessStatusCode();
                dispatcher.Dispatch(new UpdateUserDetailsSuccessAction { Email = action.Email, Firstname = action.Firstname, Lastname = action.Lastname, UserId = action.UserId, NotificationMode = action.NotificationMode });
            }
            catch
            {
                var json = await httpResult.Content.ReadAsStringAsync();
                var jObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new UpdateUserDetailsFailureAction { ErrorMessage = jObj["error_description"].GetValue<string>() });
            }
        }

        [EffectMethod]
        public async Task Handle(RevokeUserConsentAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}/consents/{action.ConsentId}"),
                Method = HttpMethod.Delete
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new RevokeUserConsentSuccessAction { ConsentId = action.ConsentId, UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(UnlinkExternalAuthProviderAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var req = new UnlinkExternalAuthProviderRequest
            {
                Scheme = action.Scheme,
                Subject = action.Subject
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}/authproviders/unlink"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UnlinkExternalAuthProviderSuccessAction { Scheme = action.Scheme, Subject = action.Subject, UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(RevokeUserSessionAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}/sessions/{action.SessionId}"),
                Method = HttpMethod.Delete
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new RevokeUserSessionSuccessAction { SessionId = action.SessionId, UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(RevokeUserSessionsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}/sessions"),
                Method = HttpMethod.Delete
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new RevokeUserSessionsSuccessAction { UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(UpdateUserClaimsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var fileteredClaims = action.Claims.Where(c => !string.IsNullOrWhiteSpace(c.Value) && !string.IsNullOrWhiteSpace(c.Name));
            var req = new UpdateUserClaimsRequest
            {
                Claims = fileteredClaims
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}/claims"),
                Method = HttpMethod.Put,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new UpdateUserClaimsSuccessAction { Claims = fileteredClaims.ToList(), UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(AddUserCredentialAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var req = new AddUserCredentialRequest
            {
                Credential = action.Credential,
                Active = action.IsDefault
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}/credentials"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var newCredential = SidJsonSerializer.Deserialize<UserCredential>(json);
            dispatcher.Dispatch(new AddUserCredentialSuccessAction { Credential = newCredential, IsDefault = action.IsDefault });
        }

        [EffectMethod]
        public async Task Handle(UpdateUserCredentialAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var req = new UpdateUserCredentialRequest
            {
                OTPAlg = action.Credential.OTPAlg,
                Value = action.Credential.Value
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}/credentials/{action.Credential.Id}"),
                Method = HttpMethod.Put,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var newCredential = SidJsonSerializer.Deserialize<UserCredential>(json);
            dispatcher.Dispatch(new UpdateUserCredentialSuccessAction { Credential = action.Credential });
        }

        [EffectMethod]
        public async Task Handle(RemoveUserCredentialAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}/credentials/{action.CredentialId}"),
                Method = HttpMethod.Delete
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new RemoveUserCredentialSuccessAction { CredentialId = action.CredentialId });
        }

        [EffectMethod]
        public async Task Handle(DefaultUserCredentialAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}/credentials/{action.CredentialId}/default"),
                Method = HttpMethod.Get
            };
            await httpClient.SendAsync(requestMessage);
            dispatcher.Dispatch(new DefaultUserCredentialSuccessAction { CredentialId = action.CredentialId, UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedUserGroupAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach(var groupId in action.GroupIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{action.UserId}/groups/{groupId}"),
                    Method = HttpMethod.Delete
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedUserGroupSuccessAction { GroupIds = action.GroupIds, UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(AssignUserGroupsAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var groups = new List<Domains.Group>();
            foreach(var groupId in action.GroupIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{action.UserId}/groups/{groupId}"),
                    Method = HttpMethod.Post
                };
                var httpResult = await httpClient.SendAsync(requestMessage);
                var json = await httpResult.Content.ReadAsStringAsync();
                groups.Add(SidJsonSerializer.Deserialize<Domains.Group>(json));
            }

            dispatcher.Dispatch(new AssignUserGroupsSuccessAction { Groups = groups, UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(ResolveUserRolesAction action, IDispatcher dispatcher)
        {
            if (!action.IsSelected) return;
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseUrl}/{action.UserId}/roles"),
                Method = HttpMethod.Get
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var roles = SidJsonSerializer.Deserialize<IEnumerable<string>>(json);
            dispatcher.Dispatch(new ResolveUserRolesSuccessAction { Roles = roles, UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedUsersAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            foreach(var userId in action.UserIds)
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{baseUrl}/{userId}"),
                    Method = HttpMethod.Delete
                };
                await httpClient.SendAsync(requestMessage);
            }

            dispatcher.Dispatch(new RemoveSelectedUsersSuccessAction { UserIds = action.UserIds });
        }

        [EffectMethod]
        public async Task Handle(AddUserAction action, IDispatcher dispatcher)
        {
            var baseUrl = await GetUsersUrl();
            var httpClient = await _websiteHttpClientFactory.Build();
            var req = new RegisterUserRequest
            {
                Claims = new Dictionary<string, string>(),
                Email = action.Email,
                Firstname = action.Firstname,
                Lastname = action.Lastname,
                Name = action.Name
            };
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(baseUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            try
            {
                httpResult.EnsureSuccessStatusCode();
                var newUser = SidJsonSerializer.Deserialize<Domains.User>(json);
                dispatcher.Dispatch(new AddUserSuccessAction()
                {
                    Id = newUser.Id,
                    Email = action.Email,
                    Firstname = action.Firstname,
                    Lastname = action.Lastname,
                    Name = action.Name
                });
            }
            catch
            {
                var jObj = JsonObject.Parse(json);
                dispatcher.Dispatch(new AddUserFailureAction { ErrorMessage = jObj["error_description"].GetValue<string>() });
            }
        }

        private async Task<string> GetUsersUrl()
        {
            var baseUrl = await GetBaseUrl();
            return $"{baseUrl}/users";
        }

        private async Task<string> GetBaseUrl()
        {
            if(_options.IsReamEnabled)
            {
                var realm = await GetRealm();
                return $"{_options.IdServerBaseUrl}/{realm}";
            }

            return $"{_options.IdServerBaseUrl}";
        }

        private async Task<string> GetRealm()
        {
            if (!_options.IsReamEnabled) return SimpleIdServer.IdServer.Constants.DefaultRealm;
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class SearchUsersAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchUsersSuccessAction
    {
        public IEnumerable<Domains.User> Users { get; set; } = new List<Domains.User>();
        public int Count { get; set; }
    }

    public class SearchUserSessionsAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
        public string UserId { get; set; }
    }

    public class SearchUserSessionsSuccessAction
    {
        public IEnumerable<Domains.UserSession> UserSessions { get; set; } = new List<Domains.UserSession>();
        public int Count { get; set; }
    }

    public class ToggleUserSelectionAction
    {
        public bool IsSelected { get; set; } = false;
        public string UserId { get; set; } = null!;
    }

    public class ToggleAllUserSelectionAction
    {
        public bool IsSelected { get; set; } = false;
    }

    public class GetUserAction
    {
        public string UserId { get; set; } = null!;
    }

    public class GetUserSuccessAction
    {
        public Domains.User User { get; set; } = null!;
    }

    public class GetUserFailureAction
    {
        public string UserId { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class UpdateUserDetailsAction
    {
        public string UserId { get; set; } = null!;
        public string? Email { get; set; } = null;
        public string? Firstname { get; set; } = null;
        public string? Lastname { get; set; } = null;
        public string? NotificationMode { get; set; } = null;
    }

    public class UpdateUserDetailsSuccessAction
    {
        public string UserId { get; set; } = null!;
        public string? Email { get; set; } = null;
        public string? Firstname { get; set; } = null;
        public string? Lastname { get; set; } = null;
        public string? NotificationMode { get; set; } = null;
    }

    public class UpdateUserDetailsFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class RevokeUserConsentAction
    {
        public string UserId { get; set; } = null!;
        public string ConsentId { get; set; } = null!;
    }

    public class RevokeUserConsentSuccessAction
    {
        public string UserId { get; set; } = null!;
        public string ConsentId { get; set; } = null!;
    }

    public class UnlinkExternalAuthProviderAction
    {
        public string UserId { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Scheme { get; set; } = null!;
    }

    public class UnlinkExternalAuthProviderSuccessAction
    {
        public string UserId { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Scheme { get; set; } = null!;
    }

    public class RevokeUserSessionAction
    {
        public string UserId { get; set; } = null!;
        public string SessionId { get; set; } = null!;
    }

    public class RevokeUserSessionSuccessAction
    {
        public string UserId { get; set; } = null!;
        public string SessionId { get; set; } = null!;
    }

    public class UpdateUserClaimsAction
    {
        public string UserId { get; set; } = null!;
        public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
    }

    public class UpdateUserClaimsSuccessAction
    {
        public string UserId { get; set; } = null!;
        public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
    }

    public class AddUserClaimAction
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
    }

    public class RemoveUserClaimAction
    {
        public string Id { get; set; } = null!;
    }

    public class AddUserCredentialAction
    {
        public string UserId { get; set; } = null!;
        public bool IsDefault { get; set; } = false;
        public UserCredential Credential { get; set; } = null!;
    }

    public class AddUserCredentialSuccessAction
    {
        public UserCredential Credential { get; set; } = null!;
        public bool IsDefault { get; set; } = false;
    }

    public class UpdateUserCredentialAction
    {
        public string UserId { get; set; } = null!;
        public UserCredential Credential { get; set; } = null!;
    }

    public class UpdateUserCredentialSuccessAction
    {
        public UserCredential Credential { get; set; } = null!;
    }

    public class RemoveUserCredentialAction
    {
        public string UserId { get; set; } = null!;
        public string CredentialId { get; set; } = null!;
    }

    public class RemoveUserCredentialSuccessAction
    {
        public string CredentialId { get; set; } = null!;
    }

    public class DefaultUserCredentialAction
    {
        public string UserId { get; set; } = null!;
        public string CredentialId { get; set; } = null!;
    }

    public class DefaultUserCredentialSuccessAction
    {
        public string UserId { get; set; } = null!;
        public string CredentialId { get; set; } = null!;
    }

    public class ToggleAllUserGroupsAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleUserGroupAction
    {
        public string GroupId { get; set; }
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedUserGroupAction
    {
        public IEnumerable<string> GroupIds { get; set; }
        public string UserId { get; set; }
    }

    public class RemoveSelectedUserGroupSuccessAction
    {
        public string UserId { get; set; }
        public IEnumerable<string> GroupIds { get; set; }
    }

    public class AssignUserGroupsAction
    {
        public string UserId { get; set; }
        public IEnumerable<string> GroupIds { get; set; }
    }

    public class AssignUserGroupsSuccessAction
    {
        public string UserId { get; set; }
        public IEnumerable<SimpleIdServer.IdServer.Domains.Group> Groups { get; set; }
    }

    public class ResolveUserRolesAction
    {
        public string UserId { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ResolveUserRolesSuccessAction
    {
        public string UserId { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }

    /// <summary>
    /// Represents the information needed to add an user.
    /// </summary>
    public class AddUserAction
    {
        /// <summary>
        /// The username to use when login.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// (Optional) The user's first name.
        /// </summary>
        public string? Firstname { get; set; } = null;

        /// <summary>
        /// (Optional) The user's last name.
        /// </summary>
        public string? Lastname { get; set; } = null;

        /// <summary>
        /// (Optional) The user's email.
        /// </summary>
        public string? Email { get; set; } = null;
    }

    /// <summary>
    /// Information about the succesfull user addition.
    /// </summary>
    public class AddUserSuccessAction
    {
        /// <summary>
        /// The generated user's TechnicalId.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The username to use when login.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// (Optional) The user's first name.
        /// </summary>
        public string? Firstname { get; set; } = null;

        /// <summary>
        /// (Optional) The user's last name.
        /// </summary>
        public string? Lastname { get; set; } = null;

        /// <summary>
        /// (Optional) The user's email.
        /// </summary>
        public string? Email { get; set; } = null;
    }

    /// <summary>
    /// Information about the failed user addition.
    /// </summary>
    public class AddUserFailureAction : FailureAction
    {

    }

    public class RemoveSelectedUsersAction
    {
        public IEnumerable<string> UserIds { get; set; }
    }

    public class RemoveSelectedUsersSuccessAction
    {
        public IEnumerable<string> UserIds { get; set; }
    }

    public class RevokeUserSessionsAction
    {
        public string UserId { get; set; }
    }

    public class RevokeUserSessionsSuccessAction
    {
        public string UserId { get; set; }
    }

    public class StartAddUserAction
    {

    }
}
