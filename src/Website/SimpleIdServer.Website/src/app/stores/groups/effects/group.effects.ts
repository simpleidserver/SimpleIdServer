import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import {
    completeSearch,
    errorSearch, startSearch
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
}
