import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import {
    completeSearch,
    errorSearch,
    startSearch
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
        return this.userService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
            );
      }
      )
    );
}
