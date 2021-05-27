import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '', redirectTo: 'home', pathMatch: 'full'
  },
  {
    path: 'home',
    loadChildren: async () => (await import('./home/home.module')).HomeModule
  },
  {
    path: 'applications',
    loadChildren: async () => (await import('./applications/applications.module')).ApplicationsModule
  },
  { path: '**', redirectTo: '/status/404' }
];
