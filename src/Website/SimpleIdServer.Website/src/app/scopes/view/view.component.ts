import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { startDelete, startGet, startUpdate } from '@app/stores/scopes/actions/scope.actions';
import { OAuthScope } from '@app/stores/scopes/models/oauthscope.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'view-scope',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewScopeComponent implements OnInit {
  isLoading: boolean = true;
  claims: string[] = [];
  scope$: OAuthScope;
  updateScopeForm: FormGroup = new FormGroup({
    scopeName : new FormControl({ value: '', disabled: true })
  });

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService,
    private actions$: ScannedActionsSubject,
    private snackbar: MatSnackBar,
    private router : Router) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.actions$.pipe(
      filter((action: any) => action.type === '[OAuthScopes] COMPLETE_UPDATE_SCOPE'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('scopes.messages.update'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[OAuthScopes] ERROR_UPDATE_SCOPE'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('scopes.messages.errorUpdate'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[OAuthScopes] COMPLETE_DELETE_SCOPE'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('scopes.messages.delete'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.router.navigate(['/scopes']);
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[OAuthScopes] ERROR_DELETE_SCOPE'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('scopes.messages.errorDelete'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectOAuthScopeResult)).subscribe((scope: OAuthScope| null) => {
      if (!scope) {
        return;
      }

      this.scope$ = scope;
      this.claims = [...scope.Claims];
      this.isLoading = false;
      this.setScope();
    });
    this.refresh();
  }

  public removeClaim(claim: string) {
    const index = this.claims.indexOf(claim, 0);
    if (index > -1) {
      this.claims.splice(index, 1);
    }
  }

  public addClaim(evt: MatChipInputEvent) {
    const value = (evt.value || '').trim();
    if (value) {
      this.claims.push(value);
    }

    evt.input.value = "";
  }

  public saveScope(evt: any) {
    evt.preventDefault();
    const update = startUpdate({ claims: this.claims, name: this.scope$.Name });
    this.store.dispatch(update);
  }

  public delete() {
    const name = this.activatedRoute.snapshot.params['id'];
    let request = startDelete({ name: name });
    this.store.dispatch(request);
  }

  private refresh() {
    const name = this.activatedRoute.snapshot.params['id'];
    let request = startGet({ name: name });
    this.store.dispatch(request);
  }

  private setScope() {
    this.updateScopeForm.get('scopeName')?.setValue(this.scope$.Name);
  }
}
