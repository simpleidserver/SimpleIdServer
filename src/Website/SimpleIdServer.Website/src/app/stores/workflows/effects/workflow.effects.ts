import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeCreateInstance, completeGetFile, completePublishFile, completeSearchFiles, completeSearchInstances, completeStartInstance, completeUpdateFile, completeUpdateFilePayload, errorCreateInstance, errorGetFile, errorPublishFile, errorSearchFiles, errorSearchInstances, errorStartInstance, errorUpdateFile, errorUpdateFilePayload, startCreateInstance, startGetFile, startInstance, startPublishFile, startSearchFiles, startSearchInstances, startUpdateFile, startUpdateFilePayload } from '../actions/workflow.actions';
import { WorkflowFileService } from '../services/workflowfile.service';
import { WorkflowInstanceService } from '../services/workflowinstance.service';

@Injectable()
export class WorkflowEffects {
  constructor(
    private actions$: Actions,
    private workflowFileService: WorkflowFileService,
    private workflowInstanceService: WorkflowInstanceService
  ) { }

  @Effect()
  searchWorkflowFiles$ = this.actions$
    .pipe(
      ofType(startSearchFiles),
      mergeMap((evt) => {
        return this.workflowFileService.search(evt.startIndex, evt.count, evt.order, evt.direction, evt.takeLatest, evt.fileId)
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

  @Effect()
  updateWorkflowFilePayload$ = this.actions$
    .pipe(
      ofType(startUpdateFilePayload),
      mergeMap((evt) => {
        return this.workflowFileService.updatePayload(evt.id, evt.payload)
          .pipe(
            map(() => completeUpdateFilePayload()),
            catchError(() => of(errorUpdateFilePayload()))
          );
      }
      )
    );

  @Effect()
  publishFile$ = this.actions$
    .pipe(
      ofType(startPublishFile),
      mergeMap((evt) => {
        return this.workflowFileService.publish(evt.id)
          .pipe(
            map((id) => completePublishFile({ id: id })),
            catchError(() => of(errorPublishFile()))
          );
      }
      )
  );

  @Effect()
  searchInstances$ = this.actions$
    .pipe(
      ofType(startSearchInstances),
      mergeMap((evt) => {
        return this.workflowInstanceService.search(evt.startIndex, evt.count, evt.order, evt.direction, evt.processFileId)
          .pipe(
            map((content) => completeSearchInstances({ content: content })),
            catchError(() => of(errorSearchInstances()))
          );
      }
      )
  );

  @Effect()
  createInstance$ = this.actions$
    .pipe(
      ofType(startCreateInstance),
      mergeMap((evt) => {
        return this.workflowInstanceService.create(evt.id)
          .pipe(
            map((content) => completeCreateInstance({ content: content })),
            catchError(() => of(errorCreateInstance()))
          );
      }
      )
  );

  @Effect()
  startInstance$ = this.actions$
    .pipe(
      ofType(startInstance),
      mergeMap((evt) => {
        return this.workflowInstanceService.start(evt.id)
          .pipe(
            map(() => completeStartInstance()),
            catchError(() => of(errorStartInstance()))
          );
      }
      )
    );
}
