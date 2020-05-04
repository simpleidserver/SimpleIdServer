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

@Component({
    selector: 'view-oauth-client',
    templateUrl: './view.component.html'
})
export class ViewOauthClientsComponent implements OnInit {
    oauthClient: OAuthClient = null;
    updateOAuthClientFormGroup: FormGroup;
    clientNames: Translation[] = [];

    constructor(private store : Store<fromReducers.AppState>, private route: ActivatedRoute) {
        this.updateOAuthClientFormGroup = new FormGroup({
            clientId: new FormControl({
                value: '',
                disabled: true
            })
        });
    }

    ngOnInit(): void {
        this.store.pipe(select(fromReducers.selectOAuthClientResult)).subscribe((oauthClient : OAuthClient) => {
            if (!oauthClient) {
                return;
            }

            this.setOAuthClient(oauthClient);
        });
        this.store.pipe(select(fromReducers.selectOAuthScopesResult)).subscribe((scopes : any) => {
            if (!scopes) {
                return;
            }

            console.log(scopes);
        });
        this.refresh();
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
        this.clientNames = Object.assign([], oauthClient.ClientNames);
    }
}