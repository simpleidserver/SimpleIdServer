import { createAction, props } from "@ngrx/store";
import { HumanTaskDef } from "../models/humantaskdef.model";
import { SearchHumanTaskDefsResult } from "../models/searchhumantaskdef.model";

export const startGet = createAction('[HumanTaskDef] START_GET_HUMANTASKDEF', props<{ id: string }>());
export const completeGet = createAction('[HumanTaskDef] COMPLETE_GET_HUMANTASKDEF', props<{ content: HumanTaskDef }>());
export const errorGet = createAction('[HumanTaskDef] ERROR_GET_HUMANTASKDEF');
export const startSearch = createAction('[HumanTaskDef] START_SEARCH_HUMANTASKDEF', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[HumanTaskDef] COMPLETE_SEARCH_HUMANTASKDEF', props<{ content: SearchHumanTaskDefsResult }>());
export const errorSearch = createAction('[HumanTaskDef] ERROR_SEARCH_HUMANTASKDEF');
export const startGetAll = createAction('[HumanTaskDef] START_GET_ALL_HUMANTASKDEF');
export const completeGetAll = createAction('[HumanTaskDef] COMPLETE_GET_ALL_HUMANTASKDEF', props<{ content: HumanTaskDef[] }>());
export const errorGetAll = createAction('[HumanTaskDef] ERROR_GET_ALL_HUMANTASKDEF');
