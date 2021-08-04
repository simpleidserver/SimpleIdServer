import { RouterModule, Routes } from '@angular/router';
import { ListAuthenticationsComponent } from './list/list.component';
import { ViewAuthenticationComponent } from './view/view.component';


const routes: Routes = [
  {
    path: '',
    component: ListAuthenticationsComponent
  },
  {
    path: ':id',
    component: ViewAuthenticationComponent
  }
];

export const AuthenticationsRoutes = RouterModule.forChild(routes);
