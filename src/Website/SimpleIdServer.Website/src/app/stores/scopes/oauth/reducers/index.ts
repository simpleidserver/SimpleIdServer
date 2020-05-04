import * as fromActions from '../actions/scope.actions';
import { OAuthScope } from '../models/oauthscope.model';

export interface State {
    Scopes: Array<OAuthScope>;
}

export const initialState: State = {
    Scopes: null
};

export function oauthScopeReducer(state = initialState, action: fromActions.ActionsUnion) {
    switch (action.type) {
        case fromActions.ActionTypes.COMPLETE_GET_ALL:
            return {
                ...state,
                Content: action.oauthScopes
            };
        default:
            return state;
    }
}