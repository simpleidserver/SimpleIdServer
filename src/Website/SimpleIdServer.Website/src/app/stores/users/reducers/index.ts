import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '@app/stores/applications/models/search.model';
import * as fromActions from '../actions/users.actions';
import { User } from '../models/user.model';

export interface SearchUsersState {
  Users: SearchResult<User>;
}

export const initialSearchUsersState: SearchUsersState = {
  Users: new SearchResult<User>()
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

export function getSearchUsersReducer(state: SearchUsersState | undefined, action: Action) {
  return searchUsersReducer(state, action);
}
