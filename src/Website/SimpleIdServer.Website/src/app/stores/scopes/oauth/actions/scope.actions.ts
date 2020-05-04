import { Action } from '@ngrx/store';
import { OAuthScope } from '../models/oauthscope.model';

export enum ActionTypes {
    START_GET_ALL = "[OAuthScopes] START_GET_ALL",
    COMPLETE_GET_ALL = "[OAuthScopes] COMPLETE_GET_ALL",
    ERROR_GET_ALL = "[OAuthScopes] ERROR_GET_ALL",
}

export class StartGetAll implements Action {
    readonly type = ActionTypes.START_GET_ALL;
    constructor() { }
}

export class CompleteGetAll implements Action {
    readonly type = ActionTypes.COMPLETE_GET_ALL;
    constructor(public oauthScopes : Array<OAuthScope>) { }
}

export type ActionsUnion = StartGetAll | CompleteGetAll;