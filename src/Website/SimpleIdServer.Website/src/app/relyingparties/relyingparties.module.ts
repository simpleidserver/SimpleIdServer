import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { AvatarModule } from 'ngx-avatar';
import { PipesModule } from '../pipes/pipes.module';
import { AddRelyingPartyComponent } from './list/add-relyingparty.component';
import { ListRelyingPartiesComponent } from './list/list.component';
import { RelyingPartiesRoutes } from './relyingparties.routes';
import { EditClaimComponent } from './view/edit-claim.component';
import { ViewRelyingPartyComponent } from './view/view.component';

@NgModule({
  imports: [
    AvatarModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    PipesModule,
    RelyingPartiesRoutes
  ],
  declarations: [
    ListRelyingPartiesComponent,
    ViewRelyingPartyComponent,
    AddRelyingPartyComponent,
    EditClaimComponent
  ]
})

export class RelyingPartiesModule { }
