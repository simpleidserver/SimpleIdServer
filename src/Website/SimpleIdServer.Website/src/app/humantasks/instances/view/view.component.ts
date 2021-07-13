import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
  selector: 'view-humantaskinstance',
  templateUrl: './view.component.html'
})
export class ViewHumanTaskInstanceComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private oauthService: OAuthService,
    private router : Router) { }

  ngOnInit(): void {
    const claims: any = this.oauthService.getIdentityClaims();
    if (!claims) {
      this.route.queryParams.subscribe(params => {
        const auth = params['auth'];
        if (auth) {
          switch (auth) {
            case 'email':
              this.oauthService.customQueryParams = {
                'acr_values': 'sid-load-021',
                'redirect_url': this.router.url
              };
              break;
          }

          this.oauthService.initLoginFlow();
        }
      });
    }
  }
}
