import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { AvatarModule } from 'ngx-avatar';
import { PipesModule } from '../pipes/pipes.module';
import { ApplicationsRoutes } from './applications.routes';
import { AddApplicationComponent } from './list/add-application.component';
import { ListApplicationsComponent } from './list/list.component';
import { DisplayJwkComponent } from './view/displayjwk.component';
import { EditTranslationComponent } from './view/edit-translation.component';
import { ViewApplicationsComponent } from './view/view.component';

@NgModule({
  imports: [
    AvatarModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    PipesModule,
    ApplicationsRoutes
  ],
  declarations: [
    ListApplicationsComponent,
    ViewApplicationsComponent,
    EditTranslationComponent,
    DisplayJwkComponent,
    AddApplicationComponent
  ]
})

export class ApplicationsModule { }
