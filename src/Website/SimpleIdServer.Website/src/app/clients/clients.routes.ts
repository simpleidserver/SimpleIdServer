import { RouterModule, Routes } from '@angular/router';


const routes: Routes = [
    { path: '', redirectTo: 'oauth', pathMatch: 'full' },
    { path: 'oauth', loadChildren: './oauth/oauthclients.module#OauthClientsModule' },
];

export const ClientRoutes = RouterModule.forChild(routes);