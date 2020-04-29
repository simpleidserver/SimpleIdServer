import { Injectable } from '@angular/core';
import { OAuthClientService } from '../services/client.service';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { ActionTypes, StartSearch } from '../actions/client.actions';

@Injectable()
export class OAuthClientEffects {
    constructor(
        private actions$: Actions,
        private oauthClientService: OAuthClientService,
    ) { }

    @Effect()
    getOpenedPrescriptions$ = this.actions$
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
}