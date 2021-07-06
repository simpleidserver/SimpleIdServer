import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeGet, completeGetAll, completeSearch, completeUpdate, errorGet, errorGetAll, errorSearch, errorUpdate, startGet, startGetAll, startSearch, startUpdate } from '../actions/delegateconfigurations.actions';
import { DelegateConfigurationService } from '../services/delegateconfiguration.service';

@Injectable()
export class DelegateConfigurationEffects {
  constructor(
    private actions$: Actions,
    private delegateConfigurationService: DelegateConfigurationService,
  ) { }

  @Effect()
  getDelegateConfiguration$ = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt) => {
        return this.delegateConfigurationService.get(evt.id)
          .pipe(
            map(content => completeGet({ content: content })),
            catchError(() => of(errorGet()))
            );
      }
      )
    );

  @Effect()
  getAllDelegateConfigurations$ = this.actions$
    .pipe(
      ofType(startGetAll),
      mergeMap((evt) => {
        return this.delegateConfigurationService.getAll()
          .pipe(
            map(content => completeGetAll({ content: content })),
            catchError(() => of(errorGetAll()))
          );
      }
      )
  );

  @Effect()
  updateDelegateConfiguration$ = this.actions$
    .pipe(
      ofType(startUpdate),
      mergeMap((evt) => {
        return this.delegateConfigurationService.update(evt.id, evt.records)
          .pipe(
            map(() => completeUpdate()),
            catchError(() => of(errorUpdate()))
          );
      }
      )
  );

  @Effect()
  searchDelegateConfigurations$ = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt) => {
        return this.delegateConfigurationService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
          );
      }
      )
    );
}
