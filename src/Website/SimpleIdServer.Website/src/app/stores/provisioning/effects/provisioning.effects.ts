import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import {
    completeGet, completeSearch, completeSearchHistory,



    completeUpdate, errorGet, errorSearch, errorSearchHistory,


    errorUpdate, startGet,

    startSearch, startSearchHistory,
    startUpdate
} from '../actions/provisioning.actions';
import { ProvisioningConfigurationService } from '../services/provisioningconfiguration.service';

@Injectable()
export class ProvisioningEffects {
  constructor(
    private actions$: Actions,
    private provisioningConfigurationService: ProvisioningConfigurationService,
  ) { }

  @Effect()
  searchProvisioningConfigurationHistory$ = this.actions$
    .pipe(
      ofType(startSearchHistory),
      mergeMap((evt) => {
        return this.provisioningConfigurationService.searchHistory(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearchHistory({ content: content })),
            catchError(() => of(errorSearchHistory()))
          );
      }
      )
    );

  @Effect()
  searchProvisioningConfiguration$ = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt) => {
        return this.provisioningConfigurationService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
          );
      }
      )
    );

  @Effect()
  getProvisioningConfiguration$ = this.actions$
    .pipe(
      ofType(startGet),
      mergeMap((evt) => {
        return this.provisioningConfigurationService.get(evt.id)
          .pipe(
            map(content => completeGet({ content: content })),
            catchError(() => of(errorGet()))
          );
      }
      )
  );

  @Effect()
  updateProvisioningConfiguration$ = this.actions$
    .pipe(
      ofType(startUpdate),
      mergeMap((evt) => {
        return this.provisioningConfigurationService.update(evt.id, evt.records)
          .pipe(
            map(() => completeUpdate()),
            catchError(() => of(errorUpdate()))
          );
      }
      )
    );
}
