import { AuthConfig } from 'angular-oauth2-oidc';

export const authCodeFlowConfig: AuthConfig = {
  issuer: 'http://localhost:5000',
  redirectUri: window.location.origin,
  clientId: 'website',
  responseType: 'code',
  scope: 'openid profile email role',
  showDebugInformation: true,
};
