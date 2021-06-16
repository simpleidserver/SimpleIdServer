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
  {
    path: 'scopes',
    loadChildren: async () => (await import('./scopes/scopes.module')).ScopesModule
  },
  {
    path: 'users',
    loadChildren: async () => (await import('./users/users.module')).UsersModule
  },
  {
    path: 'groups',
    loadChildren: async () => (await import('./groups/groups.module')).GroupsModule
  },
  { path: '**', redirectTo: '/status/404' }
];
