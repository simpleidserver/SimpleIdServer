import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SIDCommonModule } from '@app/common/sidcommon.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { AvatarModule } from 'ngx-avatar';
import { ProvisioningConfigurationComponent } from './configuration/configuration.component';
import { ProvisioningConfigurationsComponent } from './configurations/configurations.component';
import { ProvisioningConfigurationHistoryComponent } from './history/history.component';
import { ProvisioningRoutes } from './provisioning.routes';

@NgModule({
  imports: [
    AvatarModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    SIDCommonModule,
    ProvisioningRoutes
  ],
  declarations: [
    ProvisioningConfigurationHistoryComponent,
    ProvisioningConfigurationsComponent,
    ProvisioningConfigurationComponent
  ]
})

export class ProvisioningModule { }
