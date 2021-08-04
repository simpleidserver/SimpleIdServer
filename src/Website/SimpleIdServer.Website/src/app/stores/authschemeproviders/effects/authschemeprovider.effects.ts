import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import {
    completeDisable,
    completeEnable,
    completeGet,



    completeGetAll,


    completeUpdate, errorDisable, errorEnable, errorGet,


    errorGetAll,


    errorUpdate, startDisable, startEnable, startGet,

    startGetAll,


    startUpdate
} from '../actions/authschemeprovider.actions';
import { AuthSchemeProviderService } from '../services/authschemeprovider.service';

@Injectable()
export class AuthSchemeProviderEffects {
  constructor(
    private actions$: Actions,
    private authSchemeProviderService: AuthSchemeProviderService,
  ) { }

  @Effect()
  getAll$ = this.actions$
    .pipe(
      ofType(startGetAll),
      mergeMap((evt) => {
        return this.authSchemeProviderService.getAll()
          .pipe(
            map(content => completeGetAll({ content: content })),
            catchError(() => of(errorGetAll()))
            );
      }
      )
    );

  @Effect()
  get$ = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt) => {
        return this.authSchemeProviderService.get(evt.id)
          .pipe(
            map(content => completeGet({ content: content })),
            catchError(() => of(errorGet()))
          );
      }
      )
  );

  @Effect()
  updateOptions$ = this.actions$
    .pipe(
      ofType(startUpdate),
      mergeMap((evt) => {
        return this.authSchemeProviderService.updateOptions(evt.id, evt.options)
          .pipe(
            map(() => completeUpdate({ options: evt.options })),
            catchError(() => of(errorUpdate()))
          );
      }
      )
  );

  @Effect()
  enable$ = this.actions$
    .pipe(
      ofType(startEnable),
      mergeMap((evt) => {
        return this.authSchemeProviderService.enable(evt.id)
          .pipe(
            map(() => completeEnable({ id: evt.id })),
            catchError(() => of(errorEnable()))
          );
      }
      )
  );

  @Effect()
  disable$ = this.actions$
    .pipe(
      ofType(startDisable),
      mergeMap((evt) => {
        return this.authSchemeProviderService.disable(evt.id)
          .pipe(
            map(() => completeDisable({ id: evt.id })),
            catchError(() => of(errorDisable()))
          );
      }
      )
    );
}
