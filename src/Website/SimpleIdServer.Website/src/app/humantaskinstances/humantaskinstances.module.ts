import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { AvatarModule } from 'ngx-avatar';
import { RenderingModule } from './common/rendering/rendering.module';
import { HumanTaskInstancesRoutes } from './humantaskinstances.routes';
import { ListHumanTaskInstanceComponent } from './list/list.component';
import { ViewHumanTaskInstanceComponent } from './view/view.component';

@NgModule({
  imports: [
    AvatarModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    HumanTaskInstancesRoutes,
    RenderingModule
  ],
  declarations: [
    ViewHumanTaskInstanceComponent,
    ListHumanTaskInstanceComponent
  ]
})

export class HumanTaskInstancesModule { }
