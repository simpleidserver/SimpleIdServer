import * as fromActions from '../actions/client.actions';
import { OAuthClient } from '../models/oauthclient.model';
import { SearchResult } from '../models/search.model';

export interface State {
    Clients: SearchResult<OAuthClient>;
    Client: OAuthClient;
}

export const initialState: State = {
    Clients: new SearchResult<OAuthClient>(),
    Client: null
};

export function oauthClientReducer(state = initialState, action: fromActions.ActionsUnion) {
    switch (action.type) {
        case fromActions.ActionTypes.COMPLETE_SEARCH:
            return {
                ...state,
                Clients: action.content
            };
        case fromActions.ActionTypes.COMPLETE_GET:
            return {
                ...state,
                Client: action.oauthClient
            };            
        default:
            return state;
    }
}