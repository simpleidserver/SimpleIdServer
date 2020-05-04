import { Action } from '@ngrx/store';
import { SearchResult } from '../models/search.model';
import { OAuthClient } from '../models/oauthclient.model';

export enum ActionTypes {
    START_SEARCH = "[OAuthClients] START_SEARCH",
    COMPLETE_SEARCH = "[OAuthClients] COMPLETE_SEARCH",
    START_GET = "[OAuthClients] START_GET",
    COMPLETE_GET = "[OAuthClients] COMPLETE_GET",
    ERROR_SEARCH = "[OAuthClients] ERROR_SEARCH",
    ERROR_GET = "[OAuthClients] ERROR_GET",
}

export class StartSearch implements Action {
    readonly type = ActionTypes.START_SEARCH;
    constructor(public order: string, public direction: string, public count: number, public startIndex: number) { }
}

export class CompleteSearch implements Action {
    readonly type = ActionTypes.COMPLETE_SEARCH;
    constructor(public content : SearchResult<OAuthClient>) { }
}

export class StartGet implements Action {
    readonly type = ActionTypes.START_GET;
    constructor(public id : string) { }
}

export class CompleteGet implements Action {
    readonly type = ActionTypes.COMPLETE_GET;
    constructor(public oauthClient : OAuthClient) { }
}

export type ActionsUnion = StartSearch | CompleteSearch | StartGet | CompleteGet;