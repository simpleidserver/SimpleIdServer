import { RouterModule, Routes } from '@angular/router';
import { ViewConsentsComponent } from './consents/consents.component';
import { ViewDetailsComponent } from './details/details.component';
import { ViewUserComponent } from './view.component';


const routes: Routes = [
  {
    path: '',
    component: ViewUserComponent,
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
        path: 'consents',
        component: ViewConsentsComponent
      }
    ]
  }
];

export const ViewUserRoutes = RouterModule.forChild(routes);
