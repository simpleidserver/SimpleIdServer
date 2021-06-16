import { SearchResult } from '@app/stores/applications/models/search.model';
import { createAction, props } from '@ngrx/store';
import { Group } from '../models/group.model';

export const startSearch = createAction('[Groups] START_SEARCH_GROUPS', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[Groups] COMPLETE_SEARCH_GROUPS', props<{ content: SearchResult<Group> }>());
export const errorSearch = createAction('[Groups] ERROR_SEARCH_GROUPS');
