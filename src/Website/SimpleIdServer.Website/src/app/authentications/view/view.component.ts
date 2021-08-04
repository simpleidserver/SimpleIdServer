import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { startGet, startUpdate } from '@app/stores/authschemeproviders/actions/authschemeprovider.actions';
import { AuthSchemeProvider } from '@app/stores/authschemeproviders/models/authschemeprovider.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'view-application',
  templateUrl: './view.component.html'
})
export class ViewAuthenticationComponent implements OnInit {
  isLoading: boolean;
  authSchemeProvider: AuthSchemeProvider;
  updateForm: FormGroup = new FormGroup({});
  options: { key: string, value: string }[] = [];

  constructor(
    private store: Store<fromReducers.AppState>,
    private route: ActivatedRoute,
    private translateService: TranslateService,
    private snackbar: MatSnackBar,
    private actions$: ScannedActionsSubject) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.actions$.pipe(
      filter((action: any) => action.type === '[AuthSchemeProviders] COMPLETE_UPDATE_AUTHSCHEMEPROVIDER'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('authentications.messages.update'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[AuthSchemeProviders] ERROR_UPDATE_AUTHSCHEMEPROVIDER'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('authentications.messages.errorUpdate'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectAuthSchemeProviderResult)).subscribe((authSchemeProvider: AuthSchemeProvider | null) => {
      if (!authSchemeProvider) {
        return;
      }

      this.options = [];
      this.updateForm = new FormGroup({});
      this.authSchemeProvider = authSchemeProvider;
      for (const key in authSchemeProvider.options) {
        const value = authSchemeProvider.options[key];
        this.updateForm.addControl(key, new FormControl(value));
        this.options.push({ key: key, value: value });
      }

      this.isLoading = false;
    });
    this.refresh();
  }

  refresh() {
    const id = this.route.snapshot.params['id'];
    const getAuthSchemeProvider = startGet({ id: id });
    this.store.dispatch(getAuthSchemeProvider);
  }

  update(evt : any) {
    evt.preventDefault();
    this.isLoading = true;
    const action = startUpdate({ id: this.authSchemeProvider.id, options: this.updateForm.value });
    this.store.dispatch(action);
  }
}
