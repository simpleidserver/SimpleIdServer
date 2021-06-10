import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { AvatarModule } from 'ngx-avatar';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { ListScopesComponent } from './list/list.component';
import { ScopesRoutes } from './scopes.routes';

@NgModule({
  imports: [
    AvatarModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    ScopesRoutes
  ],
  declarations: [
    ListScopesComponent
  ]
})

export class ScopesModule { }
