import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { ListUsersComponent } from './list/list.component';
import { UsersRoutes } from './users.routes';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    UsersRoutes
  ],
  declarations: [
    ListUsersComponent
  ]
})

export class UsersModule { }
