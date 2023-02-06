// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    public static class UserReducer
    {
        #region SearchUsersState

        [ReducerMethod]
        public static SearchUsersState ReduceSearchUsersAction(SearchUsersState state, SearchUsersAction act) => new(isLoading: true, users: new List<User>());

        [ReducerMethod]
        public static SearchUsersState ReduceSearchUsersSuccessAction(SearchUsersState state, SearchUsersSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                Users = act.Users.Select(c => new SelectableUser(c)),
                Count = act.Users.Count()
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
            selectedUser.Value.UpdateDateTime = DateTime.UtcNow;
            return state with
            {
                Users = users
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
            state.User.UpdateDateTime = DateTime.UtcNow;
            return state;
        }

        [ReducerMethod]
        public static UserState ReduceRevokeUserConsentSuccessAction(UserState state, RevokeUserConsentSuccessAction act)
        {
            var consent = state.User.Consents.Single(c => c.Id == act.ConsentId);
            state.User.Consents.Remove(consent);
            return state;
        }

        [ReducerMethod]
        public static UserState ReduceUnlinkExternalAuthProviderSuccessAction(UserState state, UnlinkExternalAuthProviderSuccessAction act)
        {
            var externalAuthProvider = state.User.ExternalAuthProviders.Single(c => c.Subject == act.Subject && c.Scheme == act.Scheme);
            state.User.ExternalAuthProviders.Remove(externalAuthProvider);
            return state;
        }

        [ReducerMethod]
        public static UserState ReduceRevokeUserSessionSuccessAction(UserState state, RevokeUserSessionSuccessAction act)
        {
            var session = state.User.Sessions.Single(s => s.SessionId == act.SessionId);
            session.State = UserSessionStates.Rejected;
            return state;
        }

        #endregion

        #region UpdateUserState

        [ReducerMethod]
        public static UpdateUserState ReduceUpdateUserDetailsAction(UpdateUserState state, UpdateUserDetailsAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceUpdateUserDetailsAction(UpdateUserState state, UpdateUserDetailsSuccessAction act) => new(false);

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
        public static UpdateUserState ReduceRevokeUserSessionSuccessAction(UpdateUserState state, RevokeUserSessionSuccessAction act) => new(false);

        [ReducerMethod]
        public static UpdateUserState ReduceRevokeUserSessionAction(UpdateUserState state, UpdateUserClaimsAction act) => new(true);

        [ReducerMethod]
        public static UpdateUserState ReduceRevokeUserSessionSuccessAction(UpdateUserState state, UpdateUserClaimsSuccessAction act) => new(false);

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
            var claims = action.Claims.Select(c => new SelectableUserClaim(c));
            return state with
            {
                UserClaims = claims,
                Count = claims.Count()
            };
        }

        #endregion
    }
}
