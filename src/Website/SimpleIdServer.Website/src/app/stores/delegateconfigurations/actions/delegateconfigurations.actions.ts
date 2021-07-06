import { createAction, props } from '@ngrx/store';
import { DelegateConfiguration } from '../models/delegateconfiguration.model';
import { SearchDelegateConfigurationResult } from '../models/searchdelegateconfiguration.model';

export const startSearch = createAction('[DelegateConfigurations] START_SEARCH_DELEGATECONFIGURATIONS', props<{ order: string, direction: string, count: number, startIndex: number }>());
export const completeSearch = createAction('[DelegateConfigurations] COMPLETE_SEARCH_DELEGATECONFIGURATIONS', props<{ content: SearchDelegateConfigurationResult}>());
export const errorSearch = createAction('[DelegateConfigurations] ERROR_SEARCH_DELEGATECONFIGURATIONS');
export const startGet = createAction('[DelegateConfigurations] START_GET_DELEGATECONFIGURATION', props<{ id: string }>());
export const completeGet = createAction('[DelegateConfigurations] COMPLETE_GET_DELEGATECONFIGURATION', props<{ content: DelegateConfiguration}>());
export const errorGet = createAction('[DelegateConfigurations] ERROR_GET_DELEGATECONFIGURATION');
export const startUpdate = createAction('[DelegateConfigurations] START_UPDATE_DELEGATECONFIGURATION', props<{ id: string, records: any }>());
export const completeUpdate = createAction('[DelegateConfigurations] COMPLETE_UPDATE_DELEGATECONFIGURATION');
export const errorUpdate = createAction('[DelegateConfigurations] ERROR_UPDATE_DELEGATECONFIGURATION');
export const startGetAll = createAction('[DelegateConfigurations] START_GET_ALL_DELEGATECONFIGURATIONS');
export const completeGetAll = createAction('[DelegateConfigurations] COMPLETE_GET_ALL_DELEGATECONFIGURATIONS', props<{ content: string[] }>());
export const errorGetAll = createAction('[DelegateConfigurations] ERROR_GET_ALL_DELEGATECONFIGURATIONS');
