import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as fromReducers from '@app/stores/appstate';
import { Store } from '@ngrx/store';
import { UserEmail } from '@app/stores/users/models/useremail.model';

@Component({
  selector: 'edit-email',
  templateUrl: './edit-email.component.html'
})
export class EditEmailComponent {
  languages: string[] = [];
  editEmailForm: FormGroup = new FormGroup({
    value: new FormControl(),
    display: new FormControl(),
    type: new FormControl()
  });

  constructor(
    private store: Store<fromReducers.AppState>,
    @Inject(MAT_DIALOG_DATA) public data: UserEmail,
    private dialogRef: MatDialogRef<EditEmailComponent>) {
    this.editEmailForm.get('value')?.setValue(this.data.value);
    this.editEmailForm.get('display')?.setValue(this.data.display);
    this.editEmailForm.get('type')?.setValue(this.data.type);
  }

  save() {
    this.dialogRef.close(this.editEmailForm.value);
  }
}
