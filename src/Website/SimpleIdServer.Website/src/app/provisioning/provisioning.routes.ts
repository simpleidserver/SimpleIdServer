import { RouterModule, Routes } from '@angular/router';
import { ProvisioningConfigurationHistoryComponent } from './history/history.component';


const routes: Routes = [
  {
    path: 'history',
    component: ProvisioningConfigurationHistoryComponent
  },
  {
    path: 'workflows',
    loadChildren: async () => (await import('./workflows/workflows.module')).WorkflowsModule
  }
];

export const ProvisioningRoutes = RouterModule.forChild(routes);
