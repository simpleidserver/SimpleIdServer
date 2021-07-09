import { createAction, props } from '@ngrx/store';
import { SearchResult } from '../../applications/models/search.model';
import { ProvisioningConfigurationHistory } from '../models/provisioningconfigurationhistory.model';

export const startSearch = createAction('[ProvisioningConfigurationHistories] START_SEARCH', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[ProvisioningConfigurationHistories] COMPLETE_SEARCH', props<{ content: SearchResult<ProvisioningConfigurationHistory> }>());
export const errorSearch = createAction('[ProvisioningConfigurationHistories] ERROR_SEARCH');
