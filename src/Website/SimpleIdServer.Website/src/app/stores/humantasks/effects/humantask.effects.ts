import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeGet, completeSearch, errorGet, errorSearch, startGet, startSearch, startGetAll, completeGetAll, errorGetAll } from '../actions/humantasks.actions';
import { HumanTaskDefService } from '../services/humantaskdef.service';

@Injectable()
export class HumanTaskEffects {
  constructor(
    private actions$: Actions,
    private humanTaskDefService: HumanTaskDefService,
  ) { }


  @Effect()
  get = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt) => {
        return this.humanTaskDefService.get(evt.id)
          .pipe(
            map(humanTaskDef => completeGet({ content: humanTaskDef })),
            catchError(() => of(errorGet()))
          );
      }
      )
  );

  @Effect()
  search = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt) => {
        return this.humanTaskDefService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map((content) => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
          );
      }
      )
  );

  @Effect()
  getAll = this.actions$
    .pipe(
      ofType(startGetAll),
      mergeMap(() => {
        return this.humanTaskDefService.getAll()
          .pipe(
            map((content) => completeGetAll({ content: content })),
            catchError(() => of(errorGetAll()))
          );
      }
      )
    );
}
