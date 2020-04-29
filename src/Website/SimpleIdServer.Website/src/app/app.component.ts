import { Component, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { authConfig } from './auth.config';
import { Router } from '@angular/router';
import { OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  isConnected: boolean = false;
  name: string = null;
  sessionCheckTimer: any = null;

  constructor(private translate : TranslateService, private oauthService: OAuthService, private route : Router, private router: Router) {
    this.configureAuth();
  }

  disconnect() {
    this.oauthService.logOut();
    this.isConnected = false;
    this.router.navigate(['/home']);
  }

  login() {
    this.oauthService.customQueryParams = {
        'prompt': 'login'
    };
    this.oauthService.initImplicitFlow();
    return false;    
  }

  chooseSession() {    
    this.oauthService.customQueryParams = {
      'prompt': 'select_account'
    };
    this.oauthService.initImplicitFlow();
    return false;
  }

  chooseLanguage(lng: string) {
    this.translate.use(lng);
  }

  ngOnDestroy(): void {
    if (this.sessionCheckTimer) {
      clearInterval(this.sessionCheckTimer);
    }
  }

  ngOnInit() : void {
    var claims: any = this.oauthService.getIdentityClaims();
    if (!claims) {
        this.isConnected = false;;
        return;
    }

    this.name = claims.given_name;
    this.isConnected = true;
  }

  private configureAuth() {
    this.oauthService.configure(authConfig);
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    let self = this;
    this.oauthService.loadDiscoveryDocumentAndTryLogin({
      disableOAuth2StateCheck: true
    });    
    this.sessionCheckTimer = setInterval(function () {
      if (!self.oauthService.hasValidIdToken()) {
          self.oauthService.logOut();
          self.route.navigate(["/"]);
      }            
    }, 3000);
  }
}
