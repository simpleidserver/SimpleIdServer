import { Component, ViewChild } from '@angular/core';
import { OnInit } from '@angular/core';
import { select, Store, createSelector } from '@ngrx/store';
import { StartGet } from '../../../stores/clients/oauth/actions/client.actions';
import * as fromScopeActions from '../../../stores/scopes/oauth/actions/scope.actions';
import { OAuthClient } from '../../../stores/clients/oauth/models/oauthclient.model';
import * as fromReducers from '../../../stores/appstate';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl } from '@angular/forms';
import { Translation } from '../../../common/translation';
import { OAuthScope } from '../../../stores/scopes/oauth/models/oauthscope.model';
import { OAuthClientSecret } from '../../../stores/clients/oauth/models/oauthclientsecret.model';

@Component({
    selector: 'view-oauth-client',
    templateUrl: './view.component.html'
})
export class ViewOauthClientsComponent implements OnInit {
    oauthClient: OAuthClient = null;
    updateOAuthClientFormGroup: FormGroup;
    clientNames: Array<Translation> = [];
    contacts: Array<string> = [];
    policyUris: Array<Translation> = [];
    clientUris: Array<Translation> = [];
    logoUris: Array<Translation> = [];
    tosUris: Array<Translation> = [];
    clientScopes : Array<string> = [];
    oauthScopes : Array<string> = [];
    redirectUris: Array<string> = [];
    oauthGrantTypes : Array<string> = [];
    clientGrantTypes : Array<string> = [];
    oauthResponseTypes : Array<string> = [];
    clientResponseTypes : Array<string> = [];
    oauthTokenProfiles : Array<string> = [];
    oauthAuthMethods : Array<string> = [];
    clientSecrets: Array<OAuthClientSecret> [];
    oauthTokenSignedResponseAlgs : Array<string> = [];
    oauthTokenEncryptedResponseAlgs : Array<string> = [];
    oauthTokenEncryptedResponseEncs : Array<string> = [];

    constructor(private store : Store<fromReducers.AppState>, private route: ActivatedRoute) {
        this.updateOAuthClientFormGroup = new FormGroup({
            clientId: new FormControl({
                value: '',
                disabled: true
            }),
            softwareId: new FormControl(''),
            softwareVersion: new FormControl(''),
            preferredTokenProfile : new FormControl(''),
            tokenEndPointAuthMethod: new FormControl(''),
            tokenSignedResponseAlg : new FormControl(''),
            tokenEncryptedResponseAlg: new FormControl(''),
            tokenEncryptedResponseEnc: new FormControl(''),
            refreshTokenExpirationTimeInSeconds: new FormControl(''),
            tokenExpirationTimeInSeconds: new FormControl('')
        });
        this.oauthGrantTypes = [
            "client_credentials",
            "refresh_token",
            "password",
            "authorization_code"
        ];
        this.oauthResponseTypes = [
            "code",
            "token"
        ];
        this.oauthTokenProfiles = [
            "Bearer",
            "mac"
        ];
        this.oauthAuthMethods = [
            "private_key_jwt",
            "client_secret_basic",
            "client_secret_jwt",
            "client_secret_post",
            "pkce"
        ];
        this.oauthTokenSignedResponseAlgs = [
            "RS256",
            "ES256"
        ];
        this.oauthTokenEncryptedResponseAlgs = [
            "RSA1_5"
        ];
        this.oauthTokenEncryptedResponseEncs = [
            "A128CBC-HS256"
        ];
    }

    ngOnInit(): void {
        this.store.pipe(select(fromReducers.selectOAuthClientResult)).subscribe((oauthClient : OAuthClient) => {
            if (!oauthClient) {
                return;
            }

            this.setOAuthClient(oauthClient);
        });
        this.store.pipe(select(fromReducers.selectOAuthScopesResult)).subscribe((scopes : OAuthScope[]) => {
            if (!scopes) {
                return;
            }

            this.oauthScopes = scopes.map(s => s.Name);
        });
        this.refresh();
    }    

    deleteClientSecret(index : number) {
        this.clientSecrets.splice(index, 1);
    }

    refresh() {
        var id = this.route.snapshot.params['id'];
        let getClient = new StartGet(id);
        let getScopes = new fromScopeActions.StartGetAll();
        this.store.dispatch(getClient);
        this.store.dispatch(getScopes);
    }

    private setOAuthClient(oauthClient : OAuthClient) {
        this.updateOAuthClientFormGroup.controls["clientId"].setValue(oauthClient.ClientId);
        this.updateOAuthClientFormGroup.controls["softwareId"].setValue(oauthClient.SoftwareId);
        this.updateOAuthClientFormGroup.controls["softwareVersion"].setValue(oauthClient.SoftwareVersion);
        this.updateOAuthClientFormGroup.controls["preferredTokenProfile"].setValue(oauthClient.PreferredTokenProfile);
        this.updateOAuthClientFormGroup.controls["tokenEndPointAuthMethod"].setValue(oauthClient.TokenEndPointAuthMethod);
        this.updateOAuthClientFormGroup.controls["tokenSignedResponseAlg"].setValue(oauthClient.TokenSignedResponseAlg);
        this.updateOAuthClientFormGroup.controls["tokenEncryptedResponseAlg"].setValue(oauthClient.TokenEncryptedResponseAlg);
        this.updateOAuthClientFormGroup.controls["tokenEncryptedResponseEnc"].setValue(oauthClient.TokenEncryptedResponseEnc);
        this.updateOAuthClientFormGroup.controls["refreshTokenExpirationTimeInSeconds"].setValue(oauthClient.RefreshTokenExpirationTimeInSeconds);
        this.updateOAuthClientFormGroup.controls["tokenExpirationTimeInSeconds"].setValue(oauthClient.TokenExpirationTimeInSeconds);
        this.clientNames = Object.assign([], oauthClient.ClientNames);
        this.contacts = Object.assign([], oauthClient.Contacts);
        this.policyUris = Object.assign([], oauthClient.PolicyUris);
        this.clientUris = Object.assign([], oauthClient.ClientUris);
        this.logoUris = Object.assign([], oauthClient.LogoUris);
        this.tosUris = Object.assign([], oauthClient.TosUris);
        this.clientScopes = Object.assign([], oauthClient.Scopes);
        this.redirectUris = Object.assign([], oauthClient.RedirectUris);
        this.clientGrantTypes = Object.assign([], oauthClient.GrantTypes);
        this.clientResponseTypes = Object.assign([], oauthClient.ResponseTypes);
        this.clientSecrets = Object.assign([], oauthClient.Secrets);
    }
}