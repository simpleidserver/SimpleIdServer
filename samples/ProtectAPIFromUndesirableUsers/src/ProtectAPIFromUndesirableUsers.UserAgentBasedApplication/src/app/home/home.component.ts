import { Component, OnInit } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
    constructor(private oauthService: OAuthService) {
    }

    ngOnInit() {
    }

    public login() {
        this.oauthService.initImplicitFlow();        
    }

    public logoff() {
        this.oauthService.logOut();
    }

    public get name() {
        let claims = this.oauthService.getIdentityClaims();
        if (!claims) return null;
        return claims['sub'];
    }
}