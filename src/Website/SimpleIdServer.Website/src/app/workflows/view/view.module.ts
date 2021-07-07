import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { ViewDetailsComponent } from './details/details.component';
import { ViewEditorComponent } from './editor/editor.component';
import { ViewInstancesComponent } from './instances/instances.component';
import { ViewWorkflowComponent } from './view.component';
import { ViewWorkflowRoutes } from './view.routes';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    ViewWorkflowRoutes
  ],
  declarations: [
    ViewDetailsComponent,
    ViewWorkflowComponent,
    ViewEditorComponent,
    ViewInstancesComponent
  ]
})

export class ViewWorkflowModule { }
