import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '@app/stores/applications/models/search.model';
import * as fromActions from '../actions/users.actions';
import { User } from '../models/user.model';
import { UserOpenId } from '../models/user-openid.model';

export interface SearchUsersState {
  Users: SearchResult<User>;
}

export interface GetUserState {
  User: User | null;
}

export interface GetUserOpenIdState {
  User: UserOpenId | null;
}

export const initialSearchUsersState: SearchUsersState = {
  Users: new SearchResult<User>()
};

export const initialUserState: GetUserState = {
  User: null
};

export const initialUserOpenIdState: GetUserOpenIdState = {
  User: null
};

const searchUsersReducer = createReducer(
  initialSearchUsersState,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      Users: { ...content }
    };
  })
);

const userReducer = createReducer(
  initialUserState,
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      User: { ...content }
    };
  })
);

const userOpenIdReducer = createReducer(
  initialUserOpenIdState,
  on(fromActions.completeGetOpenId, (state, { user }) => {
    return {
      ...state,
      User: { ...user }
    };
  })
);

export function getSearchUsersReducer(state: SearchUsersState | undefined, action: Action) {
  return searchUsersReducer(state, action);
}

export function getUserReducer(state: GetUserState | undefined, action: Action) {
  return userReducer(state, action);
}

export function getUserOpenIdReducer(state: GetUserOpenIdState | undefined, action: Action) {
  return userOpenIdReducer(state, action);
}
