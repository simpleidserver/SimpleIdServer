import { Injectable } from '@angular/core';
import { OAuthScopeService } from '../services/scope.service';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { ActionTypes, StartGetAll, CompleteGetAll } from '../actions/scope.actions';

@Injectable()
export class OAuthScopeEffects {
    constructor(
        private actions$: Actions,
        private oauthScopeService: OAuthScopeService,
    ) { }

    @Effect()
    getOAuthClients$ = this.actions$
        .pipe(
            ofType(ActionTypes.START_GET_ALL),
            mergeMap((evt : StartGetAll) => {
                return this.oauthScopeService.getAll()
                    .pipe(
                        map(oauthScopes => { return { type: ActionTypes.COMPLETE_GET_ALL, oauthScopes: oauthScopes }; }),
                        catchError(() => of({ type: ActionTypes.ERROR_GET_ALL }))
                    );
            }
            )
    );
}