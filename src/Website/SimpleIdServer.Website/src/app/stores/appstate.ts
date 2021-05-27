import { createSelector } from '@ngrx/store';
import * as fromApplications from './applications/reducers';
import * as fromScopes from './scopes/reducers';

export interface AppState {
  application: fromApplications.ApplicationState;
  applications: fromApplications.SearchApplicationsState;
  oauthScopes: fromScopes.OAuthScopesState;
}

export const selectApplication = (state: AppState) => state.application;
export const selectApplications = (state: AppState) => state.applications;
export const selectOAuthScopes = (state: AppState) => state.oauthScopes;

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

export const selectOAuthScopesResult = createSelector(
    selectOAuthScopes,
  (state: fromScopes.OAuthScopesState) => {
    if (!state || state.Scopes === null) {
      return null;
    }

    return state.Scopes;
  }
);

export const appReducer = {
  application: fromApplications.getApplicationReducer,
  applications: fromApplications.getSearchApplicationsReducer,
  oauthScopes: fromScopes.getOAuthScopesReducer
};
