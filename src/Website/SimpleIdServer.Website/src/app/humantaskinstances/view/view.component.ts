import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { startGetInstance, startSubmitInstance } from '@app/stores/humantasks/actions/humantasks.actions';
import { HumanTaskInstanceState } from '@app/stores/humantasks/reducers';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { OAuthService } from 'angular-oauth2-oidc';
import { HumanTaskInstance } from '@app/stores/humantasks/models/humantaskinstance.model';
import { filter } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'view-humantaskinstance',
  templateUrl: './view.component.html'
})
export class ViewHumanTaskInstanceComponent implements OnInit, OnDestroy {
  subscription: any;
  isLoading: boolean;
  formGroup: FormGroup = new FormGroup({});
  option: any = null;
  humanTaskInstance: HumanTaskInstance;
  uiOption: any = {
    editMode: false
  };

  constructor(
    private store: Store<fromReducers.AppState>,
    private route: ActivatedRoute,
    private oauthService: OAuthService,
    private router: Router,
    private actions$: ScannedActionsSubject,
    private snackBar: MatSnackBar,
    private translateService: TranslateService) { }

  ngOnInit(): void {
    const claims: any = this.oauthService.getIdentityClaims();
    if (!claims) {
      this.route.queryParams.subscribe(params => {
        const auth = params['auth'];
        if (auth) {
          switch (auth) {
            case 'email':
              this.oauthService.customQueryParams = {
                'acr_values': 'sid-load-021',
                'redirect_url': this.router.url
              };
              break;
          }

          this.oauthService.initLoginFlow();
          return;
        }
      });
    }

    this.actions$.pipe(
      filter((action: any) => action.type === '[HumanTaskInstance] ERROR_SUBMIT_HUMANTASK_INSTANCE'))
      .subscribe(() => {
        this.isLoading = false;
        this.snackBar.open(this.translateService.instant('humantaskinstance.messages.errorCompleteTask'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[HumanTaskInstance] COMPLETE_SUBMIT_HUMANTASK_INSTANCE'))
      .subscribe(() => {
        this.isLoading = false;
        this.snackBar.open(this.translateService.instant('humantaskinstance.messages.taskCompleted'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.router.navigate(['/humantaskinstances']);
      });
    this.subscription = this.store.pipe(select(fromReducers.selectHumanTaskInstanceResult)).subscribe((humanTaskInstance: HumanTaskInstanceState | null) => {
      if (!humanTaskInstance) {
        return;
      }

      if (humanTaskInstance.task) {
        this.humanTaskInstance = humanTaskInstance.task;
      }

      if (humanTaskInstance.rendering) {
        this.option = humanTaskInstance.rendering;
      }

      this.isLoading = false;
    });
    this.refresh();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  onSubmit() {
    if (!this.formGroup.valid) {
      return;
    }

    this.isLoading = true;
    const id = this.route?.snapshot.params['id'];
    const req = startSubmitInstance({ id: id, operationParameters: this.formGroup.value });
    this.store.dispatch(req);
  }

  private refresh() {
    this.isLoading = true;
    const id = this.route?.snapshot.params['id'];
    this.store.dispatch(startGetInstance({ id: id }));
  }
}
