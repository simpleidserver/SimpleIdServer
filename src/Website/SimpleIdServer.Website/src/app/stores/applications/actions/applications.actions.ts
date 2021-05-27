import { createAction, props } from '@ngrx/store';
import { Application } from '../models/application.model';
import { SearchResult } from '../models/search.model';

export const startSearch = createAction('[Applications] START_SEARCH_APPLICATIONS', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[Applications] COMPLETE_SEARCH_APPLICATIONS', props<{ content: SearchResult<Application> }>());
export const errorSearch = createAction('[Applications] ERROR_SEARCH_APPLICATIONS');
export const startGet = createAction('[Applications] START_GET_APPLICATION', props<{ id: string }>());
export const completeGet = createAction('[Applications] COMPLETE_GET_APPLICATIONS', props<{ content: Application}>());
export const errorGet = createAction('[Applications] ERROR_GET_APPLICATION');
