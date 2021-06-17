import { SearchResult } from '@app/stores/applications/models/search.model';
import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/groups.actions';
import { Group } from '../models/group.model';

export interface SearchGroupsState {
  Groups: SearchResult<Group> | null;
}

export interface GroupState {
  Group: Group | null;
}

export const initialSearchGroupsState: SearchGroupsState = {
  Groups: null
};

export const initialGroupState: GroupState = {
  Group: null
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

const groupReducer = createReducer(
  initialGroupState,
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      Group: { ...content }
    };
  })
);

export function getSearchGroupsReducer(state: SearchGroupsState | undefined, action: Action) {
  return searchGroupsReducer(state, action);
}

export function getGroupReducer(state: GroupState | undefined, action: Action) {
  return groupReducer(state, action);
}
