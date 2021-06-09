import { createAction, props } from '@ngrx/store';
import { Application } from '../models/application.model';
import { SearchResult } from '../models/search.model';

export const startSearch = createAction('[Applications] START_SEARCH_APPLICATIONS', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[Applications] COMPLETE_SEARCH_APPLICATIONS', props<{ content: SearchResult<Application> }>());
export const errorSearch = createAction('[Applications] ERROR_SEARCH_APPLICATIONS');
export const startGet = createAction('[Applications] START_GET_APPLICATION', props<{ id: string }>());
export const completeGet = createAction('[Applications] COMPLETE_GET_APPLICATIONS', props<{ content: Application}>());
export const errorGet = createAction('[Applications] ERROR_GET_APPLICATION');
export const startUpdate = createAction('[Applications] START_UPDATE_APPLICATON', props<{ id: string, request: any }>());
export const completeUpdate = createAction('[Applications] COMPLETE_UPDATE_APPLICATON');
export const errorUpdate = createAction('[Applications] ERROR_UPDATE_APPLICATION');
export const startAdd = createAction('[Applications] START_ADD_APPLICATION', props<{ applicationKind: number, name: string }>());
export const completeAdd = createAction('[Applications] COMPLETE_ADD_APPLICATON', props<{ clientId: string }>());
export const errorAdd = createAction('[Applications] ERROR_ADD_APPLICATION');
export const startDelete = createAction('[Applications] START_DELETE_APPLICATION', props<{ id: string }>());
export const completeDelete = createAction('[Applications] COMPLETE_DELETE_APPLICATION');
export const errorDelete = createAction('[Applications] ERROR_DELETE_APPLICATION');
