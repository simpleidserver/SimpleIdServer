﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Website.Stores.GroupStore;
using System.Data;
using User = SimpleIdServer.IdServer.Domains.User;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    public static class UserReducer
    {
        #region SearchUsersState

        [ReducerMethod]
        public static SearchUsersState ReduceRemoveSelectedUsersAction(SearchUsersState state, RemoveSelectedUsersAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchUsersState ReduceRemoveSelectedUsersSuccessAction(SearchUsersState state, RemoveSelectedUsersSuccessAction action)
        {
            var users = state.Users.ToList();
            users = users.Where(u => !action.UserIds.Contains(u.Value.Id)).ToList();
            return state with
            {
                IsLoading = false,
                Users = users,
                Count = users.Count()
            };
        }

        [ReducerMethod]
        public static SearchUsersState ReduceSearchUsersAction(SearchUsersState state, SearchUsersAction act) => new(isLoading: true, users: new List<User>());

        [ReducerMethod]
        public static SearchUsersState ReduceSearchUsersSuccessAction(SearchUsersState state, SearchUsersSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                Users = act.Users.Select(c => new SelectableUser(c)),
                Count = act.Count
            };
        }

        [ReducerMethod]
        public static SearchUsersState ReduceAddUserFailureAction(SearchUsersState state, AddUserFailureAction act)
        {
            return state with
            {
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static SearchUsersState ReduceAddUserSuccessAction(SearchUsersState state, AddUserSuccessAction act)
        {
            List<SelectableUser> users = state.Users.ToList();
            User newUser = new User()
            {
                Id = act.Id,
                Name = act.Name,
                Firstname = act.Firstname,
                Lastname = act.Lastname,
                Email = act.Email,
                UpdateDateTime = DateTime.Now
            };
            users.Add(new SelectableUser(newUser) { IsNew = true });
            return state with
            {
                IsLoading = false,
                Users = users,
                Count = users.Count
            };
        }

        [ReducerMethod]
        public static SearchUsersState ReduceToggleUserSelectionAction(SearchUsersState state, ToggleUserSelectionAction act)
        {
            var users = state.Users?.ToList();
            if (users == null) return state;
            var selectedUser = users.Single(c => c.Value.Id == act.UserId);
            selectedUser.IsSelected = act.IsSelected;
            return state with
            {
                Users = users
            };
        }

        [ReducerMethod]
        public static SearchUsersState ReduceToggleAllUserSelectionAction(SearchUsersState state, ToggleAllUserSelectionAction act)
        {
            var users = state.Users?.ToList();
            if (users == null) return state;
            foreach (var user in users) user.IsSelected = act.IsSelected;
            return state with
            {
                Users = users
            };
        }

        [ReducerMethod]
        public static SearchUsersState ReduceUpdateUserDetailsAction(SearchUsersState state, UpdateUserDetailsSuccessAction act)
        {
            var users = state.Users?.ToList();
            if (users == null) return state;
            var selectedUser = users.Single(u => u.Value.Id == act.UserId);
            selectedUser.Value.UpdateName(act.Firstname);
            selectedUser.Value.UpdateLastname(act.Lastname);
            selectedUser.Value.UpdateEmail(act.Email);
            selectedUser.Value.NotificationMode = act.NotificationMode;
            selectedUser.Value.UpdateDateTime = DateTime.Now;
            return state with
            {
                Users = users
            };
        }

        #endregion

        #region SearchUserSessionsState

        [ReducerMethod]
        public static SearchUserSessionsState ReduceSearchUserSessionsAction(SearchUserSessionsState state, SearchUserSessionsAction act) => new(isLoading: true, userSessions: new List<UserSession>());

        [ReducerMethod]
        public static SearchUserSessionsState ReduceSearchUserSessionsSuccessAction(SearchUserSessionsState state, SearchUserSessionsSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                UserSessions = act.UserSessions,
                Count = act.Count
            };
        }

        [ReducerMethod]
        public static SearchUserSessionsState ReduceRevokeUserSessionSuccessAction(SearchUserSessionsState state, RevokeUserSessionSuccessAction act)
        {
            var sessions = state.UserSessions;
            var session = sessions.Single(s => s.SessionId == act.SessionId);
            session.State = UserSessionStates.Rejected;
            return state with
            {
                UserSessions = sessions
            };
        }

        [ReducerMethod]
        public static SearchUserSessionsState ReduceRevokeUserSessionsSuccessAction(SearchUserSessionsState state, RevokeUserSessionsSuccessAction act)
        {
            var sessions = state.UserSessions;
            foreach(var session in sessions.Where(s => s.IsActive()))
                session.State = UserSessionStates.Rejected;

            return state with
            {
                UserSessions = sessions
            };
        }

        #endregion

        #region UserState

        [ReducerMethod]
        public static UserState ReduceGetUserAction(UserState state, GetUserAction act) => new(isLoading: true, user: null);

        [ReducerMethod]
        public static UserState ReduceGetUserSuccessAction(UserState state, GetUserSuccessAction act) => state with
        {
            IsLoading = false,
            User = act.User
        };

        [ReducerMethod]
        public static UserState ReduceGetUserFailureAction(UserState state, GetUserFailureAction act) => state with
        {
            IsLoading = false,
            User = null
        };

        [ReducerMethod]
        public static UserState ReduceUpdateUserDetailsAction(UserState state, UpdateUserDetailsSuccessAction act)
        {
            state.User.UpdateEmail(act.Email);
            state.User.UpdateName(act.Firstname);
            state.User.UpdateLastname(act.Lastname);
            state.User.NotificationMode = act.NotificationMode;
            state.User.UpdateDateTime = DateTime.Now;
            return state;
        }

        [ReducerMethod]
        public static UserState ReduceRevokeUserConsentSuccessAction(UserState state, RevokeUserConsentSuccessAction act)
        {
            var user = state.User;
            var consents = user.Consents.ToList();
            consents.Remove(consents.Single(c => c.Id == act.ConsentId));
            user.Consents = consents;
            return state with
            {
                User = user
            };
        }

        [ReducerMethod]
        public static UserState ReduceUnlinkExternalAuthProviderSuccessAction(UserState state, UnlinkExternalAuthProviderSuccessAction act)
        {
            var externalAuthProvider = state.User.ExternalAuthProviders.Single(c => c.Subject == act.Subject && c.Scheme == act.Scheme);
            state.User.ExternalAuthProviders.Remove(externalAuthProvider);
            return state;
        }

        [ReducerMethod]
        public static UserState ReduceUpdateUserCredentialSuccessAction(UserState state, UpdateUserCredentialSuccessAction act)
        {
            var user = state.User;
            var credential = user.Credentials.Single(c => c.Id == act.Credential.Id);
            credential.Value = act.Credential.Value;
            credential.OTPAlg = act.Credential.OTPAlg;
            return state with
            {
                User = user
            };
        }

        [ReducerMethod]
        public static UserState ReduceAddUserCredentialSuccessAction(UserState state, AddUserCredentialSuccessAction act)
        {
            var user = state.User;
            if (act.IsDefault)
            {
                foreach (var c in user.Credentials.Where(c => c.CredentialType == act.Credential.CredentialType))
                    c.IsActive = false;
                act.Credential.IsActive = true;
            }

            user.Credentials.Add(act.Credential);
            return state with
            {
                User = user
            };
        }

        #endregion

        #region UpdateUserState

        [ReducerMethod]
        public static UpdateUserState ReduceStartAddUserAction(UpdateUserState state, StartAddUserAction act)
        {
            return new UpdateUserState(false)
            {
                ErrorMessage = null
            };
        }

        [ReducerMethod]
        public static UpdateUserState ReduceUpdateUserDetailsAction(UpdateUserState state, UpdateUserDetailsAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceUpdateUserDetailsAction(UpdateUserState state, UpdateUserDetailsSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceUpdateUserDetailsAction(UpdateUserState state, UpdateUserDetailsFailureAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceRevokeUserConsentAction(UpdateUserState state, RevokeUserConsentAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceRevokeUserConsentSuccessAction(UpdateUserState state, RevokeUserConsentSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceUnlinkExternalAuthProviderAction(UpdateUserState state, UnlinkExternalAuthProviderAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceUnlinkExternalAuthProviderSuccessAction(UpdateUserState state, UnlinkExternalAuthProviderSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceRevokeUserSessionAction(UpdateUserState state, RevokeUserSessionAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceRevokeUserSessionsAction(UpdateUserState state, RevokeUserSessionsAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceRevokeUserSessionSuccessAction(UpdateUserState state, RevokeUserSessionSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceRevokeUserSessionsSuccessAction(UpdateUserState state, RevokeUserSessionsSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceUpdateUserClaimsAction(UpdateUserState state, UpdateUserClaimsAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceUpdateUserClaimsSuccessAction(UpdateUserState state, UpdateUserClaimsSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceAddUserCredentialAction(UpdateUserState state, AddUserCredentialAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceAddUserCredentialSuccessAction(UpdateUserState state, AddUserCredentialSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceUpdateUserCredentialAction(UpdateUserState state, UpdateUserCredentialAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceUpdateUserCredentialSuccessAction(UpdateUserState state, UpdateUserCredentialSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceRemoveUserCredentialAction(UpdateUserState state, RemoveUserCredentialAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceRemoveUserCredentialSuccessAction(UpdateUserState state, RemoveUserCredentialSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceDefaultUserCredentialAction(UpdateUserState state, DefaultUserCredentialAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceDefaultUserCredentialSuccessAction(UpdateUserState state, DefaultUserCredentialSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceAssignUserGroupsAction(UpdateUserState state, AssignUserGroupsAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceAssignUserGroupsSuccessAction(UpdateUserState state, AssignUserGroupsSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceAddUserAction(UpdateUserState state, AddUserAction act)
            => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceAddUserSuccessAction(UpdateUserState state, AddUserSuccessAction act)
            => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceAddUserFailureAction(UpdateUserState state, AddUserFailureAction act)
            => new(false) { ErrorMessage = act.ErrorMessage, IsUpdating = false };

        #endregion

        #region UserClaims

        [ReducerMethod]
        public static UserClaimsState ReduceGetUserSuccessAction(UserClaimsState state, GetUserSuccessAction act)
        {
            var claims = act.User.OAuthUserClaims.Select(c => new SelectableUserClaim(c)).ToList();
            return state with
            {
                UserClaims = claims,
                Count = claims.Count()
            };
        }


        [ReducerMethod]
        public static UserClaimsState ReduceAddUserClaimAction(UserClaimsState state, AddUserClaimAction act)
        {
            var claims = state.UserClaims.ToList();
            claims.Add(new SelectableUserClaim(new UserClaim { Id = Guid.NewGuid().ToString(), Name = act.Key, Value = act.Value }) { IsNew = true });
            return state with
            {
                UserClaims = claims
            };
        }

        [ReducerMethod]
        public static UserClaimsState ReduceRemoveUserClaimAction(UserClaimsState state, RemoveUserClaimAction act)
        {
            var claims = state.UserClaims.ToList();
            var claim = claims.First(c => c.Value.Id == act.Id);
            claims.Remove(claim);
            return state with
            {
                UserClaims = claims
            };
        }

        [ReducerMethod]
        public static UserClaimsState ReduceUpdateUserClaimsSuccessAction(UserClaimsState state, UpdateUserClaimsSuccessAction action)
        {
            var claims = state.UserClaims.Where(c => c.IsRole).ToList();
            foreach(var cl in action.Claims) claims.Add(new SelectableUserClaim(cl));
            return state with
            {
                UserClaims = claims,
                Count = claims.Count()
            };
        }

        [ReducerMethod]
        public static UserClaimsState ReduceResolveUserRolesAction(UserClaimsState state, ResolveUserRolesAction action)
        {
            if (!action.IsSelected)
            {
                var claims = state.UserClaims.ToList();
                claims = claims.Where(c => !c.IsRole).ToList();
                return state with
                {
                    UserClaims = claims
                };
            }

            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static UserClaimsState ReduceResolveUserRolesSuccessAction(UserClaimsState state, ResolveUserRolesSuccessAction action)
        {
            var claims = state.UserClaims.ToList();
            claims.AddRange(action.Roles.Select(r => new SelectableUserClaim(new UserClaim(Guid.NewGuid().ToString(), "role", r))
            {
                IsNew = true,
                IsRole = true
            }).ToList());
            return state with
            {
                UserClaims = claims,
                IsLoading = false,
                Count = claims.Count()
            };
        }

        #endregion

        #region UserCredentials

        [ReducerMethod]
        public static UserCredentialsState ReduceGetUserAction(UserCredentialsState state, GetUserAction act) => new(isLoading: true, userCredentials: new List<UserCredential>());

        [ReducerMethod]
        public static UserCredentialsState ReduceGetUserSuccessAction(UserCredentialsState state, GetUserSuccessAction act)
        {
            var claims = act.User.Credentials.Select(c => new UserCredential
            {
                CredentialType = c.CredentialType,
                IsActive = c.IsActive,
                Id = c.Id,
                OTPAlg = c.OTPAlg,
                OTPCounter = c.OTPCounter,
                Value = c.Value
            });
            return state with
            {
                UserCredentials = claims,
                Count = claims.Count(),
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static UserCredentialsState ReduceAddUserCredentialSuccessAction(UserCredentialsState state, AddUserCredentialSuccessAction act)
        {
            var credentials = state.UserCredentials.ToList();
            if (act.IsDefault)
            {
                if (act.Credential.IsActive)
                    foreach (var a in credentials.Where(c => c.CredentialType == act.Credential.CredentialType))
                        a.IsActive = false;
                act.Credential.IsActive = true;
            }

            credentials.Add(act.Credential);
            return state with
            {
                UserCredentials = credentials,
                Count = credentials.Count()
            };
        }

        [ReducerMethod]
        public static UserCredentialsState ReduceUpdateUserCredentialSuccessAction(UserCredentialsState state, UpdateUserCredentialSuccessAction act)
        {
            var credentials = state.UserCredentials.ToList();
            var credential = credentials.Single(c => c.Id == act.Credential.Id);
            credential.Value = act.Credential.Value;
            credential.OTPAlg = act.Credential.OTPAlg;
            state.UserCredentials = credentials;
            return state with
            {
                UserCredentials = credentials,
            };
        }

        [ReducerMethod]
        public static UserCredentialsState ReduceRemoveUserCredentialSuccessAction(UserCredentialsState state, RemoveUserCredentialSuccessAction act)
        {
            var credentials = state.UserCredentials.ToList();
            var credential = credentials.Single(c => c.Id == act.CredentialId);
            credentials.Remove(credential);
            state.UserCredentials = credentials;
            return state with
            {
                UserCredentials = credentials,
                Count = credentials.Count()
            };
        }

        [ReducerMethod]
        public static UserCredentialsState ReduceDefaultUserCredentialSuccessAction(UserCredentialsState state, DefaultUserCredentialSuccessAction act)
        {
            var credentials = state.UserCredentials.ToList();
            var credential = credentials.Single(c => c.Id == act.CredentialId);
            foreach (var cred in credentials.Where(c => c.CredentialType == credential.CredentialType))
                cred.IsActive = false;
            credential.IsActive = true;
            state.UserCredentials = credentials;
            return state with
            {
                UserCredentials = credentials,
            };
        }

        #endregion

        #region UserGroupsState

        [ReducerMethod]
        public static UserGroupsState ReduceGetUserAction(UserGroupsState state, GetUserAction act) => new(new List<Group>(), true);

        [ReducerMethod]
        public static UserGroupsState ReduceGetUserSuccessAction(UserGroupsState state, GetUserSuccessAction act) => new(act.User.Groups.Select(g => g.Group), false);

        [ReducerMethod]
        public static UserGroupsState ReduceToggleAllUserGroupsAction(UserGroupsState state, ToggleAllUserGroupsAction act)
        {
            var groups = state.Groups.ToList();
            foreach (var group in groups)
                group.IsSelected = act.IsSelected;
            return state with
            {
                Groups = groups
            };
        }

        [ReducerMethod]
        public static UserGroupsState ReduceToggleUserGroupAction(UserGroupsState state, ToggleUserGroupAction act)
        {
            var groups = state.Groups.ToList();
            var group = state.Groups.Single(g => g.Value.Id == act.GroupId);
            group.IsSelected = act.IsSelected;
            return state with
            {
                Groups = groups
            };
        }

        [ReducerMethod]
        public static UserGroupsState ReduceRemoveSelectedUserGroupAction(UserGroupsState state, RemoveSelectedUserGroupAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static UserGroupsState ReduceRemoveSelectedUserGroupSuccessAction(UserGroupsState state, RemoveSelectedUserGroupSuccessAction act)
        {
            var groups = state.Groups.ToList();
            groups = groups.Where(g => !act.GroupIds.Contains(g.Value.Id)).ToList();
            return state with
            {
                IsLoading = false,
                Groups = groups,
                Count = groups.Count()
            };
        }

        [ReducerMethod]
        public static UserGroupsState ReduceAssignUserGroupsAction(UserGroupsState state, AssignUserGroupsAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static UserGroupsState ReduceAssignUserGroupsSuccessAction(UserGroupsState state, AssignUserGroupsSuccessAction act)
        {
            var groups = state.Groups.ToList();
            groups.AddRange(act.Groups.Select(g => new SelectableUserGroup(g) { IsNew = true }).ToList());
            return state with
            {
                IsLoading = false,
                Groups = groups,
                Count = groups.Count()
            };
        }

        [ReducerMethod]
        public static UserGroupsState ReduceSearchGroupsAction(UserGroupsState state, SearchGroupsAction act)
        {
            return state with
            {
                IsEditableGroupsLoading = true
            };
        }

        [ReducerMethod]
        public static UserGroupsState ReduceSearchGroupsSuccessAction(UserGroupsState state, SearchGroupsSuccessAction act)
        {
            var result = act.Groups.OrderBy(s => s.Name).Select(s => new EditableUserGroup(s)
            {
                IsPresent = state.Groups.Any(sc => sc.Value.Id == s.Id)
            }).ToList();
            return state with
            {
                IsEditableGroupsLoading = false,
                EditableGroups = result,
                EditableGroupsCount = act.Count,
                Count = state.Groups.Count(),
                Groups = state.Groups,
                IsLoading = false
            };
        }

        #endregion
    }
}
