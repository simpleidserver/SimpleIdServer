import { Action, createReducer, on } from '@ngrx/store';
import * as fromActions from '../actions/workflow.actions';
import { SearchWorkflowFileResult } from '../models/searchworkflowfile.model';
import { SearchWorkflowInstanceResult } from '../models/searchworkflowinstance.model';
import { WorkflowFile } from '../models/workflowfile.model';
import { WorkflowInstance } from '../models/workflowinstance.model';

export interface SearchWorkflowFilesState {
  WorkflowFiles: SearchWorkflowFileResult | null;
}

export interface SearchWorkflowInstancesState {
  WorkflowInstances: SearchWorkflowInstanceResult | null;
}

export interface WorkflowInstanceState {
  WorkflowInstance: WorkflowInstance | null;
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

export const initialSearchWorkflowInstancesState: SearchWorkflowInstancesState = {
  WorkflowInstances: null
};

export const initialWorkflowInstanceState: WorkflowInstanceState = {
  WorkflowInstance: null
};

const searchWorkflowFilesReducer = createReducer(
  initialSearchWorkflowFilesState,
  on(fromActions.completeSearchFiles, (state, { content }) => {
    return {
      ...state,
      WorkflowFiles: { ...content }
    };
  }),
  on(fromActions.completePublishFile, (state, { id }) => {
    const workflowFiles = state.WorkflowFiles as SearchWorkflowFileResult;
    const workflowFile = new WorkflowFile();
    const files = state.WorkflowFiles?.content;
    let version = 0;
    files?.forEach((wf: WorkflowFile) => {
      if (version < wf.version) {
        version = wf.version;
      }
    });

    workflowFile.version = version;
    workflowFile.id = id;
    return {
      ...state,
      WorkflowFiles: {
        ...workflowFiles,
        content: [
          ...workflowFiles.content,
          workflowFile
        ]
      }
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
  }),
  on(fromActions.completePublishFile, (state, { id }) => {
    const workflowFile = state.WorkflowFile as WorkflowFile;
    return {
      ...state,
      WorkflowFile: { ...workflowFile, id: id, version: workflowFile.version + 1 }
    };
  })
);

const searchWorkflowInstancesReducer = createReducer(
  initialSearchWorkflowInstancesState,
  on(fromActions.completeSearchInstances, (state, { content }) => {
    return {
      ...state,
      WorkflowInstances: { ...content }
    };
  }),
  on(fromActions.completeCreateInstance, (state, { content }) => {
    const workflowInstances = state.WorkflowInstances as SearchWorkflowInstanceResult;
    return {
      ...state,
      WorkflowInstances: {
        ...workflowInstances,
        content: [
          ...workflowInstances.content,
          content.content[0]
        ]
      }
    };
  })
);

const workflowInstanceReducer = createReducer(
  initialWorkflowInstanceState,
  on(fromActions.completeGetInstance, (state, { content }) => {
    return {
      ...state,
      WorkflowInstance: { ...content }
    };
  })
);

export function getSearchWorkflowFilesReducer(state: SearchWorkflowFilesState | undefined, action: Action) {
  return searchWorkflowFilesReducer(state, action);
}


export function getWorkflowFileReducer(state: WorkflowFileState | undefined, action: Action) {
  return workflowFileReducer(state, action);
}

export function getWorkflowInstancesReducer(state: SearchWorkflowInstancesState | undefined, action: Action) {
  return searchWorkflowInstancesReducer(state, action);
}

export function getWorkflowInstanceReducer(state: WorkflowInstanceState | undefined, action: Action) {
  return workflowInstanceReducer(state, action);
}
