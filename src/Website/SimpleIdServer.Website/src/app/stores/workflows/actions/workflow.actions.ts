import { createAction, props } from '@ngrx/store';
import { SearchWorkflowFileResult } from '../models/searchworkflowfile.model';

export const startSearchFiles = createAction('[Workflows] START_SEARCH_FILES', props<{ startIndex: number, count: number, order: string, direction: string }>());
export const completeSearchFiles = createAction('[Workflows] COMPLETE_SEARCH_FILES', props<{ content: SearchWorkflowFileResult }>());
export const errorSearchFiles = createAction('[Workflows] ERROR_SEARCH_FILES');
