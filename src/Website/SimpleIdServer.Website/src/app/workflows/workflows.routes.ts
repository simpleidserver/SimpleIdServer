import { RouterModule, Routes } from '@angular/router';
import { ListWorkflowsComponent } from './list/list.component';


const routes: Routes = [
  {
    path: '',
    component: ListWorkflowsComponent
  }
];

export const WorkflowsRoutes = RouterModule.forChild(routes);
