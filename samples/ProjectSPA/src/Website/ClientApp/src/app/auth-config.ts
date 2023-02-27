import { AuthConfig } from 'angular-oauth2-oidc';

export const authCodeFlowConfig: AuthConfig = {
  issuer: 'http://localhost:5001',
  redirectUri: window.location.origin,
  clientId: 'protectedSpa',
  responseType: 'code',
  scope: 'openid profile',
  showDebugInformation: true,
};
