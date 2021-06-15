import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as fromReducers from '@app/stores/appstate';
import { Store } from '@ngrx/store';
import { UserPhoto } from '../../stores/users/models/userphoto.model';

@Component({
  selector: 'edit-photo',
  templateUrl: './edit-photo.component.html'
})
export class EditPhotoComponent {
  photo: UserPhoto | null= null;
  languages: string[] = [];
  editPhotoForm: FormGroup = new FormGroup({
    value: new FormControl(),
    display: new FormControl(),
    type: new FormControl()
  });

  constructor(
    private store: Store<fromReducers.AppState>,
    @Inject(MAT_DIALOG_DATA) public data: UserPhoto,
    private dialogRef: MatDialogRef<EditPhotoComponent>) {
    this.editPhotoForm.get('value')?.setValue(this.data.value);
    this.editPhotoForm.get('display')?.setValue(this.data.display);
    if (this.data.type) {
      this.editPhotoForm.get('type')?.setValue(this.data.type);
    } else {
      this.editPhotoForm.get('type')?.setValue('photo');
    }
  }

  save() {
    this.dialogRef.close(this.editPhotoForm.value);
  }
}
