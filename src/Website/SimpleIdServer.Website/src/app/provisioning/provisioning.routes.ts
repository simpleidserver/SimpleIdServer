import { RouterModule, Routes } from '@angular/router';
import { ProvisioningConfigurationComponent } from './configuration/configuration.component';
import { ProvisioningConfigurationsComponent } from './configurations/configurations.component';
import { ProvisioningConfigurationHistoryComponent } from './history/history.component';


const routes: Routes = [
  {
    path: 'history',
    component: ProvisioningConfigurationHistoryComponent
  },
  {
    path: 'workflows',
    loadChildren: async () => (await import('./workflows/workflows.module')).WorkflowsModule
  },
  {
    path: 'configurations',
    component: ProvisioningConfigurationsComponent
  },
  {
    path: 'configurations/:id',
    component: ProvisioningConfigurationComponent
  }
];

export const ProvisioningRoutes = RouterModule.forChild(routes);
