import { createAction, props } from '@ngrx/store';
import { Language } from '../models/language.model';

export const startGetLanguages = createAction('[Metadata] START_GET_LANGUAGES');
export const completeGetLanguages = createAction('[Metadata] COMPLETE_GET_LANGUAGES', props<{ content: Array<Language> }>());
export const errorGetLanguages = createAction('[Metadata] ERROR_GET_LANGUAGES');
