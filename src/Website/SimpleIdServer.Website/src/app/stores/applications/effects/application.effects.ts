import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import {
  completeAdd,
  completeDelete,
  completeGet,
  completeSearch,
  errorAdd,
  errorDelete,
  errorGet,
  errorSearch,
  startAdd,
  startDelete,
  startGet,
  startSearch,
  startUpdate,
  completeUpdate,
  errorUpdate
} from '../actions/applications.actions';
import { ApplicationService } from '../services/application.service';

@Injectable()
export class ApplicationEffects {
  constructor(
    private actions$: Actions,
    private applicationService: ApplicationService,
  ) { }

  @Effect()
  searchApplications$ = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt) => {
        return this.applicationService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
            );
      }
      )
    );

  @Effect()
  getOAuthClient$ = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt) => {
        return this.applicationService.get(evt.id)
          .pipe(
            map(content => completeGet({ content: content })),
            catchError(() => of(errorGet()))
          );
      }
      )
  );

  @Effect()
  addOAuthClient$ = this.actions$
    .pipe(
      ofType(startAdd),
      mergeMap((evt) => {
        return this.applicationService.add(evt.applicationKind, evt.name)
          .pipe(
            map(content => completeAdd({ clientId : content })),
            catchError(() => of(errorAdd()))
          );
      }
      )
  );

  @Effect()
  deleteClient$ = this.actions$
    .pipe(
      ofType(startDelete),
      mergeMap((evt) => {
        return this.applicationService.delete(evt.id)
          .pipe(
            map(() => completeDelete()),
            catchError(() => of(errorDelete()))
          );
      }
      )
  );

  @Effect()
  updateOAuthClient$ = this.actions$
    .pipe(
      ofType(startUpdate),
      mergeMap((evt) => {
        return this.applicationService.update(evt.id, evt.request)
          .pipe(
            map(content => completeUpdate()),
            catchError(() => of(errorUpdate()))
          );
      }
      )
    );
}
