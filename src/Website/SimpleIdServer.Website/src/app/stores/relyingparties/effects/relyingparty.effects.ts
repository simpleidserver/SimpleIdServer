import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import {
    completeAdd,
    completeDelete,
    completeGet,
    completeSearch,









    completeUpdate, errorAdd,
    errorDelete,
    errorGet,
    errorSearch,






    errorUpdate, startAdd,
    startDelete,
    startGet,
    startSearch,
    startUpdate
} from '../actions/relyingparty.actions';
import { RelyingPartyService } from '../services/relyingparty.service';

@Injectable()
export class RelyingPartyEffects {
  constructor(
    private actions$: Actions,
    private relyingPartyService: RelyingPartyService,
  ) { }

  @Effect()
  searchRelyingParties$ = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt) => {
        return this.relyingPartyService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
            );
      }
      )
    );

  @Effect()
  getRelyingParty$ = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt) => {
        return this.relyingPartyService.get(evt.id)
          .pipe(
            map(content => completeGet({ content: content })),
            catchError(() => of(errorGet()))
          );
      }
      )
  );

  @Effect()
  addRelyingParty$ = this.actions$
    .pipe(
      ofType(startAdd),
      mergeMap((evt) => {
        return this.relyingPartyService.add(evt.metadataUrl)
          .pipe(
            map(content => completeAdd({ id : content })),
            catchError(() => of(errorAdd()))
          );
      }
      )
  );

  @Effect()
  deleteRelyingParty$ = this.actions$
    .pipe(
      ofType(startDelete),
      mergeMap((evt) => {
        return this.relyingPartyService.delete(evt.id)
          .pipe(
            map(() => completeDelete()),
            catchError(() => of(errorDelete()))
          );
      }
      )
  );

  @Effect()
  updateRelyingParty$ = this.actions$
    .pipe(
      ofType(startUpdate),
      mergeMap((evt) => {
        return this.relyingPartyService.update(evt.id, evt.request)
          .pipe(
            map(content => completeUpdate()),
            catchError(() => of(errorUpdate()))
          );
      }
      )
    );
}
