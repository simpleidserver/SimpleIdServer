import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { ListUsersComponent } from './list/list.component';
import { UsersRoutes } from './users.routes';
import { EditPhotoComponent } from './view/edit-photo.component';
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
    EditPhotoComponent
  ]
})

export class UsersModule { }
