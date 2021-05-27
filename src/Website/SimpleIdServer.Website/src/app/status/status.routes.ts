import { RouterModule, Routes } from '@angular/router';
import { UnauthorizedComponent } from './components/401/401.component';
import { NotFoundComponent } from './components/404/404.component';

const routes: Routes = [
    { path: '404', component: NotFoundComponent },
    { path: '401', component: UnauthorizedComponent }
];

export const StatusRoute = RouterModule.forChild(routes);