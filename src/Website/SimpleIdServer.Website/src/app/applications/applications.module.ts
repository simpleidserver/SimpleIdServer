import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { TranslateEnumPipe } from '@app/pipes/translateenum.pipe';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { AvatarModule } from 'ngx-avatar';
import { TranslateMetadataPipe } from '../pipes/translatemetadata';
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
    ApplicationsRoutes
  ],
  declarations: [
    ListApplicationsComponent,
    ViewApplicationsComponent,
    EditTranslationComponent,
    DisplayJwkComponent,
    AddApplicationComponent,
    TranslateEnumPipe,
    TranslateMetadataPipe
  ]
})

export class ApplicationsModule { }
