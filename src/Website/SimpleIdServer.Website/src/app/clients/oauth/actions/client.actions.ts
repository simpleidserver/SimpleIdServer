import { Action } from '@ngrx/store';
import { SearchResult } from '../models/search.model';
import { OAuthClient } from '../models/oauthclient.model';

export enum ActionTypes {
    START_SEARCH = "[OAuthClients] START_SEARCH",
    COMPLETE_SEARCH = "[OAuthClients] COMPLETE_SEARCH",
    ERROR_SEARCH = "[OAuthClients] ERROR_SEARCH"
}

export class StartSearch implements Action {
    readonly type = ActionTypes.START_SEARCH;
    constructor(public order: string, public direction: string, public count: number, public startIndex: number) { }
}

export class CompleteSearch implements Action {
    readonly type = ActionTypes.COMPLETE_SEARCH;
    constructor(public content : SearchResult<OAuthClient>) { }
}

export type ActionsUnion = StartSearch | CompleteSearch;