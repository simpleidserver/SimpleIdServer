import { NgModule } from '@angular/core';
import { UnauthorizedComponent } from './components/401/401.component';
import { NotFoundComponent } from './components/404/404.component';
import { StatusRoute } from './status.routes';

@NgModule({
    imports: [
        StatusRoute
    ],
    declarations: [
        UnauthorizedComponent, NotFoundComponent
    ]
})

export class StatusModule { }
