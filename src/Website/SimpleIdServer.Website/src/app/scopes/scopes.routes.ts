import { RouterModule, Routes } from '@angular/router';
import { ListScopesComponent } from './list/list.component';
import { ViewScopeComponent } from './view/view.component';


const routes: Routes = [
  {
    path: '',
    component: ListScopesComponent
  },
  {
    path: ':id',
    component: ViewScopeComponent
  }
];

export const ScopesRoutes = RouterModule.forChild(routes);
