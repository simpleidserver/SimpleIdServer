import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as fromReducers from '@app/stores/appstate';
import { Store } from '@ngrx/store';
import { UserRole } from '@app/stores/users/models/userrole.model';

@Component({
  selector: 'edit-role',
  templateUrl: './edit-role.component.html'
})
export class EditRoleComponent {
  languages: string[] = [];
  editRoleForm: FormGroup = new FormGroup({
    value: new FormControl(),
    display: new FormControl(),
    type: new FormControl()
  });

  constructor(
    private store: Store<fromReducers.AppState>,
    @Inject(MAT_DIALOG_DATA) public data: UserRole,
    private dialogRef: MatDialogRef<EditRoleComponent>) {
    this.editRoleForm.get('value')?.setValue(this.data.value);
    this.editRoleForm.get('display')?.setValue(this.data.display);
    this.editRoleForm.get('type')?.setValue(this.data.type);
  }

  save() {
    this.dialogRef.close(this.editRoleForm.value);
  }
}
