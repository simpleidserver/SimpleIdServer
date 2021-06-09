import { createAction, props } from '@ngrx/store';
import { SearchResult } from '../../applications/models/search.model';
import { OAuthScope } from '../models/oauthscope.model';

export const startSearch = createAction('[OAuthScopes] START_SEARCH_SCOPES', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[OAuthScopes] COMPLETE_SEARCH_SCOPES', props<{ content: SearchResult<OAuthScope> }>());
export const errorSearch = createAction('[OAuthScopes] ERROR_SEARCH_SCOPES');
export const startGetAll = createAction('[OAuthScopes] START_GET_ALL_SCOPES');
export const completeGetAll = createAction('[OAuthScopes] COMPLETE_GET_ALL_SCOPES', props<{ content: OAuthScope[] }>());
export const errorGetAll = createAction('[OAuthScopes] ERROR_GET_ALL_SCOPES');
