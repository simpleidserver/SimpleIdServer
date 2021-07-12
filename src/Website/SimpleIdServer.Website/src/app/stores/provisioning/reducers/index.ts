import { Action, createReducer, on } from '@ngrx/store';
import { SearchResult } from '../../applications/models/search.model';
import * as fromActions from '../actions/provisioning.actions';
import { ProvisioningConfiguration } from '../models/provisioningconfiguration.model';
import { ProvisioningConfigurationHistory } from '../models/provisioningconfigurationhistory.model';

export interface ProvisioningHistoriesState {
  ProvisioningHistories: SearchResult<ProvisioningConfigurationHistory>;
}

export interface ProvisioningConfigurationsState {
  ProvisioningConfigurations: SearchResult<ProvisioningConfiguration>;
}

export interface ProvisioningConfigurationState {
  ProvisioningConfiguration: ProvisioningConfiguration | null;
}

export const initialProvisioningHistoriesState: ProvisioningHistoriesState = {
  ProvisioningHistories: new SearchResult<ProvisioningConfigurationHistory>()
};

export const initialProvisioningConfigurationsState: ProvisioningConfigurationsState = {
  ProvisioningConfigurations: new SearchResult<ProvisioningConfiguration>()
};

export const initialProvisioningConfigurationState: ProvisioningConfigurationState = {
  ProvisioningConfiguration: null
};


const provisioningHistoriesReducer= createReducer(
  initialProvisioningHistoriesState,
  on(fromActions.completeSearchHistory, (state, { content }) => {
    return {
      ...state,
      ProvisioningHistories: { ...content }
    };
  })
);

const provisioningConfigurationsReducer = createReducer(
  initialProvisioningConfigurationsState,
  on(fromActions.completeSearch, (state, { content }) => {
    return {
      ...state,
      ProvisioningConfigurations: { ...content }
    };
  })
);

const provisioningConfigurationReducer = createReducer(
  initialProvisioningConfigurationState,
  on(fromActions.completeGet, (state, { content }) => {
    return {
      ...state,
      ProvisioningConfiguration: { ...content }
    };
  })
);


export function getProvisioningHistoriesReducer(state: ProvisioningHistoriesState | undefined, action: Action) {
  return provisioningHistoriesReducer(state, action);
}

export function getProvisioningConfigurationsReducer(state: ProvisioningConfigurationsState | undefined, action: Action) {
  return provisioningConfigurationsReducer(state, action);
}

export function getProvisioningConfigurationReducer(state: ProvisioningConfigurationState | undefined, action: Action) {
  return provisioningConfigurationReducer(state, action);
}
