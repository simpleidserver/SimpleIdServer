import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { ListUsersComponent } from './list/list.component';
import { UsersRoutes } from './users.routes';
import { EditAddressComponent } from './view/edit-address.component';
import { EditEmailComponent } from './view/edit-email.component';
import { EditPhoneNumberComponent } from './view/edit-phonenumber.component';
import { EditPhotoComponent } from './view/edit-photo.component';
import { EditRoleComponent } from './view/edit-role.component';
import { ViewUserComponent } from './view/view.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    UsersRoutes
  ],
  declarations: [
    ListUsersComponent,
    ViewUserComponent,
    EditPhotoComponent,
    EditEmailComponent,
    EditPhoneNumberComponent,
    EditAddressComponent,
    EditRoleComponent
  ]
})

export class UsersModule { }
