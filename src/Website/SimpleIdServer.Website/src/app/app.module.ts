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
import { OAuthModule, OAuthStorage } from 'angular-oauth2-oidc';
import { MultiTranslateHttpLoader } from "ngx-translate-multi-http-loader";
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
import { translationFactory } from './translation.util';

export function createTranslateLoader(http: HttpClient) {
  let url = environment.baseUrl + 'assets/i18n/';
  return new TranslateHttpLoader(http, url, '.json');
}


export function HttpLoaderFactory(http: HttpClient) {
  let url = environment.baseUrl + 'assets/i18n/';
  return new MultiTranslateHttpLoader(http, [
    { prefix: "./assets/translate/core/", suffix: ".json" },
    { prefix: "./assets/translate/shared/", suffix: ".json" },
  ]);
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
        useFactory: translationFactory,
        deps: [HttpClient]
      }
    })
  ],
  declarations: [
    AppComponent
  ],
  providers: [
    ApplicationService,
    OAuthScopeService,
    MetadataService,
    {
      provide: OAuthStorage,
      useValue: sessionStorage
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
