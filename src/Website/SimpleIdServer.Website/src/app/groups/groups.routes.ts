import { RouterModule, Routes } from '@angular/router';
import { ListGroupsComponent } from './list/list.component';


const routes: Routes = [
  {
    path: '',
    component: ListGroupsComponent
  }
];

export const GroupsRoutes = RouterModule.forChild(routes);
