import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { completeGetAllScopes, errorGetAllScopes, startGetAllScopes } from '../actions/scope.actions';
import { OAuthScopeService } from '../services/scope.service';

@Injectable()
export class OAuthScopeEffects {
  constructor(
    private actions$: Actions,
    private oauthScopeService: OAuthScopeService,
  ) { }

  @Effect()
  getAllOAuthScopes = this.actions$
    .pipe(
      ofType(startGetAllScopes),
      mergeMap((evt) => {
        return this.oauthScopeService.getAll()
          .pipe(
            map(content => completeGetAllScopes({ content: content })),
            catchError(() => of(errorGetAllScopes()))
          );
      }
      )
    );
}
