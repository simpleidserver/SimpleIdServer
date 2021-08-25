import { RouterModule, Routes } from '@angular/router';
import { ListRelyingPartiesComponent } from './list/list.component';
import { ViewRelyingPartyComponent } from './view/view.component';


const routes: Routes = [
  {
    path: '',
    component: ListRelyingPartiesComponent
  },
  {
    path: ':id',
    component: ViewRelyingPartyComponent
  }
];

export const RelyingPartiesRoutes = RouterModule.forChild(routes);
