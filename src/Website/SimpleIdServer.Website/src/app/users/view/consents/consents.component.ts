import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { startGet, startGetOpenId, startProvision } from '@app/stores/users/actions/users.actions';
import { UserOpenId } from '@app/stores/users/models/user-openid.model';
import { User } from '@app/stores/users/models/user.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'view-consents',
  templateUrl: './consents.component.html',
  styleUrls: ['./consents.component.scss']
})
export class ViewConsentsComponent implements OnInit {
  isLoadingUserOpenId: boolean;
  isLoadingUser: boolean;
  openidUserDoesntExit: boolean;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private actions$: ScannedActionsSubject,
    private snackbar: MatSnackBar,
    private translateService: TranslateService) {

  }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[Users] ERROR_GET_OPENID_USER'))
      .subscribe(() => {
        this.openidUserDoesntExit = true;
        this.isLoadingUserOpenId = false;
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Users] COMPLETE_PROVISION'))
      .subscribe(() => {
        this.isLoadingUserOpenId = false;
        this.snackbar.open(this.translateService.instant('users.messages.provision'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Users] ERROR_PROVISION'))
      .subscribe(() => {
        this.isLoadingUserOpenId = false;
        this.snackbar.open(this.translateService.instant('users.messages.errorProvision'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });

    this.store.pipe(select(fromReducers.selectUserOpenIdResult)).subscribe((user: UserOpenId | null) => {
      if (!user) {
        return;
      }

      this.isLoadingUserOpenId = false;
    });
    this.store.pipe(select(fromReducers.selectUserResult)).subscribe((user: User | null) => {
      if (!user) {
        return;
      }

      this.isLoadingUser = false;
    });
    this.refresh();
  }

  create() {
    this.isLoadingUserOpenId = true;
    const scimId = this.activatedRoute.parent?.snapshot.params['id'];
    const p = startProvision({ scimId: scimId });
    this.store.dispatch(p);
  }

  cancel() {
    this.openidUserDoesntExit = false;
  }

  private refresh() {
    this.isLoadingUserOpenId = true;
    this.isLoadingUser = true;
    const scimId = this.activatedRoute.parent?.snapshot.params['id'];
    const getUserOpenId = startGetOpenId({ scimId: scimId });
    const getUser = startGet({ userId: scimId });
    this.store.dispatch(getUserOpenId);
    this.store.dispatch(getUser);
  }
}
