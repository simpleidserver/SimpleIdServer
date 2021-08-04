import { createAction, props } from '@ngrx/store';
import { AuthSchemeProvider } from '../models/authschemeprovider.model';

export const startGetAll = createAction('[AuthSchemeProviders] START_GET_ALL_AUTHSCHEMEPROVIDERS');
export const completeGetAll = createAction('[AuthSchemeProviders] COMPLETE_GET_ALL_AUTHSCHEMEPROVIDERS', props<{ content: AuthSchemeProvider[] }>());
export const errorGetAll = createAction('[AuthSchemeProviders] ERROR_GET_ALL_AUTHSCHEMEPROVIDERS');
export const startGet = createAction('[AuthSchemeProviders] START_GET_AUTHSCHEMEPROVIDER', props<{ id: string }>());
export const completeGet = createAction('[AuthSchemeProviders] COMPLETE_GET_AUTHSCHEMEPROVIDER', props<{ content: AuthSchemeProvider}>());
export const errorGet = createAction('[AuthSchemeProviders] ERROR_GET_AUTHSCHEMEPROVIDER');
export const startUpdate = createAction('[AuthSchemeProviders] START_UPDATE_AUTHSCHEMEPROVIDER', props<{ id: string, options: any }>());
export const completeUpdate = createAction('[AuthSchemeProviders] COMPLETE_UPDATE_AUTHSCHEMEPROVIDER', props<{ options: any }>());
export const errorUpdate = createAction('[AuthSchemeProviders] ERROR_UPDATE_AUTHSCHEMEPROVIDER');
export const startEnable = createAction('[AuthSchemeProviders] START_ENABLE_AUTHSCHEMEPROVIDER', props<{ id: string }>());
export const completeEnable = createAction('[AuthSchemeProviders] COMPLETE_ENABLE_AUTHSCHEMEPROVIDER', props<{ id: string }>());
export const errorEnable = createAction('[AuthSchemeProviders] ERROR_ENABLE_AUTHSCHEMEPROVIDER');
export const startDisable = createAction('[AuthSchemeProviders] START_DISABLE_AUTHSCHEMEPROVIDER', props<{ id: string }>());
export const completeDisable = createAction('[AuthSchemeProviders] COMPLETE_DISABLE_AUTHSCHEMEPROVIDER', props<{ id: string }>());
export const errorDisable = createAction('[AuthSchemeProviders] ERROR_DISABLE_AUTHSCHEMEPROVIDER');

