import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/authschemeprovider.actions';
import { AuthSchemeProvider } from '../models/authschemeprovider.model';

export interface AuthSchemeProviderState {
  AuthSchemeProvider: AuthSchemeProvider;
}

export interface AuthSchemeProvidersState {
  AuthSchemeProviders: AuthSchemeProvider[];
}

export const initialAuthSchemeProviderState: AuthSchemeProviderState = {
  AuthSchemeProvider: new AuthSchemeProvider()
};

export const initialAuthSchemeProvidersState: AuthSchemeProvidersState = {
  AuthSchemeProviders: []
};

const authSchemeProviderReducer = createReducer(
  initialAuthSchemeProviderState,
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      AuthSchemeProvider: { ...content }
    };
  }),
  on(fromActions.completeUpdate, (state, { options }) => {
    return {
      ...state,
      AuthSchemeProvider: {
        ...state.AuthSchemeProvider,
        options: options
      }
    };
  }),
);

const authSchemeProvidersReducer = createReducer(
  initialAuthSchemeProvidersState,
  on(fromActions.completeGetAll, (state, { content }) => {
    return {
      ...state,
      AuthSchemeProviders: [...content]
    };
  }),
  on(fromActions.completeDisable, (state, { id }) => {
    const authSchemeProviders = state.AuthSchemeProviders.map(a => ({ ...a }));
    authSchemeProviders.filter((a) => a.id === id)[0].isEnabled = false;
    return {
      ...state,
      AuthSchemeProviders: authSchemeProviders
    };
  }),
  on(fromActions.completeEnable, (state, { id }) => {
    const authSchemeProviders = state.AuthSchemeProviders.map(a => ({ ...a }));
    authSchemeProviders.filter((a) => a.id === id)[0].isEnabled = true;
    return {
      ...state,
      AuthSchemeProviders: authSchemeProviders
    };
  }),
);


export function getAuthSchemeProviderReducer(state: AuthSchemeProviderState | undefined, action: Action) {
  return authSchemeProviderReducer(state, action);
}

export function getAuthSchemeProvidersReducer(state: AuthSchemeProvidersState | undefined, action: Action) {
  return authSchemeProvidersReducer(state, action);
}
