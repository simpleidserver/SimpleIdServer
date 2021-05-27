import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/applications.actions';
import { Application } from '../models/application.model';
import { SearchResult } from '../models/search.model';

export interface ApplicationState {
  Application: Application;
}

export interface SearchApplicationsState {
  Applications: SearchResult<Application>;
}

export const initialClientState: ApplicationState = {
  Application: new Application()
};

export const initialSearchApplicationState: SearchApplicationsState = {
  Applications: new SearchResult<Application>()
};

const searchApplicationsReducer = createReducer(
  initialSearchApplicationState,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      Applications: { ...content }
    };
  })
);

const applicationReducer = createReducer(
  initialClientState,
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      Application: { ...content }
    };
  })
);

export function getSearchApplicationsReducer(state: SearchApplicationsState | undefined, action: Action) {
  return searchApplicationsReducer(state, action);
}

export function getApplicationReducer(state: ApplicationState | undefined, action: Action) {
  return applicationReducer(state, action);
}
