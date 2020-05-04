import { RouterModule, Routes } from '@angular/router';
import { ListOauthClientsComponent } from './list/list.component';
import { ViewOauthClientsComponent } from './view/view.component';

const routes: Routes = [
    { path: '', redirectTo: 'list', pathMatch: 'full' },
    { path: 'list', component: ListOauthClientsComponent },
    { path: ':id', component: ViewOauthClientsComponent }
];

export const OAuthClientRoutes = RouterModule.forChild(routes);