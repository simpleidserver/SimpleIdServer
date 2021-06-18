import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as fromReducers from '@app/stores/appstate';
import { Store } from '@ngrx/store';
import { UserAddress } from '@app/stores/users/models/useraddress.model';

@Component({
  selector: 'edit-address',
  templateUrl: './edit-address.component.html'
})
export class EditAddressComponent {
  languages: string[] = [];
  editAddressForm: FormGroup = new FormGroup({
    streetAddress: new FormControl(),
    formatted: new FormControl(),
    locality: new FormControl(),
    region: new FormControl(),
    postalCode: new FormControl(),
    type: new FormControl()
  });

  constructor(
    private store: Store<fromReducers.AppState>,
    @Inject(MAT_DIALOG_DATA) public data: UserAddress,
    private dialogRef: MatDialogRef<EditAddressComponent>) {
    this.editAddressForm.get('streetAddress')?.setValue(this.data.streetAddress);
    this.editAddressForm.get('formatted')?.setValue(this.data.formatted);
    this.editAddressForm.get('locality')?.setValue(this.data.locality);
    this.editAddressForm.get('region')?.setValue(this.data.region);
    this.editAddressForm.get('postalCode')?.setValue(this.data.postalCode);
    if (this.data.type) {
      this.editAddressForm.get('type')?.setValue(this.data.type);
    }
    else {
      this.editAddressForm.get('type')?.setValue('work');
    }
  }

  save() {
    this.dialogRef.close(this.editAddressForm.value);
  }
}
