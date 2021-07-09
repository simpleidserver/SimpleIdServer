import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../applications/models/search.model';
import * as fromActions from '../actions/provisioning.actions';
import { ProvisioningConfigurationHistory } from '../models/provisioningconfigurationhistory.model';

export interface ProvisioningHistoriesState {
  ProvisioningHistories: SearchResult<ProvisioningConfigurationHistory>;
}

export const initialProvisioningHistoriesState: ProvisioningHistoriesState = {
  ProvisioningHistories: new SearchResult<ProvisioningConfigurationHistory>()
};

const provisioningHistoriesReducer= createReducer(
  initialProvisioningHistoriesState,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      ProvisioningHistories: { ...content }
    };
  })
);

export function getProvisioningHistoriesReducer(state: ProvisioningHistoriesState | undefined, action: Action) {
  return provisioningHistoriesReducer(state, action);
}
