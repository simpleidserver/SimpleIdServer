import { createAction, props } from '@ngrx/store';
import { SearchResult } from '@app/stores/applications/models/search.model';
import { User } from '../models/user.model';

export const startSearch = createAction('[Users] START_SEARCH_USERS', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[Users] COMPLETE_SEARCH_USERS', props<{ content: SearchResult<User> }>());
export const errorSearch = createAction('[Users] ERROR_SEARCH_USERS');
export const startGet = createAction('[Users] START_GET_USER', props<{ userId: string }>());
export const completeGet = createAction('[Users] COMPLETE_GET_USER', props<{ content: User }>());
export const errorGet = createAction('[Users] ERROR_GET_USER');
export const startUpdate = createAction('[Users] START_UPDATE_USER', props<{ userId: string, request: any }>());
export const completeUpdate = createAction('[Users] COMPLETE_UPDATE_USER');
export const errorUpdate = createAction('[Users] ERROR_UPDATE_USER');
