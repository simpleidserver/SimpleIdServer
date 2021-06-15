import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { startGet } from '@app/stores/users/actions/users.actions';
import { User } from '@app/stores/users/models/user.model';
import { select, Store } from '@ngrx/store';
import { UserPhoto } from '@app/stores/users/models/userphoto.model';
import { EditPhotoComponent } from './edit-photo.component';

@Component({
  selector: 'view-user',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewUserComponent implements OnInit {
  user$: User;
  photos: UserPhoto[] = [];
  currentPhotoIndex: number = 1;
  editUserFormGroup: FormGroup = new FormGroup({
    userName: new FormControl({ value: '' }),
    familyName: new FormControl({ value: '' }),
    givenName: new FormControl({ value: '' })
  });
  isLoading: boolean = false;
  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private dialog : MatDialog) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectUserResult)).subscribe((user: User | null) => {
      if (!user) {
        return;
      }

      this.user$ = user;
      this.isLoading = false;
      this.refreshEditForm();
    });
    this.refresh();
  }

  refresh() {
    this.isLoading = true;
    const userId = this.activatedRoute.snapshot.params['id'];
    let request = startGet({ userId: userId });
    this.store.dispatch(request);
  }

  addPhoto() {
    let photoRecord: UserPhoto = { display: '', type: '', value: '' };
    const dialogRef = this.dialog.open(EditPhotoComponent, {
      data: photoRecord
    })
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      this.photos.push(r as UserPhoto);
    });
  }

  nextPhoto() {
    this.currentPhotoIndex++;
  }

  previousPhoto() {
    this.currentPhotoIndex--;
  }

  removePhoto() {
    this.photos.splice(this.currentPhotoIndex - 1, 1);
    if (this.currentPhotoIndex > 1) {
      this.currentPhotoIndex--;
    }
  }

  editPhoto() {
    if (this.photos.length === 0) {
      return;
    }

    let photo = this.photos[this.currentPhotoIndex - 1];
    const dialogRef = this.dialog.open(EditPhotoComponent, {
      data: photo
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      let p = r as UserPhoto;
      photo.display = p.display;
      photo.type = p.type;
      photo.value = p.value;
    });
  }

  private refreshEditForm() {
    this.editUserFormGroup.get('userName')?.setValue(this.user$.userName);
    this.editUserFormGroup.get('familyName')?.setValue(this.user$.familyName);
    this.editUserFormGroup.get('givenName')?.setValue(this.user$.givenName);
  }
}
