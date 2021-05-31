import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeGetLanguages, errorGetLanguages, startGetLanguages } from '../actions/metadata.actions';
import { MetadataService } from '../services/metadata.service';

@Injectable()
export class MetadataEffects {
  constructor(
    private actions$: Actions,
    private metadataService: MetadataService,
  ) { }

  @Effect()
  getLanguages$ = this.actions$
    .pipe(
      ofType(startGetLanguages),
      mergeMap((evt) => {
        return this.metadataService.getLanguages()
          .pipe(
            map(content => completeGetLanguages({ content: content })),
            catchError(() => of(errorGetLanguages()))
          );
      }
      )
    );
}
