import { Injectable } from '@angular/core';
import { OAuthClientService } from '../services/client.service';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { ActionTypes, StartSearch, StartGet } from '../actions/client.actions';

@Injectable()
export class OAuthClientEffects {
    constructor(
        private actions$: Actions,
        private oauthClientService: OAuthClientService,
    ) { }

    @Effect()
    getOAuthClients$ = this.actions$
        .pipe(
            ofType(ActionTypes.START_SEARCH),
            mergeMap((evt : StartSearch) => {
                return this.oauthClientService.search(evt.startIndex, evt.count, evt.order, evt.direction)
                    .pipe(
                        map(content => { return { type: ActionTypes.COMPLETE_SEARCH, content: content }; }),
                        catchError(() => of({ type: ActionTypes.ERROR_SEARCH }))
                    );
            }
            )
    );

    @Effect()
    getOAuthClient$ = this.actions$
    .pipe(
        ofType(ActionTypes.START_GET),
        mergeMap((evt : StartGet) => {
            return this.oauthClientService.get(evt.id)
                .pipe(
                    map(oauthClient => { return { type: ActionTypes.COMPLETE_GET, oauthClient: oauthClient }; }),
                    catchError(() => of({ type: ActionTypes.ERROR_GET }))
                );
        }
        )
);
}