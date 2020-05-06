import { createSelector } from '@ngrx/store';
import * as fromClients from './clients/oauth/reducers';
import * as fromScopes from './scopes/oauth/reducers';

export interface AppState {
    oauthClient: fromClients.State;
    oauthScope : fromScopes.State;
}

export const selectOAuthClient = (state: AppState) => state.oauthClient;
export const selectOAuthScope = (state : AppState) => state.oauthScope;

export const selectOAuthClientsResult = createSelector(
    selectOAuthClient,
    (state: fromClients.State) => {
        if (!state || state.Clients == null) {
            return null;
        }

        return state.Clients;
    }
);

export const selectOAuthClientResult = createSelector(
    selectOAuthClient,
    (state: fromClients.State) => {
        if (!state || state.Client == null) {
            return null;
        }

        return state.Client;
    }
);

export const selectOAuthScopesResult = createSelector(
    selectOAuthScope,
    (state: fromScopes.State) => {
        if (!state || state.Scopes == null) {
            return null;
        }

        return state.Scopes;
    }
);

export const appReducer = {
    oauthClient: fromClients.oauthClientReducer,
    oauthScope: fromScopes.oauthScopeReducer
};