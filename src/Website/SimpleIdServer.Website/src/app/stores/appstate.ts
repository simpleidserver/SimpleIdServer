import { createSelector } from '@ngrx/store';
import * as fromApplications from './applications/reducers';
import * as fromMetadata from './metadata/reducers';
import * as fromScopes from './scopes/reducers';
import * as fromUsers from './users/reducers';
import * as fromGroups from './groups/reducers';

export interface AppState {
  application: fromApplications.ApplicationState;
  applications: fromApplications.SearchApplicationsState;
  languages: fromMetadata.LanguagesState;
  searchOAuthScopes: fromScopes.SearchOAuthScopesState,
  oauthScopes: fromScopes.OAuthScopesState;
  oauthScope: fromScopes.OAuthScopeState;
  wellKnownConfiguration: fromMetadata.WellKnownConfigurationState,
  users: fromUsers.SearchUsersState,
  user: fromUsers.GetUserState,
  groups: fromGroups.SearchGroupsState
}

export const selectApplication = (state: AppState) => state.application;
export const selectApplications = (state: AppState) => state.applications;
export const selectLanguages = (state: AppState) => state.languages;
export const selectSearchOAuthScopes = (state: AppState) => state.searchOAuthScopes;
export const selectOAuthScopes = (state: AppState) => state.oauthScopes;
export const selectOAuthScope = (state: AppState) => state.oauthScope;
export const selectWellKnownConfiguration = (state: AppState) => state.wellKnownConfiguration;
export const selectUsers = (state: AppState) => state.users;
export const selectUser = (state: AppState) => state.user;
export const selectGroups = (state: AppState) => state.groups;

export const selectApplicationResult = createSelector(
  selectApplication,
  (state: fromApplications.ApplicationState) => {
    if (!state || state.Application === null) {
      return null;
    }

    return state.Application;
  }
);

export const selectApplicationsResult = createSelector(
  selectApplications,
  (state: fromApplications.SearchApplicationsState) => {
    if (!state || state.Applications === null) {
      return null;
    }

    return state.Applications;
  }
);

export const selectLanguagesResult = createSelector(
  selectLanguages,
  (state: fromMetadata.LanguagesState) => {
    if (!state || state.Languages === null) {
      return null;
    }

    return state.Languages;
  }
);

export const selectSearchOAuthScopesResult = createSelector(
  selectSearchOAuthScopes,
  (state: fromScopes.SearchOAuthScopesState) => {
    if (!state || state.Scopes === null) {
      return null;
    }

    return state.Scopes;
  }
);

export const selectOAuthScopesResult = createSelector(
    selectOAuthScopes,
  (state: fromScopes.OAuthScopesState) => {
    if (!state || state.Scopes === null) {
      return null;
    }

    return state.Scopes;
  }
);

export const selectOAuthScopeResult = createSelector(
  selectOAuthScope,
  (state: fromScopes.OAuthScopeState) => {
    if (!state || state.Scope === null) {
      return null;
    }

    return state.Scope;
  }
);

export const selectWellKnownConfigurationResult = createSelector(
  selectWellKnownConfiguration,
  (state: fromMetadata.WellKnownConfigurationState) => {
    if (!state || state.Configuration === null) {
      return null;
    }

    return state.Configuration;
  }
);

export const selectUsersResult = createSelector(
  selectUsers,
  (state: fromUsers.SearchUsersState) => {
    if (!state || state.Users === null) {
      return null;
    }

    return state.Users;
  }
);

export const selectUserResult = createSelector(
  selectUser,
  (state: fromUsers.GetUserState) => {
    if (!state || state.User === null) {
      return null;
    }

    return state.User;
  }
);

export const selectGroupsResult = createSelector(
  selectGroups,
  (state: fromGroups.SearchGroupsState) => {
    if (!state || !state.Groups) {
      return null;
    }

    return state.Groups;
  }
);

export const appReducer = {
  application: fromApplications.getApplicationReducer,
  applications: fromApplications.getSearchApplicationsReducer,
  languages: fromMetadata.getLanguagesReducer,
  searchOAuthScopes: fromScopes.getSearchOauthScopesReducer,
  oauthScopes: fromScopes.getOAuthScopesReducer,
  oauthScope: fromScopes.getOAuthScopeReducer,
  wellKnownConfiguration: fromMetadata.getWellKnownConfiguration,
  users: fromUsers.getSearchUsersReducer,
  user: fromUsers.getUserReducer,
  groups: fromGroups.getSearchGroupsReducer
};
