import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/delegateconfigurations.actions';
import { DelegateConfiguration } from '../models/delegateconfiguration.model';
import { SearchDelegateConfigurationResult } from '../models/searchdelegateconfiguration.model';

export interface DelegateConfigurationLstState {
  lstIds: string[];
  content: SearchDelegateConfigurationResult | null
}

export interface DelegateConfigurationState {
  content: DelegateConfiguration | null
}

export const initialDelegateConfigurationLstState: DelegateConfigurationLstState = {
  lstIds: [],
  content: null
};

export const initialDelegateConfigurationState: DelegateConfigurationState = {
  content: null
};

const delegateConfigurationLstReducer = createReducer(
  initialDelegateConfigurationLstState,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      content: { ...content }
    };
  }),
  on(fromActions.completeGetAll, (state, { content }) => {
    return {
      ...state,
      lstIds: [...content ]
    };
  })
);

const delegateConfigurationReducer = createReducer(
  initialDelegateConfigurationState,
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      content: { ...content }
    };
  })
);

export function getDelegateConfigurationLstReducer(state: DelegateConfigurationLstState | undefined, action: Action) {
  return delegateConfigurationLstReducer(state, action);
}

export function getDelegateConfigurationReducer(state: DelegateConfigurationState | undefined, action: Action) {
  return delegateConfigurationReducer(state, action);
}
