import { RouterModule, Routes } from '@angular/router';
import { ListApplicationsComponent } from './list/list.component';
import { ViewApplicationsComponent } from './view/view.component';


const routes: Routes = [
  {
    path: '',
    component: ListApplicationsComponent
  },
  {
    path: ':id',
    component: ViewApplicationsComponent
  }
];

export const ApplicationsRoutes = RouterModule.forChild(routes);
