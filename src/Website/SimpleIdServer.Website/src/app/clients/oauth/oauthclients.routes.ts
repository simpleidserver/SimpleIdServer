import { RouterModule, Routes } from '@angular/router';
import { ListOauthClientsComponent } from './list/list.component';

const routes: Routes = [
    { path: '', redirectTo: 'list', pathMatch: 'full' },
    { path: 'list', component: ListOauthClientsComponent }
];

export const OAuthClientRoutes = RouterModule.forChild(routes);