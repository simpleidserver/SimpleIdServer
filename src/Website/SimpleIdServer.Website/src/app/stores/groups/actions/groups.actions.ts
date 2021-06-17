import { SearchResult } from '@app/stores/applications/models/search.model';
import { createAction, props } from '@ngrx/store';
import { Group } from '../models/group.model';

export const startSearch = createAction('[Groups] START_SEARCH_GROUPS', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[Groups] COMPLETE_SEARCH_GROUPS', props<{ content: SearchResult<Group> }>());
export const errorSearch = createAction('[Groups] ERROR_SEARCH_GROUPS');
export const startGet = createAction('[Groups] START_GET_GROUP', props<{ groupId: string }>());
export const completeGet = createAction('[Groups] COMPLETE_GET_GROUP', props<{ content: Group }>());
export const errorGet = createAction('[Groups] ERROR_GET_GROUP');
export const startUpdate = createAction('[Groups] START_UPDATE_GROUP', props<{ groupId: string, request: any }>());
export const completeUpdate = createAction('[Groups] COMPLETE_UPDATE_GROUP');
export const errorUpdate = createAction('[Groups] ERROR_UPDATE_GROUP');
export const startDelete = createAction('[Groups] START_DELETE_GROUP', props<{ groupId: string }>());
export const completeDelete = createAction('[Groups] COMPLETE_DELETE_GROUP');
export const errorDelete = createAction('[Groups] ERROR_DELETE_GROUP');
