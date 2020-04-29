import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home.component';


const routes: Routes = [
    { path: '', component: HomeComponent }
];

export const HomeRoutes = RouterModule.forChild(routes);