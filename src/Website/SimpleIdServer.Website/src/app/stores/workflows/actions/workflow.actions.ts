import { createAction, props } from '@ngrx/store';
import { SearchWorkflowFileResult } from '../models/searchworkflowfile.model';
import { WorkflowFile } from '../models/workflowfile.model';

export const startSearchFiles = createAction('[Workflows] START_SEARCH_FILES', props<{ startIndex: number, count: number, order: string, direction: string }>());
export const completeSearchFiles = createAction('[Workflows] COMPLETE_SEARCH_FILES', props<{ content: SearchWorkflowFileResult }>());
export const errorSearchFiles = createAction('[Workflows] ERROR_SEARCH_FILES');
export const startGetFile = createAction('[Workflows] START_GET_FILE', props<{ id: string }>());
export const completeGetFile = createAction('[Workflows] COMPLETE_GET_FILE', props<{ content: WorkflowFile }>());
export const errorGetFile = createAction('[Workflows] ERROR_GET_FILE');
export const startUpdateFile = createAction('[Workflows] START_UPDATE_FILE', props<{ id: string, name: string, description: string }>());
export const completeUpdateFile = createAction('[Workflows] COMPLETE_UPDATE_FILE', props<{name: string, description: string}>());
export const errorUpdateFile = createAction('[Workflows] ERROR_UPDATE_FILE');
