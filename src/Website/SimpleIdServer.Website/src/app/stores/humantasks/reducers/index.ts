import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/humantasks.actions';
import { HumanTaskDef } from '../models/humantaskdef.model';
import { SearchHumanTaskDefsResult } from '../models/searchhumantaskdef.model';

export interface HumanTaskDefLstState {
  content: SearchHumanTaskDefsResult | null,
  lst: HumanTaskDef[]
}

export interface HumanTaskState {
  content: HumanTaskDef | null
}

export const initialHumanTaskLstState: HumanTaskDefLstState = {
  content: null,
  lst: []
};

export const initialHumanTaskState: HumanTaskState = {
  content: null
};

const humanTaskLstReducer = createReducer(
  initialHumanTaskLstState,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      content: { ...content }
    };
  }),
  on(fromActions.completeGetAll, (state, { content }) => {
    return {
      ...state,
      lst: [ ...content ]
    };
  })
);

const humanTaskReducer = createReducer(
  initialHumanTaskState,
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      content: { ...content }
    };
  })
);

export function getHumanTaskLstReducer(state: HumanTaskDefLstState | undefined, action: Action) {
  return humanTaskLstReducer(state, action);
}

export function getHumanTaskReducer(state: HumanTaskState | undefined, action: Action) {
  return humanTaskReducer(state, action);
}
