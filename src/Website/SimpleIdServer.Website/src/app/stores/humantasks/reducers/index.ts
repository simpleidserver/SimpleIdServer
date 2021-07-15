import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../applications/models/search.model';
import * as fromActions from '../actions/humantasks.actions';
import { HumanTaskDef } from '../models/humantaskdef.model';
import { HumanTaskInstance } from '../models/humantaskinstance.model';
import { SearchHumanTaskDefsResult } from '../models/searchhumantaskdef.model';

export interface HumanTaskDefLstState {
  content: SearchHumanTaskDefsResult | null,
  lst: HumanTaskDef[]
}

export interface HumanTaskState {
  content: HumanTaskDef | null
}

export interface HumanTaskInstanceState {
  rendering: any,
  task: HumanTaskInstance | null
}

export interface HumanTaskInstancesState {
  content: SearchResult<HumanTaskInstance> | null;
}

export const initialHumanTaskLstState: HumanTaskDefLstState = {
  content: null,
  lst: []
};

export const initialHumanTaskState: HumanTaskState = {
  content: null
};

export const initialHumanTaskInstanceState: HumanTaskInstanceState = {
  rendering: null,
  task: null
};

export const initialHumanTaskInstancesState: HumanTaskInstancesState = {
  content : null
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

const humanTaskInstanceReducer = createReducer(
  initialHumanTaskInstanceState,
  on(fromActions.completeGetInstance, (state, { rendering, task }) => {
    return {
      ...state,
      rendering: rendering,
      task : task
    };
  })
);

const humanTaskInstancesReducer = createReducer(
  initialHumanTaskInstancesState,
  on(fromActions.completeSearchInstances, (state, { content }) => {
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

export function getHumanTaskInstanceReducer(state: HumanTaskInstanceState | undefined, action: Action) {
  return humanTaskInstanceReducer(state, action);
}

export function getHumanTaskInstancesReducer(state: HumanTaskInstancesState | undefined, action: Action) {
  return humanTaskInstancesReducer(state, action);
}
