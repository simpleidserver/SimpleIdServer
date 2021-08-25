import { DataSource } from '@angular/cdk/collections';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { startDelete, startGet, startUpdate } from '@app/stores/relyingparties/actions/relyingparty.actions';
import { RelyingPartyClaimMapping } from '@app/stores/relyingparties/models/relyingparty-claimapping.model';
import { RelyingParty } from '@app/stores/relyingparties/models/relyingparty.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { Observable, ReplaySubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { EditClaimComponent } from './edit-claim.component';

class ClaimMappingDataSource extends DataSource<RelyingPartyClaimMapping> {
  private _dataStream = new ReplaySubject<RelyingPartyClaimMapping[]>();

  constructor(private initialData: RelyingPartyClaimMapping[]) {
    super();
    this.setData(initialData);
    this.data = initialData;
  }

  public data: RelyingPartyClaimMapping[];

  connect(): Observable<RelyingPartyClaimMapping[]> {
    return this._dataStream;
  }

  disconnect() { }

  setData(data: RelyingPartyClaimMapping[]) {
    this.data = data;
    this._dataStream.next(data);
  }
}

@Component({
  selector: 'view-relyingparty',
  templateUrl: './view.component.html'
})
export class ViewRelyingPartyComponent implements OnInit {
  relyingParty: RelyingParty;
  claims$: ClaimMappingDataSource;
  isLoading: boolean = false;
  updateRelyingPartyFormGroup: FormGroup = new FormGroup({
    id: new FormControl({ value: '', disabled: true }),
    assertionExpirationTimeInSeconds: new FormControl({ value: '' }),
    metadataUrl: new FormControl({ value: '' }, [ Validators.required ])
  });
  displayedClaimColumns: string[] = ['userAttribute', 'claimName', 'action'];

  constructor(
    private store: Store<fromReducers.AppState>,
    private route: ActivatedRoute,
    private translateService: TranslateService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar,
    private actions$: ScannedActionsSubject,
    private router : Router) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.store.pipe(select(fromReducers.selectRelyingPartyResult)).subscribe((relyingParty: RelyingParty | null) => {
      if (!relyingParty) {
        return;
      }

      this.isLoading = false;
      this.relyingParty = relyingParty;
      this.claims$ = new ClaimMappingDataSource(JSON.parse(JSON.stringify(relyingParty.claimMappings)) as RelyingPartyClaimMapping[]);
      this.updateRelyingPartyFormGroup.get('id')?.setValue(this.relyingParty.id);
      this.updateRelyingPartyFormGroup.get('metadataUrl')?.setValue(this.relyingParty.metadataUrl);
      this.updateRelyingPartyFormGroup.get('assertionExpirationTimeInSeconds')?.setValue(this.relyingParty.assertionExpirationTimeInSeconds);
    });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] COMPLETE_UPDATE_RELYINGPARTY'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('relyingParties.messages.update'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] ERROR_UPDATE_RELYINGPARTY'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('relyingParties.messages.errorUpdate'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] COMPLETE_DELETE_RELYINGPARTY'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('relyingParties.messages.delete'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.router.navigate(['/relyingparties']);
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] ERROR_DELETE_RELYINGPARTY'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('relyingParties.messages.errorDelete'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.refresh();
  }

  refresh() {
    var id = this.route.snapshot.params['id'];
    let getRelyingParty = startGet({ id: id });
    this.store.dispatch(getRelyingParty);
  }

  saveRelyingParty(evt : any, formValue : any) {
    evt.preventDefault();
    const request: any = {
      metadata_url: formValue.metadataUrl,
      assertion_expiration_time_seconds: formValue.assertionExpirationTimeInSeconds,
      claim_mappings: this.claims$.data.map((c) => {
        return {
          claim_format: c.claimFormat,
          claim_name: c.claimName,
          user_attribute: c.userAttribute
        };
      })
    };
    const update = startUpdate({ id: this.relyingParty.id, request: request });
    this.store.dispatch(update);
  }

  delete() {
    let deleteRelyingParty = startDelete({ id: this.relyingParty.id });
    this.store.dispatch(deleteRelyingParty);
  }

  addClaimMapping(evt: any) {
    evt.preventDefault();
    const userEmail: RelyingPartyClaimMapping = { claimFormat: '', claimName: '', userAttribute: '' };
    const dialogRef = this.dialog.open(EditClaimComponent, {
      data: userEmail
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      const emails = [
        ...this.claims$.data,
        r as RelyingPartyClaimMapping
      ];
      this.claims$.setData(emails);
    });
  }

  removeRelyingParty(evt: any, claim: RelyingPartyClaimMapping) {
    evt.preventDefault();
    const claims = this.claims$.data;
    const index = claims.indexOf(claim);
    claims.splice(index, 1);
    this.claims$.setData(claims);
  }

  editRelyingParty(evt: any, claim: RelyingPartyClaimMapping, index: number) {
    evt.preventDefault();
    const dialogRef = this.dialog.open(EditClaimComponent, {
      data: claim
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      const updatedClaim = r as RelyingPartyClaimMapping;
      const claims = this.claims$.data;
      const em = claims[index];
      em.claimFormat = updatedClaim.claimFormat;
      em.claimName = updatedClaim.claimName;
      em.userAttribute = updatedClaim.userAttribute;
      this.claims$.setData(claims);
    });
  }

  getRelyingPartyId() {
    return this.relyingParty.id;
  }
}
