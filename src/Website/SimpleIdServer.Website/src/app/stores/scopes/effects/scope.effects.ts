import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAdd, completeDelete, completeGet, completeGetAll, completeSearch, completeUpdate, errorAdd, errorDelete, errorGet, errorGetAll, errorSearch, errorUpdate, startAdd, startDelete, startGet, startGetAll, startSearch, startUpdate } from '../actions/scope.actions';
import { OAuthScopeService } from '../services/scope.service';

@Injectable()
export class OAuthScopeEffects {
  constructor(
    private actions$: Actions,
    private oauthScopeService: OAuthScopeService,
  ) { }

  @Effect()
  searchOAuthScopes$ = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt) => {
        return this.oauthScopeService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
          );
      }
      )
  );

  @Effect()
  getAllOAuthScopes$ = this.actions$
    .pipe(
      ofType(startGetAll),
      mergeMap((evt) => {
        return this.oauthScopeService.getAll()
          .pipe(
            map(content => completeGetAll({ content: content })),
            catchError(() => of(errorGetAll()))
          );
      }
      )
  );

  @Effect()
  getScope$ = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt) => {
        return this.oauthScopeService.get(evt.name)
          .pipe(
            map(content => completeGet({ content: content })),
            catchError(() => of(errorGet()))
          );
      }
      )
  );

  @Effect()
  updateScope$ = this.actions$
    .pipe(
      ofType(startUpdate),
      mergeMap((evt) => {
        return this.oauthScopeService.update(evt.name, evt.claims)
          .pipe(
            map(content => completeUpdate()),
            catchError(() => of(errorUpdate()))
          );
      }
      )
  );

  @Effect()
  addScope$ = this.actions$
    .pipe(
      ofType(startAdd),
      mergeMap((evt) => {
        return this.oauthScopeService.add(evt.name)
          .pipe(
            map((name) => completeAdd({ name : name })),
            catchError(() => of(errorAdd()))
          );
      }
      )
  );

  @Effect()
  deleteScope$ = this.actions$
    .pipe(
      ofType(startDelete),
      mergeMap((evt) => {
        return this.oauthScopeService.delete(evt.name)
          .pipe(
            map((name) => completeDelete()),
            catchError(() => of(errorDelete()))
          );
      }
      )
    );
}
