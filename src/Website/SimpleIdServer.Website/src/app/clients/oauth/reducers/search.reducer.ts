import * as fromActions from '../actions/client.actions';
import { OAuthClient } from '../models/oauthclient.model';
import { SearchResult } from '../models/search.model';

export interface State {
    Content: SearchResult<OAuthClient>;
}

export const initialState: State = {
    Content: new SearchResult<OAuthClient>()
};

export function oauthClientReducer(state = initialState, action: fromActions.ActionsUnion) {
    switch (action.type) {
        case fromActions.ActionTypes.COMPLETE_SEARCH:
            return {
                ...state,
                Content: action.content
            };
        default:
            return state;
    }
}