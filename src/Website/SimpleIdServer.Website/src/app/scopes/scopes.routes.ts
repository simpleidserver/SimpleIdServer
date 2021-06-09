import { RouterModule, Routes } from '@angular/router';
import { ListScopesComponent } from './list/list.component';


const routes: Routes = [
  {
    path: '',
    component: ListScopesComponent
  }
];

export const ScopesRoutes = RouterModule.forChild(routes);
