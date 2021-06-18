import { RouterModule, Routes } from '@angular/router';
import { ListUsersComponent } from './list/list.component';


const routes: Routes = [
  {
    path: '',
    component: ListUsersComponent
  },
  {
    path: ':id',
    loadChildren: async () => (await import('./view/view.module')).ViewUserModule
  }
];

export const UsersRoutes = RouterModule.forChild(routes);
