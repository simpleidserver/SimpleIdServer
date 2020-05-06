import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', loadChildren: './home/home.module#HomeModule' },
    { path: 'clients', loadChildren: './clients/clients.module#ClientsModule' },
    { path: '**', redirectTo: '/status/404' }
];