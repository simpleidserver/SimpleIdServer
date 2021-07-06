import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeSearchFiles, errorSearchFiles, startSearchFiles } from '../actions/workflow.actions';
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
}
