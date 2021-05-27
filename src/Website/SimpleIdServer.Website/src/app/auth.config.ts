import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '@envs/environments';

export const authConfig: AuthConfig = {
  issuer: environment.openidUrl,
  clientId: 'simpleIdServerWebsite',
  scope: 'openid profile email role',
  redirectUri: environment.redirectUrl,
  responseType: 'code',
  requireHttps: false,
  showDebugInformation: true
}
