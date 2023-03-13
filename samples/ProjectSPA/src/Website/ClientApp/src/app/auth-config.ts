import { AuthConfig } from 'angular-oauth2-oidc';

export const authCodeFlowConfig: AuthConfig = {
  issuer: 'http://localhost:5001/master',
  redirectUri: window.location.origin,
  silentRefreshRedirectUri: window.location.origin + '/silent-refresh.html',
  clientId: 'protectedSpa',
  responseType: 'code',
  scope: 'openid profile',
  showDebugInformation: true,
  strictDiscoveryDocumentValidation: false,
  sessionChecksEnabled: true,
};
