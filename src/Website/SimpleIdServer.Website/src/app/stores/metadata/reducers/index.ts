import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/metadata.actions';
import { Language } from '../models/language.model';

export interface LanguagesState {
  Languages: Array<Language>;
}

export interface WellKnownConfigurationState {
  Configuration: any;
}

export const initialLanguagesState: LanguagesState = {
  Languages: []
};

export const initialWellKnownConfiguration: WellKnownConfigurationState = {
  Configuration: {}
};

const languagesReducer = createReducer(
  initialLanguagesState,
  on(fromActions.completeGetLanguages, (state, { content }) => {
    return {
      ...state,
      Languages: [ ...content ]
    };
  })
);

const wellKnownConfigurationReducer = createReducer(
  initialWellKnownConfiguration,
  on(fromActions.completeGetWellKnownConfiguration, (state, { content }) => {
    return {
      ...state,
      Configuration: { ...content }
    };
  })
);

export function getLanguagesReducer(state: LanguagesState | undefined, action: Action) {
  return languagesReducer(state, action);
}

export function getWellKnownConfiguration(state: WellKnownConfigurationState | undefined, action: Action) {
  return wellKnownConfigurationReducer(state, action);
}
