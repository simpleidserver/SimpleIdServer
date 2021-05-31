import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatFormFieldModule } from '@angular/material/form-field';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { environment } from '@envs/environments';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { OAuthModule } from 'angular-oauth2-oidc';
import { AppComponent } from './app.component';
import { routes } from './app.routes';
import { HomeModule } from './home/home.module';
import { MaterialModule } from './shared/material.module';
import { SharedModule } from './shared/shared.module';
import { ApplicationEffects } from './stores/applications/effects/application.effects';
import { ApplicationService } from './stores/applications/services/application.service';
import { appReducer } from './stores/appstate';
import { MetadataEffects } from './stores/metadata/effects/metadata.effects';
import { MetadataService } from './stores/metadata/services/metadata.service';
import { OAuthScopeEffects } from './stores/scopes/effects/scope.effects';
import { OAuthScopeService } from './stores/scopes/services/scope.service';

export function createTranslateLoader(http: HttpClient) {
  let url = environment.baseUrl + 'assets/i18n/';
  return new TranslateHttpLoader(http, url, '.json');
}

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    SharedModule,
    MaterialModule,
    HomeModule,
    MatFormFieldModule,
    FlexLayoutModule,
    BrowserAnimationsModule,
    HttpClientModule,
    OAuthModule.forRoot(),
    EffectsModule.forRoot([ApplicationEffects, OAuthScopeEffects, MetadataEffects]),
    StoreModule.forRoot(appReducer),
    StoreDevtoolsModule.instrument({
      maxAge: 10
    }),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (createTranslateLoader),
        deps: [HttpClient]
      }
    })
  ],
  declarations: [
    AppComponent
  ],
  providers: [ApplicationService, OAuthScopeService, MetadataService],
  bootstrap: [AppComponent]
})
export class AppModule { }
