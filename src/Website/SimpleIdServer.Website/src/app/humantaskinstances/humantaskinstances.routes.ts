import { RouterModule, Routes } from '@angular/router';
import { ListHumanTaskInstanceComponent } from './list/list.component';
import { ViewHumanTaskInstanceComponent } from './view/view.component';

const routes: Routes = [
  {
    path: ':id',
    component: ViewHumanTaskInstanceComponent
  },
  {
    path: '',
    component: ListHumanTaskInstanceComponent
  }
];

export const HumanTaskInstancesRoutes = RouterModule.forChild(routes);
