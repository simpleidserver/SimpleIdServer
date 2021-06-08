import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeAdd, completeGet, completeSearch, completeUpdate, errorAdd, errorGet, errorSearch, errorUpdate, startAdd, startGet, startSearch, startUpdate } from '../actions/applications.actions';
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
}
