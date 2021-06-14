import { RouterModule, Routes } from '@angular/router';
import { ListUsersComponent } from './list/list.component';


const routes: Routes = [
  {
    path: '',
    component: ListUsersComponent
  }
];

export const UsersRoutes = RouterModule.forChild(routes);
