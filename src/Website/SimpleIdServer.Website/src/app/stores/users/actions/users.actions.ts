import { createAction, props } from '@ngrx/store';
import { SearchResult } from '@app/stores/applications/models/search.model';
import { User } from '../models/user.model';

export const startSearch = createAction('[Users] START_SEARCH_USERS', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[Users] COMPLETE_SEARCH_USERS', props<{ content: SearchResult<User> }>());
export const errorSearch = createAction('[Users] ERROR_SEARCH_USERS');
