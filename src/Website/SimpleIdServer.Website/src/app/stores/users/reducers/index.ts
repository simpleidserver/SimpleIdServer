import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '@app/stores/applications/models/search.model';
import * as fromActions from '../actions/users.actions';
import { User } from '../models/user.model';

export interface SearchUsersState {
  Users: SearchResult<User>;
}

export interface GetUserState {
  User: User | null;
}

export const initialSearchUsersState: SearchUsersState = {
  Users: new SearchResult<User>()
};

export const initialUserState: GetUserState = {
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

export function getSearchUsersReducer(state: SearchUsersState | undefined, action: Action) {
  return searchUsersReducer(state, action);
}

export function getUserReducer(state: GetUserState | undefined, action: Action) {
  return userReducer(state, action);
}
