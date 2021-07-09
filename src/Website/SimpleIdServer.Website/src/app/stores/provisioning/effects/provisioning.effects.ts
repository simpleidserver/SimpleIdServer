import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import {
  startSearch,
  completeSearch,
  errorSearch
} from '../actions/provisioning.actions';
import { ProvisioningConfigurationHistoryService } from '../services/provisioningconfigurationhistory.service';

@Injectable()
export class ProvisioningEffects {
  constructor(
    private actions$: Actions,
    private provisioningConfigurationHistoryService: ProvisioningConfigurationHistoryService,
  ) { }

  @Effect()
  searchProvisioningConfigurationHistory$ = this.actions$
    .pipe(
      ofType(startSearch),
      mergeMap((evt) => {
        return this.provisioningConfigurationHistoryService.search(evt.startIndex, evt.count, evt.order, evt.direction)
          .pipe(
            map(content => completeSearch({ content: content })),
            catchError(() => of(errorSearch()))
            );
      }
      )
    );
}
