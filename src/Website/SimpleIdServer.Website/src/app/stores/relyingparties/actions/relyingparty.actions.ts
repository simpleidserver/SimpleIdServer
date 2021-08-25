import { createAction, props } from '@ngrx/store';
import { SearchResult } from '../../applications/models/search.model';
import { RelyingParty } from '../models/relyingparty.model';

export const startSearch = createAction('[Applications] START_SEARCH_RELYINGPARTIES', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[Applications] COMPLETE_SEARCH_RELYINGPARTIES', props<{ content: SearchResult<RelyingParty> }>());
export const errorSearch = createAction('[Applications] ERROR_SEARCH_RELYINGPARTIES');
export const startGet = createAction('[Applications] START_GET_RELYINGPARTY', props<{ id: string }>());
export const completeGet = createAction('[Applications] COMPLETE_GET_RELYINGPARTY', props<{ content: RelyingParty}>());
export const errorGet = createAction('[Applications] ERROR_GET_RELYINGPARTY');
export const startUpdate = createAction('[Applications] START_UPDATE_RELYINGPARTY', props<{ id: string, request: any }>());
export const completeUpdate = createAction('[Applications] COMPLETE_UPDATE_RELYINGPARTY');
export const errorUpdate = createAction('[Applications] ERROR_UPDATE_RELYINGPARTY');
export const startAdd = createAction('[Applications] START_ADD_RELYINGPARTY', props<{ metadataUrl: string }>());
export const completeAdd = createAction('[Applications] COMPLETE_ADD_RELYINGPARTY', props<{ id: string }>());
export const errorAdd = createAction('[Applications] ERROR_ADD_RELYINGPARTY');
export const startDelete = createAction('[Applications] START_DELETE_RELYINGPARTY', props<{ id: string }>());
export const completeDelete = createAction('[Applications] COMPLETE_DELETE_RELYINGPARTY');
export const errorDelete = createAction('[Applications] ERROR_DELETE_RELYINGPARTY');
