import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/scope.actions';
import { OAuthScope } from '../models/oauthscope.model';

export interface OAuthScopesState {
  Scopes: Array<OAuthScope>;
}

export const initialOAuthScopesState: OAuthScopesState = {
  Scopes: []
};

const oauthScopesReducer = createReducer(
  initialOAuthScopesState,
  on(fromActions.completeGetAllScopes, (state, { content }) => {
    return {
      ...state,
      Scopes: [...content]
    };
  })
);

export function getOAuthScopesReducer(state: OAuthScopesState | undefined, action: Action) {
  return oauthScopesReducer(state, action);
}
