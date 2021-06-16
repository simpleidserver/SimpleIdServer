import { DataSource } from '@angular/cdk/table';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { startDelete, startGet, startUpdate } from '@app/stores/users/actions/users.actions';
import { User } from '@app/stores/users/models/user.model';
import { UserPhoto } from '@app/stores/users/models/userphoto.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { Observable, ReplaySubject } from 'rxjs';
import { UserEmail } from '@app/stores/users/models/useremail.model';
import { UserPhoneNumber } from '@app/stores/users/models/userphonenumber.model';
import { EditEmailComponent } from './edit-email.component';
import { EditPhotoComponent } from './edit-photo.component';
import { EditPhoneNumberComponent } from './edit-phonenumber.component';
import { UserAddress } from '@app/stores/users/models/useraddress.model';
import { EditAddressComponent } from './edit-address.component';
import { UserRole } from '@app/stores/users/models/userrole.model';
import { EditRoleComponent } from './edit-role.component';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

class EmailDataSource extends DataSource<UserEmail> {
  private _dataStream = new ReplaySubject<UserEmail[]>();

  constructor(private initialData: UserEmail[]) {
    super();
    this.setData(initialData);
    this.data = initialData;
  }

  public data: UserEmail[];

  connect(): Observable<UserEmail[]> {
    return this._dataStream;
  }

  disconnect() { }

  setData(data: UserEmail[]) {
    this.data = data;
    this._dataStream.next(data);
  }
}

class PhoneNumberDataSource extends DataSource<UserPhoneNumber> {
  private _dataStream = new ReplaySubject<UserPhoneNumber[]>();

  constructor(private initialData: UserPhoneNumber[]) {
    super();
    this.setData(initialData);
    this.data = initialData;
  }

  public data: UserPhoneNumber[];

  connect(): Observable<UserPhoneNumber[]> {
    return this._dataStream;
  }

  disconnect() { }

  setData(data: UserPhoneNumber[]) {
    this.data = data;
    this._dataStream.next(data);
  }
}

class AddressDataSource extends DataSource<UserAddress> {
  private _dataStream = new ReplaySubject<UserAddress[]>();

  constructor(private initialData: UserAddress[]) {
    super();
    this.setData(initialData);
    this.data = initialData;
  }

  public data: UserAddress[];

  connect(): Observable<UserAddress[]> {
    return this._dataStream;
  }

  disconnect() { }

  setData(data: UserAddress[]) {
    this.data = data;
    this._dataStream.next(data);
  }
}

class RoleDataSource extends DataSource<UserRole> {
  private _dataStream = new ReplaySubject<UserRole[]>();

  constructor(private initialData: UserRole[]) {
    super();
    this.setData(initialData);
    this.data = initialData;
  }

  public data: UserRole[];

  connect(): Observable<UserRole[]> {
    return this._dataStream;
  }

  disconnect() { }

  setData(data: UserRole[]) {
    this.data = data;
    this._dataStream.next(data);
  }
}

@Component({
  selector: 'view-user',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewUserComponent implements OnInit {
  displayedEmailColumns: string[] = ['value', 'display', 'type', 'action'];
  displayedPhoneNumberColumns: string[] = ['value', 'display', 'type', 'action'];
  displayedAddressNumberColumns: string[] = ['streetAddress', 'postalCode', 'type', 'action'];
  displayedRoleColumns: string[] = ['value', 'display', 'type', 'action'];
  user$: User;
  photos$: UserPhoto[] = [];
  emails$: EmailDataSource;
  phoneNumbers$: PhoneNumberDataSource;
  addresses$: AddressDataSource;
  roles$: RoleDataSource;
  currentPhotoIndex: number = 1;
  editUserFormGroup: FormGroup = new FormGroup({
    externalId: new FormControl({ value: '' }),
    userName: new FormControl({ value: '' }),
    familyName: new FormControl({ value: '' }),
    givenName: new FormControl({ value: '' }),
    displayName: new FormControl({ value: '' }),
    nickName: new FormControl({ value: '' }),
    title: new FormControl({ value: '' }),
    profileUrl: new FormControl({ value: '' }),
    userType: new FormControl({ value: '' })
  });
  isLoading: boolean = false;
  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private snackbar: MatSnackBar,
    private translateService: TranslateService,
    private router : Router) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectUserResult)).subscribe((user: User | null) => {
      if (!user) {
        return;
      }

      this.user$ = { ...user };
      this.photos$ = JSON.parse(JSON.stringify(user.photos)) as UserPhoto[];
      this.emails$ = new EmailDataSource(JSON.parse(JSON.stringify(user.emails)) as UserEmail[]);
      this.phoneNumbers$ = new PhoneNumberDataSource(JSON.parse(JSON.stringify(user.phoneNumbers)) as UserPhoneNumber[]);
      this.addresses$ = new AddressDataSource(JSON.parse(JSON.stringify(user.addresses)) as UserAddress[]);
      this.roles$ = new RoleDataSource(JSON.parse(JSON.stringify(user.roles)) as UserRole[]);
      this.isLoading = false;
      this.refreshEditForm();
    });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Users] ERROR_UPDATE_USER'))
      .subscribe(() => {
        this.isLoading = false;
        this.snackbar.open(this.translateService.instant('users.messages.errorUpdate'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Users] COMPLETE_UPDATE_USER'))
      .subscribe(() => {
        this.user$.userName = this.editUserFormGroup.get('userName')?.value;
        this.isLoading = false;
        this.snackbar.open(this.translateService.instant('users.messages.update'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Users] ERROR_DELETE_USER'))
      .subscribe(() => {
        this.isLoading = false;
        this.snackbar.open(this.translateService.instant('users.messages.errorDelete'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Users] COMPLETE_DELETE_USER'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('users.messages.delete'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.router.navigate(['/users']);
      });
    this.refresh();
  }

  refresh() {
    this.isLoading = true;
    const userId = this.activatedRoute.snapshot.params['id'];
    let request = startGet({ userId: userId });
    this.store.dispatch(request);
  }

  addPhoto(evt: any) {
    evt.preventDefault();
    let photoRecord: UserPhoto = { display: '', type: '', value: '' };
    const dialogRef = this.dialog.open(EditPhotoComponent, {
      data: photoRecord
    })
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      this.photos$.push(r as UserPhoto);
    });
  }

  nextPhoto(evt: any) {
    evt.preventDefault();
    this.currentPhotoIndex++;
  }

  previousPhoto(evt: any) {
    evt.preventDefault();
    this.currentPhotoIndex--;
  }

  removePhoto(evt: any) {
    evt.preventDefault();
    this.photos$.splice(this.currentPhotoIndex - 1, 1);
    if (this.currentPhotoIndex > 1) {
      this.currentPhotoIndex--;
    }
  }

  editPhoto(evt: any) {
    evt.preventDefault();
    if (this.photos$.length === 0) {
      return;
    }

    let photo = this.photos$[this.currentPhotoIndex - 1];
    const dialogRef = this.dialog.open(EditPhotoComponent, {
      data: photo
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      const updatedPhoto = r as UserPhoto;
      photo.display = updatedPhoto.display;
      photo.type = updatedPhoto.type;
      photo.value = updatedPhoto.value;
    });
  }

  addEmail(evt: any) {
    evt.preventDefault();
    const userEmail: UserEmail = { display: '', type: '', value: '' };
    const dialogRef = this.dialog.open(EditEmailComponent, {
      data: userEmail
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      const emails = [
        ...this.emails$.data,
        r as UserEmail
      ];
      this.emails$.setData(emails);
    });
  }

  removeEmail(evt: any, email: UserEmail) {
    evt.preventDefault();
    const emails = this.emails$.data;
    const index = emails.indexOf(email);
    emails.splice(index, 1);
    this.emails$.setData(emails);
  }

  editEmail(evt: any, email: UserEmail, index: number) {
    evt.preventDefault();
    const dialogRef = this.dialog.open(EditEmailComponent, {
      data: email
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      const updatedEmail = r as UserEmail;
      const emails = this.emails$.data;
      const em = emails[index];
      em.display = updatedEmail.display;
      em.type = updatedEmail.type;
      em.value = updatedEmail.value;
      this.emails$.setData(emails);
    });
  }

  addPhoneNumber(evt: any) {
    evt.preventDefault();
    const userPhoneNumber: UserPhoneNumber = { display: '', type: '', value: '' };
    const dialogRef = this.dialog.open(EditPhoneNumberComponent, {
      data: userPhoneNumber
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      const phoneNumbers = [
        ...this.phoneNumbers$.data,
        r as UserPhoneNumber
      ];
      this.phoneNumbers$.setData(phoneNumbers);
    });
  }

  removePhoneNumber(evt: any, phoneNumber: UserPhoneNumber) {
    evt.preventDefault();
    const phoneNumbers = this.phoneNumbers$.data;
    const index = phoneNumbers.indexOf(phoneNumber);
    phoneNumbers.splice(index, 1);
    this.phoneNumbers$.setData(phoneNumbers);
  }

  editPhoneNumber(evt: any, phoneNumber: UserPhoneNumber, index: number) {
    evt.preventDefault();
    const dialogRef = this.dialog.open(EditPhoneNumberComponent, {
      data: phoneNumber
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      const updatedPhoneNumber = r as UserPhoneNumber;
      const phoneNumbers = this.phoneNumbers$.data;
      const em = phoneNumbers[index];
      em.display = updatedPhoneNumber.display;
      em.type = updatedPhoneNumber.type;
      em.value = updatedPhoneNumber.value;
      this.phoneNumbers$.setData(phoneNumbers);
    });
  }

  addAddress(evt: any) {
    evt.preventDefault();
    const userAddress: UserAddress = { type: '', country: '', formatted: '', locality: '', postalCode: '', region: '', streetAddress: '' };
    const dialogRef = this.dialog.open(EditAddressComponent, {
      data: userAddress
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      const addresses = [
        ...this.addresses$.data,
        r as UserAddress
      ];
      this.addresses$.setData(addresses);
    });
  }

  removeAddress(evt: any, address: UserAddress) {
    evt.preventDefault();
    const addresses = this.addresses$.data;
    const index = addresses.indexOf(address);
    addresses.splice(index, 1);
    this.addresses$.setData(addresses);
  }

  editAddress(evt: any, address: UserAddress, index: number) {
    evt.preventDefault();
    const dialogRef = this.dialog.open(EditAddressComponent, {
      data: address
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      const updatedAddress = r as UserAddress;
      const addresses = this.addresses$.data;
      const addr = addresses[index];
      addr.type = updatedAddress.type;
      addr.country = updatedAddress.country;
      addr.formatted = updatedAddress.formatted;
      addr.locality = updatedAddress.locality;
      addr.postalCode = updatedAddress.postalCode;
      addr.region = updatedAddress.region;
      addr.streetAddress = updatedAddress.streetAddress;
      addr.type = updatedAddress.type;
      this.addresses$.setData(addresses);
    });
  }

  addRole(evt: any) {
    evt.preventDefault();
    const userRole: UserRole = { type: '', display: '', value: ''};
    const dialogRef = this.dialog.open(EditRoleComponent, {
      data: userRole
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      const roles = [
        ...this.roles$.data,
        r as UserRole
      ];
      this.roles$.setData(roles);
    });
  }

  removeRole(evt: any, address: UserRole) {
    evt.preventDefault();
    const roles = this.roles$.data;
    const index = roles.indexOf(address);
    roles.splice(index, 1);
    this.roles$.setData(roles);
  }

  editRole(evt: any, role: UserRole, index: number) {
    evt.preventDefault();
    const dialogRef = this.dialog.open(EditRoleComponent, {
      data: role
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      const updatedRole = r as UserRole;
      const roles = this.roles$.data;
      const role = roles[index];
      role.type = updatedRole.type;
      role.display = updatedRole.display;
      role.value = updatedRole.value;
      this.roles$.setData(roles);
    });
  }

  delete() {
    const deleteUser = startDelete({ userId: this.user$.id });
    this.store.dispatch(deleteUser);
  }

  saveUser(evt: any, formValue: any) {
    evt.preventDefault();
    var request : any = {
      schemas: ["urn:ietf:params:scim:schemas:core:2.0:User"],
      externalId: formValue.externalId,
      userName: formValue.userName,
      name: {
        familyName: formValue.familyName,
        givenName: formValue.givenName
      },
      displayName: formValue.displayName,
      nickName: formValue.nickName,
      title: formValue.title,
      profileUrl: formValue.profileUrl,
      userType: formValue.userType,
      photos: this.photos$,
      emails: this.emails$.data,
      phoneNumbers: this.phoneNumbers$.data,
      addresses: this.addresses$.data,
      roles: this.roles$.data
    };
    this.isLoading = true;
    const updateUser = startUpdate({ userId: this.user$.id, request: request });
    this.store.dispatch(updateUser);
  }

  private refreshEditForm() {
    this.editUserFormGroup.get('externalId')?.setValue(this.user$.externalId);
    this.editUserFormGroup.get('userName')?.setValue(this.user$.userName);
    this.editUserFormGroup.get('familyName')?.setValue(this.user$.familyName);
    this.editUserFormGroup.get('givenName')?.setValue(this.user$.givenName);
    this.editUserFormGroup.get('displayName')?.setValue(this.user$.displayName);
    this.editUserFormGroup.get('nickName')?.setValue(this.user$.nickName);
    this.editUserFormGroup.get('title')?.setValue(this.user$.title);
    this.editUserFormGroup.get('profileUrl')?.setValue(this.user$.profileUrl);
    this.editUserFormGroup.get('userType')?.setValue(this.user$.userType);
  }
}
