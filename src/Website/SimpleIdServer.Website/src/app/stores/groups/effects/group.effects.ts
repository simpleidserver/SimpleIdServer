import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import {
    completeSearch,
    errorSearch,
    startSearch,
    startGet,
    completeGet,
    errorGet,
    startUpdate,
    completeUpdate,
    errorUpdate,
    startDelete,
    completeDelete,
    errorDelete
} from '../actions/groups.actions';
import { GroupService } from '../services/group.service';

@Injectable()
export class GroupEffects {
  constructor(
    private actions$: Actions,
    private groupService: GroupService,
  ) { }

  @Effect()
  searchGroups$ = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt) => {
        return this.groupService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
          );
      }
      )
  );

  @Effect()
  getGroup$ = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt) => {
        return this.groupService.get(evt.groupId)
          .pipe(
            map(content => completeGet({ content: content })),
            catchError(() => of(errorGet()))
          );
      }
      )
  );

  @Effect()
  updateGroup$ = this.actions$
      .pipe(
        ofType(startUpdate),
      mergeMap((evt) => {
        return this.groupService.update(evt.groupId, evt.request)
          .pipe(
            map(content => completeUpdate()),
            catchError(() => of(errorUpdate()))
          );
      }
      )
  );

  @Effect()
  deleteGroup$ = this.actions$
    .pipe(
      ofType(startDelete),
      mergeMap((evt) => {
        return this.groupService.delete(evt.groupId)
          .pipe(
            map(content => completeDelete()),
            catchError(() => of(errorDelete()))
          );
      }
      )
    );
}
