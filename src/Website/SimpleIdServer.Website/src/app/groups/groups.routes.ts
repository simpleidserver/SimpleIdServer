import { RouterModule, Routes } from '@angular/router';
import { ListGroupsComponent } from './list/list.component';
import { ViewGroupComponent } from './view/view.component';


const routes: Routes = [
  {
    path: '',
    component: ListGroupsComponent
  },
  {
    path: ':id',
    component: ViewGroupComponent
  }
];

export const GroupsRoutes = RouterModule.forChild(routes);
