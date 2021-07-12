import { createAction, props } from '@ngrx/store';
import { SearchResult } from '../../applications/models/search.model';
import { ProvisioningConfiguration } from '../models/provisioningconfiguration.model';
import { ProvisioningConfigurationHistory } from '../models/provisioningconfigurationhistory.model';
import { ProvisioningConfigurationRecord } from '../models/provisioningconfigurationrecord.model';

export const startSearchHistory = createAction('[ProvisioningConfigurationHistories] START_SEARCH_HISTORY', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearchHistory = createAction('[ProvisioningConfigurationHistories] COMPLETE_SEARCH_HISTORY', props<{ content: SearchResult<ProvisioningConfigurationHistory> }>());
export const errorSearchHistory = createAction('[ProvisioningConfigurationHistories] ERROR_SEARCH_HISTORY');
export const startSearch = createAction('[ProvisioningConfigurationHistories] START_SEARCH', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[ProvisioningConfigurationHistories] COMPLETE_SEARCH', props<{ content: SearchResult<ProvisioningConfiguration> }>());
export const errorSearch = createAction('[ProvisioningConfigurationHistories] ERROR_SEARCH');
export const startGet = createAction('[ProvisioningConfigurationHistories] START_GET', props<{ id: string }>());
export const completeGet = createAction('[ProvisioningConfigurationHistories] COMPLETE_GET', props<{ content: ProvisioningConfiguration }>());
export const errorGet = createAction('[ProvisioningConfigurationHistories] ERROR_GET');
export const startUpdate = createAction('[ProvisioningConfigurationHistories] START_UPDATE', props<{ id: string, records: ProvisioningConfigurationRecord[] }>());
export const completeUpdate = createAction('[ProvisioningConfigurationHistories] COMPLETE_UPDATE');
export const errorUpdate = createAction('[ProvisioningConfigurationHistories] ERROR_UPDATE');
