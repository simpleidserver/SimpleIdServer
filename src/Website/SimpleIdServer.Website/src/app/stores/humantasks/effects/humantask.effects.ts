import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { forkJoin, of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { startSearchInstances, completeSearchInstances, errorSearchInstances, completeGet, completeGetAll, completeSearch, completeGetInstance, errorGetInstance, errorGet, errorGetAll, errorSearch, startGet, startGetAll, startGetInstance, startSearch, startSubmitInstance, completeSubmitInstance, errorSubmitInstance } from '../actions/humantasks.actions';
import { HumanTaskDefService } from '../services/humantaskdef.service';
import { HumanTaskInstanceService } from '../services/humantaskinstance.service';

@Injectable()
export class HumanTaskEffects {
  constructor(
    private actions$: Actions,
    private humanTaskDefService: HumanTaskDefService,
    private humanTaskInstanceService: HumanTaskInstanceService
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

  @Effect()
  getHumanTaskInstance = this.actions$
    .pipe(
      ofType(startGetInstance),
      mergeMap((evt) => {
        const renderingCall = this.humanTaskInstanceService.getRendering(evt.id);
        const detailsCall = this.humanTaskInstanceService.getDetails(evt.id);
        return forkJoin([renderingCall, detailsCall ]).pipe(
          map((results) => completeGetInstance({ rendering: results[0], task: results[1] })),
          catchError(() => of(errorGetInstance()))
        );
      }
      )
  );

  @Effect()
  complete = this.actions$
    .pipe(
      ofType(startSubmitInstance),
      mergeMap((evt) => {
        return this.humanTaskInstanceService.completeTask(evt.id, evt.operationParameters)
          .pipe(
            map(() => completeSubmitInstance()),
            catchError(() => of(errorSubmitInstance()))
          );
      }
      )
  );

  @Effect()
  searchHumanTaskInstances = this.actions$
    .pipe(
      ofType(startSearchInstances),
      mergeMap((evt) => {
        return this.humanTaskInstanceService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map((content) => completeSearchInstances({ content: content })),
            catchError(() => of(errorSearchInstances()))
          );
      }
      )
    );
}
