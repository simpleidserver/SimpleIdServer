import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeGetFile, completeSearchFiles, completeUpdateFile, errorGetFile, errorSearchFiles, errorUpdateFile, startGetFile, startSearchFiles, startUpdateFile } from '../actions/workflow.actions';
import { WorkflowFileService } from '../services/workflowfile.service';

@Injectable()
export class WorkflowEffects {
  constructor(
    private actions$: Actions,
    private workflowFileService: WorkflowFileService,
  ) { }

  @Effect()
  searchWorkflowFiles$ = this.actions$
    .pipe(
      ofType(startSearchFiles),
      mergeMap((evt) => {
        return this.workflowFileService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearchFiles({ content: content })),
            catchError(() => of(errorSearchFiles()))
          );
      }
      )
  );

  @Effect()
  getWorkflowFile$ = this.actions$
    .pipe(
      ofType(startGetFile),
      mergeMap((evt) => {
        return this.workflowFileService.get(evt.id)
          .pipe(
            map(content => completeGetFile({ content: content })),
            catchError(() => of(errorGetFile()))
          );
      }
      )
  );

  @Effect()
  updateWorkflowFile$ = this.actions$
    .pipe(
      ofType(startUpdateFile),
      mergeMap((evt) => {
        return this.workflowFileService.update(evt.id, evt.name, evt.description)
          .pipe(
            map(() => completeUpdateFile({ name: evt.name, description: evt.description })),
            catchError(() => of(errorUpdateFile()))
          );
      }
      )
    );
}
