import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../applications/models/search.model';
import * as fromActions from '../actions/relyingparty.actions';
import { RelyingParty } from '../models/relyingparty.model';

export interface RelyingPartyState {
  RelyingParty: RelyingParty;
}

export interface SearchRelyingPartiesState {
  RelyingParties: SearchResult<RelyingParty>;
}

export const initialRelyingPartyState: RelyingPartyState = {
  RelyingParty: new RelyingParty()
};

export const initialRelyingPartiesState: SearchRelyingPartiesState = {
  RelyingParties: new SearchResult<RelyingParty>()
};

const searchRelyingPartiesReducer = createReducer(
  initialRelyingPartiesState,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      RelyingParties: { ...content }
    };
  })
);

const relyingPartyReducer = createReducer(
  initialRelyingPartyState,
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      RelyingParty: { ...content }
    };
  })
);

export function getSearchRelyingPartiesReducer(state: SearchRelyingPartiesState | undefined, action: Action) {
  return searchRelyingPartiesReducer(state, action);
}

export function getRelyingPartyReducer(state: RelyingPartyState | undefined, action: Action) {
  return relyingPartyReducer(state, action);
}
