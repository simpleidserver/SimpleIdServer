import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as fromReducers from '@app/stores/appstate';
import { Store } from '@ngrx/store';
import { UserEmail } from '@app/stores/users/models/useremail.model';

@Component({
  selector: 'edit-phonenumber',
  templateUrl: './edit-phonenumber.component.html'
})
export class EditPhoneNumberComponent {
  languages: string[] = [];
  editPhoneNumberForm: FormGroup = new FormGroup({
    value: new FormControl(),
    display: new FormControl(),
    type: new FormControl()
  });

  constructor(
    private store: Store<fromReducers.AppState>,
    @Inject(MAT_DIALOG_DATA) public data: UserEmail,
    private dialogRef: MatDialogRef<EditPhoneNumberComponent>) {
    this.editPhoneNumberForm.get('value')?.setValue(this.data.value);
    this.editPhoneNumberForm.get('display')?.setValue(this.data.display);
    this.editPhoneNumberForm.get('type')?.setValue(this.data.type);
  }

  save() {
    this.dialogRef.close(this.editPhoneNumberForm.value);
  }
}
