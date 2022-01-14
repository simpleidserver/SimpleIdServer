import { AuthConfig } from 'angular-oauth2-oidc';

export const authCodeFlowConfig: AuthConfig = {
  issuer: 'https://localhost:5001',
  redirectUri: window.location.origin,
  clientId: 'website',
  responseType: 'code',
  scope: 'openid profile email role',
  showDebugInformation: true,
};
