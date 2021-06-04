import { createSelector } from '@ngrx/store';
import * as fromApplications from './applications/reducers';
import * as fromMetadata from './metadata/reducers';
import * as fromScopes from './scopes/reducers';

export interface AppState {
  application: fromApplications.ApplicationState;
  applications: fromApplications.SearchApplicationsState;
  languages: fromMetadata.LanguagesState;
  oauthScopes: fromScopes.OAuthScopesState;
  wellKnownConfiguration: fromMetadata.WellKnownConfigurationState
}

export const selectApplication = (state: AppState) => state.application;
export const selectApplications = (state: AppState) => state.applications;
export const selectLanguages = (state: AppState) => state.languages;
export const selectOAuthScopes = (state: AppState) => state.oauthScopes;
export const selectWellKnownConfiguration = (state: AppState) => state.wellKnownConfiguration;

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

export const selectOAuthScopesResult = createSelector(
    selectOAuthScopes,
  (state: fromScopes.OAuthScopesState) => {
    if (!state || state.Scopes === null) {
      return null;
    }

    return state.Scopes;
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

export const appReducer = {
  application: fromApplications.getApplicationReducer,
  applications: fromApplications.getSearchApplicationsReducer,
  languages: fromMetadata.getLanguagesReducer,
  oauthScopes: fromScopes.getOAuthScopesReducer,
  wellKnownConfiguration: fromMetadata.getWellKnownConfiguration
};
