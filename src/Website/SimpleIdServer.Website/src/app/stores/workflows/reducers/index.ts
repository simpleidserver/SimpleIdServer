import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/workflow.actions';
import { SearchWorkflowFileResult } from '../models/searchworkflowfile.model';
import { WorkflowFile } from '../models/workflowfile.model';

export interface SearchWorkflowFilesState {
  WorkflowFiles: SearchWorkflowFileResult | null;
}

export interface WorkflowFileState {
  WorkflowFile: WorkflowFile | null;
}

export const initialSearchWorkflowFilesState: SearchWorkflowFilesState = {
  WorkflowFiles: null
};

export const initialWorkflowFileState: WorkflowFileState = {
  WorkflowFile: null
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

const workflowFileReducer = createReducer(
  initialWorkflowFileState,
  on(fromActions.completeGetFile, (state, { content }) => {
    return {
      ...state,
      WorkflowFile: { ...content }
    };
  }),
  on(fromActions.completeUpdateFile, (state, { name, description }) => {
    const workflowFile = state.WorkflowFile as WorkflowFile;
    return {
      ...state,
      WorkflowFile: { ...workflowFile, name: name, description: description}
    };
  })
);

export function getSearchWorkflowFilesReducer(state: SearchWorkflowFilesState | undefined, action: Action) {
  return searchWorkflowFilesReducer(state, action);
}


export function getWorkflowFileReducer(state: WorkflowFileState | undefined, action: Action) {
  return workflowFileReducer(state, action);
}
