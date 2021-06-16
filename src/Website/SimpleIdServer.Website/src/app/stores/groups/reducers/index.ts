import { SearchResult } from '@app/stores/applications/models/search.model';
import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/groups.actions';
import { Group } from '../models/group.model';

export interface SearchGroupsState {
  Groups: SearchResult<Group>;
}

export const initialSearchGroupsState: SearchGroupsState = {
  Groups: new SearchResult<Group>()
};

const searchGroupsReducer = createReducer(
  initialSearchGroupsState,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      Groups: { ...content }
    };
  })
);

export function getSearchGroupsReducer(state: SearchGroupsState | undefined, action: Action) {
  return searchGroupsReducer(state, action);
}
