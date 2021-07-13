import { RouterModule, Routes } from '@angular/router';
import { ViewHumanTaskInstanceComponent } from './view/view.component';

const routes: Routes = [
  {
    path: ':instanceId',
    component: ViewHumanTaskInstanceComponent
  }
];

export const HumanTaskInstancesRoutes = RouterModule.forChild(routes);
