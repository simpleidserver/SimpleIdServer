import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { authConfig } from './auth.config';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  encapsulation: ViewEncapsulation.None,
  animations: [
    trigger('indicatorRotate', [
      state('collapsed', style({ transform: 'rotate(0deg)' })),
      state('expanded', style({ transform: 'rotate(180deg)' })),
      transition('expanded <=> collapsed',
        animate('225ms cubic-bezier(0.4,0.0,0.2,1)')
      ),
    ])
  ]
})
export class AppComponent implements OnInit, OnDestroy {
  isConnected: boolean = false;
  name: string;
  sessionCheckTimer: any = null;
  showProvisioning: boolean = false;
  showUserManagement: boolean = false;
  showApplications: boolean = false;

  constructor(private translate: TranslateService, private oauthService: OAuthService, private route: Router, private router: Router) {
    translate.setDefaultLang('fr');
    translate.use('fr');
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
    this.oauthService.initLoginFlow();
    return false;
  }

  chooseSession() {
    this.oauthService.customQueryParams = {
      'prompt': 'select_account'
    };
    this.oauthService.initLoginFlow();
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

  ngOnInit(): void {
    this.init();
    this.oauthService.events.subscribe((e: any) => {
      if (e.type === "logout") {
        this.isConnected = false;
      } else if (e.type === "user_profile_loaded") {
        this.init();
      }
    });
  }

  private init(): void {
    var claims: any = this.oauthService.getIdentityClaims();
    if (!claims) {
      this.isConnected = false;;
      return;
    }

    if (this.router.routerState.snapshot.url.startsWith('/provisioning')) {
      this.showProvisioning = true;
    }

    if (this.router.routerState.snapshot.url.startsWith('/users') || this.router.routerState.snapshot.url.startsWith('/groups')) {
      this.showUserManagement = true;
    }


    if (this.router.routerState.snapshot.url.startsWith('/applications')
      || this.router.routerState.snapshot.url.startsWith('/scopes')
      || this.router.routerState.snapshot.url.startsWith('/relyingparties')) {
      this.showApplications = true;
    }

    this.name = claims['given_name'];
    this.isConnected = true;
  }

  private configureAuth() {
    this.oauthService.configure(authConfig);
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    let self = this;
    this.oauthService.loadDiscoveryDocumentAndTryLogin({
      disableOAuth2StateCheck: true
    }).then(() => {
      this.oauthService.loadUserProfile();
    });
    this.sessionCheckTimer = setInterval(function () {
      if (!self.oauthService.hasValidIdToken()) {
        self.oauthService.logOut();
        self.route.navigate(["/"]);
      }
    }, 3000);
  }
}
