"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.authCodeFlowConfig = void 0;
exports.authCodeFlowConfig = {
    issuer: 'http://localhost:5000',
    redirectUri: window.location.origin + '/index.html',
    clientId: 'website',
    responseType: 'code',
    scope: 'openid profile email role',
    showDebugInformation: true,
};
//# sourceMappingURL=auth-config.js.map