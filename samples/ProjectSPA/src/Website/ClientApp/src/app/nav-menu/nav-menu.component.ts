import { Component } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from '../auth-config';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
  isConnected: boolean = false;
  name: string = "";

  constructor(private oauthService: OAuthService) {
    this.oauthService.configure(authCodeFlowConfig);
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
    var claims: any = this.oauthService.getIdentityClaims();
    if (!claims) {
      return;
    }

    this.isConnected = true;
    this.name = claims["sub"];
  }

  login(evt: any) {
    evt.preventDefault();
    this.oauthService.initCodeFlow();
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
