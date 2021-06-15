import { RouterModule, Routes } from '@angular/router';
import { ListUsersComponent } from './list/list.component';
import { ViewUserComponent } from './view/view.component';


const routes: Routes = [
  {
    path: '',
    component: ListUsersComponent
  },
  {
    path: ':id',
    component: ViewUserComponent
  }
];

export const UsersRoutes = RouterModule.forChild(routes);
