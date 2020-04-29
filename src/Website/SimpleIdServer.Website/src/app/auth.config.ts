import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from './../environments/environment';

export const authConfig: AuthConfig = {
    issuer: environment.openIdUrl,
    clientId: 'simpleIdServerWebsite',
    scope: 'openid profile email role',
    redirectUri: environment.redirectUrl,
    requireHttps: false
}