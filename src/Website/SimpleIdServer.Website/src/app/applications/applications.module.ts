import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateEnumPipe } from '@app/pipes/translateenum.pipe';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { AvatarModule } from 'ngx-avatar';
import { ApplicationsRoutes } from './applications.routes';
import { ListApplicationsComponent } from './list/list.component';
import { ViewApplicationsComponent } from './view/view.component';

@NgModule({
  imports: [
    AvatarModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    ApplicationsRoutes
  ],
  declarations: [
    ListApplicationsComponent,
    ViewApplicationsComponent,
    TranslateEnumPipe
  ]
})

export class ApplicationsModule { }
