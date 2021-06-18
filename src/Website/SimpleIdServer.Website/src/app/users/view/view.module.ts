import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { ViewConsentsComponent } from './consents/consents.component';
import { ViewDetailsComponent } from './details/details.component';
import { EditAddressComponent } from './details/edit-address.component';
import { EditEmailComponent } from './details/edit-email.component';
import { EditPhoneNumberComponent } from './details/edit-phonenumber.component';
import { EditPhotoComponent } from './details/edit-photo.component';
import { EditRoleComponent } from './details/edit-role.component';
import { ViewUserComponent } from './view.component';
import { ViewUserRoutes } from './view.routes';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    ViewUserRoutes
  ],
  declarations: [
    ViewConsentsComponent,
    ViewDetailsComponent,
    EditAddressComponent,
    EditEmailComponent,
    EditPhoneNumberComponent,
    EditPhotoComponent,
    EditRoleComponent,
    ViewUserComponent
  ]
})

export class ViewUserModule { }
