import { createAction, props } from '@ngrx/store';
import { OAuthScope } from '../models/oauthscope.model';

export const startGetAllScopes = createAction('[OAuthScopes] START_GET_ALL_OAUTH_SCOPES');
export const completeGetAllScopes = createAction('[OAuthScopes] COMPLETE_GET_ALL_OAUTH_SCOPES', props<{ content: Array<OAuthScope> }>());
export const errorGetAllScopes = createAction('[OAuthScopes] ERROR_GET_ALL_SCOPES');
