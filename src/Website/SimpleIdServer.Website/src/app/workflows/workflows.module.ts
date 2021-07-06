import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { ListWorkflowsComponent } from './list/list.component';
import { WorkflowsRoutes } from './workflows.routes';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    WorkflowsRoutes
  ],
  declarations: [
    ListWorkflowsComponent
  ]
})

export class WorkflowsModule { }
