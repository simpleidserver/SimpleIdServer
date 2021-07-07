import { RouterModule, Routes } from '@angular/router';
import { ViewDetailsComponent } from './details/details.component';
import { ViewEditorComponent } from './editor/editor.component';
import { ViewInstanceComponent } from './instance/instance.component';
import { ViewInstancesComponent } from './instances/instances.component';
import { ViewWorkflowComponent } from './view.component';

const routes: Routes = [
  {
    path: '',
    component: ViewWorkflowComponent,
    children: [
      {
        path: '',
        redirectTo: 'details'
      },
      {
        path: 'details',
        component: ViewDetailsComponent
      },
      {
        path: 'editor',
        component: ViewEditorComponent
      },
      {
        path: 'instances',
        component: ViewInstancesComponent
      }
    ]
  },
  {
    path: 'instances/:instanceid',
    component: ViewInstanceComponent
  }
];

export const ViewWorkflowRoutes = RouterModule.forChild(routes);
