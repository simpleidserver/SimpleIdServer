import { createSelector } from '@ngrx/store';
import * as fromSearch from './search.reducer';

export interface OAuthClientState {
    search: fromSearch.State;
}

export const selectSearch = (state: OAuthClientState) => state.search;

export const selectSearchResults = createSelector(
    selectSearch,
    (state: fromSearch.State) => {
        if (!state || state.Content == null) {
            return null;
        }

        return state.Content;
    }
);

export const appReducer = {
    search: fromSearch.oauthClientReducer
};