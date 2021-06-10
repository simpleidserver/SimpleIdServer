import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { AvatarModule } from 'ngx-avatar';
import { AddScopeComponent } from './list/add-scope.component';
import { ListScopesComponent } from './list/list.component';
import { ScopesRoutes } from './scopes.routes';
import { ViewScopeComponent } from './view/view.component';

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
    ListScopesComponent,
    ViewScopeComponent,
    AddScopeComponent
  ]
})

export class ScopesModule { }
