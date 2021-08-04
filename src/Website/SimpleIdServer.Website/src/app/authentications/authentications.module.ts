import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { AvatarModule } from 'ngx-avatar';
import { PipesModule } from '../pipes/pipes.module';
import { AuthenticationsRoutes } from './authentications.routes';
import { ListAuthenticationsComponent } from './list/list.component';
import { ViewAuthenticationComponent } from './view/view.component';

@NgModule({
  imports: [
    AvatarModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    PipesModule,
    AuthenticationsRoutes
  ],
  declarations: [
    ListAuthenticationsComponent,
    ViewAuthenticationComponent
  ]
})

export class AuthenticationsModule { }
