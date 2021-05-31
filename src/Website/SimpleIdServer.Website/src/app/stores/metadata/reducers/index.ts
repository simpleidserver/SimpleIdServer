import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/metadata.actions';
import { Language } from '../models/language.model';

export interface LanguagesState {
  Languages: Array<Language>;
}

export const initialLanguagesState: LanguagesState = {
  Languages: []
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

export function getLanguagesReducer(state: LanguagesState | undefined, action: Action) {
  return languagesReducer(state, action);
}
