import { Component, OnInit } from '@angular/core';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import { MatSnackBar } from '@angular/material/snack-bar';
import * as fromReducers from '@app/stores/appstate';
import { startDisable, startEnable, startGetAll } from '@app/stores/authschemeproviders/actions/authschemeprovider.actions';
import { AuthSchemeProvider } from '@app/stores/authschemeproviders/models/authschemeprovider.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'list-authschemeproviders',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListAuthenticationsComponent implements OnInit {
  isLoading: boolean;
  authSchemeProviders$: Array<AuthSchemeProvider> = [];

  constructor(
    private store: Store<fromReducers.AppState>,
    private actions$: ScannedActionsSubject,
    private translateService: TranslateService,
    private snackbar: MatSnackBar) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.actions$.pipe(
      filter((action: any) => action.type === '[AuthSchemeProviders] COMPLETE_ENABLE_AUTHSCHEMEPROVIDER'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('authentications.messages.enable'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[AuthSchemeProviders] ERROR_ENABLE_AUTHSCHEMEPROVIDER'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('authentications.messages.errorEnable'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.isLoading = false;
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[AuthSchemeProviders] COMPLETE_DISABLE_AUTHSCHEMEPROVIDER'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('authentications.messages.disable'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[AuthSchemeProviders] ERROR_DISABLE_AUTHSCHEMEPROVIDER'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('authentications.messages.errorDisable'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.isLoading = false;
      });
    this.store.pipe(select(fromReducers.selectAuthSchemeProvidersResult)).subscribe((authSchemeProviders: AuthSchemeProvider[] | null) => {
      if (!authSchemeProviders) {
        return;
      }

      this.authSchemeProviders$ = authSchemeProviders;
      this.isLoading = false;
    });
    this.refresh();
  }

  toggle(evt: MatSlideToggleChange, authSchemeProvider: AuthSchemeProvider) {
    this.isLoading = true;
    if (evt.checked) {
      const enable = startEnable({ id: authSchemeProvider.id });
      this.store.dispatch(enable);
    } else {
      const disable = startDisable({ id: authSchemeProvider.id });
      this.store.dispatch(disable);
    }
  }

  refresh() {
    const getAll = startGetAll();
    this.store.dispatch(getAll);
  }
}
