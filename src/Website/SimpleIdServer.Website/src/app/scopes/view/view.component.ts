import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatChipInputEvent } from '@angular/material/chips';
import { ActivatedRoute } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { startGet } from '@app/stores/scopes/actions/scope.actions';
import { OAuthScope } from '@app/stores/scopes/models/oauthscope.model';
import { select, Store } from '@ngrx/store';

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
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.isLoading = true;
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

  public saveScope(evt: any, form: any) {
    evt.preventDefault();

  }

  public delete() {

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
