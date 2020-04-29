import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ListOauthClientsComponent } from './list/list.component';
import { EffectsModule } from '@ngrx/effects';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { StoreModule } from '@ngrx/store';
import * as fromReducers  from './reducers/index';
import { OAuthClientEffects } from './effects/client.effects';
import { OAuthClientRoutes } from './oauthclients.routes';
import { OAuthClientService } from './services/client.service';
import { MaterialModule } from '../../shared/material.module';
import { SharedModule} from '../../shared/shared.module';
import { TranslateEnumPipe } from '../../pipes/translateenum.pipe';
import { AvatarModule } from 'ngx-avatar';

@NgModule({
    imports: [
        AvatarModule,
        OAuthClientRoutes,
        CommonModule,
        SharedModule,
        MaterialModule,
        EffectsModule.forRoot( [ OAuthClientEffects ] ),
        StoreModule.forRoot(fromReducers.appReducer),
        StoreDevtoolsModule.instrument({
            maxAge: 10
        })
    ],
    entryComponents: [],
    declarations: [ ListOauthClientsComponent, TranslateEnumPipe ],
    providers: [ OAuthClientService ]
})

export class OauthClientsModule { }