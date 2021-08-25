import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as fromReducers from '@app/stores/appstate';
import { RelyingPartyClaimMapping } from '@app/stores/relyingparties/models/relyingparty-claimapping.model';
import { Store } from '@ngrx/store';

@Component({
  selector: 'edit-claim',
  templateUrl: './edit-claim.component.html'
})
export class EditClaimComponent {
  languages: string[] = [];
  editClaimForm: FormGroup = new FormGroup({
    userAttribute: new FormControl(),
    claimName: new FormControl()
  });

  constructor(
    private store: Store<fromReducers.AppState>,
    @Inject(MAT_DIALOG_DATA) public data: RelyingPartyClaimMapping,
    private dialogRef: MatDialogRef<EditClaimComponent>) {
    if (this.data) {
      this.editClaimForm.get('userAttribute')?.setValue(this.data.userAttribute);
      this.editClaimForm.get('claimName')?.setValue(this.data.claimName);
    }
  }

  save() {
    this.dialogRef.close(this.editClaimForm.value);
  }
}
