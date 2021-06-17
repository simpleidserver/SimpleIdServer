import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { AvatarModule } from 'ngx-avatar';
import { GroupsRoutes } from './groups.routes';
import { ListGroupsComponent } from './list/list.component';
import { SelectUsersComponent } from './view/selectusers.component';
import { ViewGroupComponent } from './view/view.component';

@NgModule({
  imports: [
    AvatarModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    GroupsRoutes
  ],
  declarations: [
    ListGroupsComponent,
    ViewGroupComponent,
    SelectUsersComponent
  ]
})

export class GroupsModule { }
