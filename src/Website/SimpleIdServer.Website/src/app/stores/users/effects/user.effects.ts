import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import {
  completeDelete, completeGet,









  completeGetOpenId, completeProvision, completeSearch,






  completeUpdate,



  errorDelete, errorGet,







  errorGetOpenId, errorProvision, errorSearch,






  errorUpdate,
  startDelete, startGet,









  startGetOpenId, startProvision, startSearch,


  startUpdate
} from '../actions/users.actions';
import { UserService } from '../services/user.service';

@Injectable()
export class UserEffects {
  constructor(
    private actions$: Actions,
    private userService: UserService,
  ) { }

  @Effect()
  searchUsers$ = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt) => {
        return this.userService.search(evt.startIndex, evt.count, evt.order, evt.direction, evt.filter)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
          );
      }
      )
    );

  @Effect()
  getUser$ = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt) => {
        return this.userService.get(evt.userId)
          .pipe(
            map(content => completeGet({ content: content })),
            catchError(() => of(errorGet()))
          );
      }
      )
    );

  @Effect()
  updateUser$ = this.actions$
    .pipe(
      ofType(startUpdate),
      mergeMap((evt) => {
        return this.userService.update(evt.userId, evt.request)
          .pipe(
            map(content => completeUpdate()),
            catchError(() => of(errorUpdate()))
          );
      }
      )
    );

  @Effect()
  deleteUser$ = this.actions$
    .pipe(
      ofType(startDelete),
      mergeMap((evt) => {
        return this.userService.delete(evt.userId)
          .pipe(
            map(content => completeDelete()),
            catchError(() => of(errorDelete()))
          );
      }
      )
    );

  @Effect()
  getOpenIdUser$ = this.actions$
    .pipe(
      ofType(startGetOpenId),
      mergeMap((evt) => {
        return this.userService.getOpenId(evt.scimId)
          .pipe(
            map(content => completeGetOpenId({ user: content })),
            catchError(() => of(errorGetOpenId()))
          );
      }
      )
  );

  @Effect()
  provision$ = this.actions$
    .pipe(
      ofType(startProvision),
      mergeMap((evt) => {
        return this.userService.provision(evt.scimId)
          .pipe(
            map(() => completeProvision()),
            catchError(() => of(errorProvision()))
          );
      }
      )
    );
}
