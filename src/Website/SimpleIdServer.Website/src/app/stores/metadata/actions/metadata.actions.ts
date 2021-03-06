import { createAction, props } from '@ngrx/store';
import { Language } from '../models/language.model';

export const startGetLanguages = createAction('[Metadata] START_GET_LANGUAGES');
export const completeGetLanguages = createAction('[Metadata] COMPLETE_GET_LANGUAGES', props<{ content: Array<Language> }>());
export const errorGetLanguages = createAction('[Metadata] ERROR_GET_LANGUAGES');
export const startGetWellKnownConfiguration = createAction('[Metadata] START_GET_WELLKNOWN_CONFIGURATION');
export const completeGetWellKnownConfiguration = createAction('[Metadata] COMPLETE_GET_WELLKNOWN_CONFIGURATION', props < { content: any }>());
export const errorGetWellKnownConfiguration = createAction('[Metadata] ERROR_GET_WELLKNOWN_CONFIGURATION');
