import { RouterModule, Routes } from '@angular/router';


const routes: Routes = [
  {
    path: ':id',
    children: [
      {
        path: 'instances',
        loadChildren: async () => (await import('./instances/humantaskinstances.module')).HumanTaskInstancesModule
      }
    ]
  }
];

export const HumanTasksRoutes = RouterModule.forChild(routes);
