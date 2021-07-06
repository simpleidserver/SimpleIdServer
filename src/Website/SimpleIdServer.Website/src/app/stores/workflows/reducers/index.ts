import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/workflow.actions';
import { SearchWorkflowFileResult } from '../models/searchworkflowfile.model';

export interface SearchWorkflowFilesState {
  WorkflowFiles: SearchWorkflowFileResult | null;
}

export const initialSearchWorkflowFilesState: SearchWorkflowFilesState = {
  WorkflowFiles: null
};

const searchWorkflowFilesReducer = createReducer(
  initialSearchWorkflowFilesState,
  on(fromActions.completeSearchFiles, (state, { content }) => {
    return {
      ...state,
      WorkflowFiles: { ...content }
    };
  })
);

export function getSearchWorkflowFilesReducer(state: SearchWorkflowFilesState | undefined, action: Action) {
  return searchWorkflowFilesReducer(state, action);
}
