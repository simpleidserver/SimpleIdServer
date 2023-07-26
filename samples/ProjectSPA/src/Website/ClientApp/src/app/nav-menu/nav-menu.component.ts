import { Component, OnInit } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { filter } from 'rxjs';
import { authCodeFlowConfig } from '../auth-config';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  isConnected: boolean = false;
  name: string = "";

  constructor(private oauthService: OAuthService) {
    this.oauthService.setupAutomaticSilentRefresh();
    this.oauthService.configure(authCodeFlowConfig);
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
    var claims: any = this.oauthService.getIdentityClaims();
    if (!claims) {
      return;
    }

    this.isConnected = true;
    this.name = claims["sub"];
  }

  ngOnInit() {
    this.oauthService.events.pipe(filter(e => e.type === 'token_received')).subscribe(e => {
      var claims: any = this.oauthService.getIdentityClaims();
      this.isConnected = true;
      this.name = claims["sub"];
    });
    this.oauthService.events.pipe(filter(e => e.type === 'session_terminated')).subscribe(e => {
      this.isConnected = false;
      this.name = "";
    });
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
