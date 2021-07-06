import { RouterModule, Routes } from '@angular/router';
import { ViewDetailsComponent } from './details/details.component';
import { ViewEditorComponent } from './editor/editor.component';
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
      }
    ]
  }
];

export const ViewWorkflowRoutes = RouterModule.forChild(routes);
