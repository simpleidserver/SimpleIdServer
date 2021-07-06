import { RouterModule, Routes } from '@angular/router';
import { ListWorkflowsComponent } from './list/list.component';

const routes: Routes = [
  {
    path: '',
    component: ListWorkflowsComponent
  },
  {
    path: ':id',
    loadChildren: async () => (await import('./view/view.module')).ViewWorkflowModule
  }
];

export const WorkflowsRoutes = RouterModule.forChild(routes);
