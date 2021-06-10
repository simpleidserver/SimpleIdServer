import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { SelectionResult } from '@app/common/components/multiselector/multiselector.component';
import { Translation } from '@app/common/translation';
import { startDelete, startGet, startUpdate } from '@app/stores/applications/actions/applications.actions';
import { Application } from '@app/stores/applications/models/application.model';
import * as fromReducers from '@app/stores/appstate';
import { startGetLanguages, startGetWellKnownConfiguration } from '@app/stores/metadata/actions/metadata.actions';
import { startGetAll } from '@app/stores/scopes/actions/scope.actions';
import { OAuthScope } from '@app/stores/scopes/models/oauthscope.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';
import { DisplayJwkComponent } from './displayjwk.component';
import { EditTranslationComponent } from './edit-translation.component';

@Component({
  selector: 'view-application',
  templateUrl: './view.component.html'
})
export class ViewApplicationsComponent implements OnInit {
  displayedJwkColumns: string[] = ['kty', 'alg', 'use'];
  clientId: string | null = null;
  clientNames: Translation[] = [];
  clientLogoUris: Translation[] = [];
  redirectUris: string[];
  logoutUris: string[];
  jwks: any[];
  grantTypes: SelectionResult[] = [];
  scopes: SelectionResult[] = [];
  receivedScopes: OAuthScope[] = [];
  application: Application;
  isApplicationLoaded: boolean = false;
  isScopesLoaded: boolean = false;
  isWellKnownConfigurationLoaded: boolean = false;
  conf: any;
  isLoading: boolean;
  clientSecretInputType: string = "password";
  updateApplicationForm: FormGroup = new FormGroup({
    clientName: new FormControl({ value: '', disabled: true }),
    clientId: new FormControl({ value: '', disabled: true }),
    clientSecret: new FormControl({ value: '', disabled: true }),
    logoUri: new FormControl({ value: '', disabled: true }),
    applicationKind: new FormControl({ value: '' }),
    tokenExpirationTime: new FormControl({ value: '' }),
    tokenAuthMethod: new FormControl({ value: '' }),
    refreshTokenExpirationTime: new FormControl({ value: '' })
  });
  get applicationKind() : number {
    return parseInt(this.updateApplicationForm.get('applicationKind')?.value);
  }

  constructor(
    private store: Store<fromReducers.AppState>,
    private route: ActivatedRoute,
    private translateService: TranslateService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar,
    private actions$: ScannedActionsSubject,
    private router : Router) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.isScopesLoaded = false;
    this.isApplicationLoaded = false;
    this.isWellKnownConfigurationLoaded = false;
    this.store.pipe(select(fromReducers.selectApplicationResult)).subscribe((application: Application | null) => {
      if (!application) {
        return;
      }

      this.isApplicationLoaded = true;
      this.clientNames = this.clone(application.ClientNames);
      this.clientLogoUris = this.clone(application.LogoUris);
      this.redirectUris = [...application.RedirectUris];
      this.logoutUris = [...application.PostLogoutRedirectUris];
      this.jwks = application.Jwks;
      this.application = application;
      this.refreshApplication();
      this.refreshScopes();
      this.refreshMetadata();
      this.setIsLoading();
    });
    this.store.pipe(select(fromReducers.selectOAuthScopesResult)).subscribe((scopes: OAuthScope[] | null) => {
      if (!scopes) {
        return;
      }

      this.isScopesLoaded = true;
      this.receivedScopes = scopes;
      this.refreshScopes();
      this.setIsLoading();
    });
    this.store.pipe(select(fromReducers.selectWellKnownConfigurationResult)).subscribe((conf: any | null) => {
      if (!conf || !conf['grant_types_supported']) {
        return;
      }

      this.isWellKnownConfigurationLoaded = true;
      this.conf = conf;
      this.refreshMetadata();
      this.setIsLoading();
    });
    this.translateService.onLangChange.subscribe(() => {
      this.refreshApplication();
    });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] COMPLETE_UPDATE_APPLICATON'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('applications.messages.update'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] ERROR_UPDATE_APPLICATION'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('applications.messages.errorUpdate'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] COMPLETE_DELETE_APPLICATION'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('applications.messages.delete'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.router.navigate(['/applications']);
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] ERROR_DELETE_APPLICATION'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('applications.messages.errorDelete'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.refresh();
  }

  refresh() {
    var id = this.route.snapshot.params['id'];
    let getClient = startGet({ id: id });
    let getLanguages = startGetLanguages();
    let getWellKnownConfiguration = startGetWellKnownConfiguration();
    let getOAuthScopes = startGetAll();
    this.store.dispatch(getClient);
    this.store.dispatch(getLanguages);
    this.store.dispatch(getWellKnownConfiguration);
    this.store.dispatch(getOAuthScopes);
  }

  displayClientSecret() {
    this.clientSecretInputType = "text";
  }

  hideClientSecret() {
    this.clientSecretInputType = "password";
  }

  getClientSecret() {
    return this.updateApplicationForm.get('clientSecret')?.value;
  }

  edit(translations: Translation[], title: string) {
    const dialogRef = this.dialog.open(EditTranslationComponent, {
      data: {
        translations: translations,
        title: title
      },
      width: '500px'
    });
    dialogRef.afterClosed().subscribe(res => {
      if (!res) {
        return;
      }

      for (const lng in res) {
        const filtered = translations.filter((cn) => cn.Language === lng);
        if (filtered.length === 1) {
          filtered[0].Value = res[lng];
        } else {
          translations.push({ Language : lng, Value : res[lng] });
        }
      }

      this.refreshApplication();
    });
  }

  translate(key: string) {
    return this.translateService.instant(key);
  }

  onApplicationKindChanged(evt: any) {
    this.updateApplicationForm.get('applicationKind')?.setValue(evt);
    this.updateApplicationForm.get('tokenAuthMethod')?.setValue('');
  }

  removeRedirectUri(redirectUri: string) {
    const index = this.redirectUris.indexOf(redirectUri, 0);
    if (index > -1) {
      this.redirectUris.splice(index, 1);
    }
  }

  addRedirectUri(evt: MatChipInputEvent) {
    const value = (evt.value || '').trim();
    if (value) {
      this.redirectUris.push(value);
    }

    evt.input.value = "";
  }

  removeLogoutUri(redirectUri: string) {
    const index = this.logoutUris.indexOf(redirectUri, 0);
    if (index > -1) {
      this.logoutUris.splice(index, 1);
    }
  }

  addLogoutUri(evt: MatChipInputEvent) {
    const value = (evt.value || '').trim();
    if (value) {
      this.logoutUris.push(value);
    }

    evt.input.value = "";
  }

  displayGrantType(selection: SelectionResult) {
    return this.translateService.instant('applications.grantType.' + selection.name);
  }

  displayJwk(evt: any, jwk: any) {
    evt.preventDefault();
    this.dialog.open(DisplayJwkComponent, {
      data: {
        jwk: jwk
      },
      width: '500px'
    });
  }

  saveApplication(evt : any, formValue : any) {
    evt.preventDefault();
    const request : any = {
      token_endpoint_auth_method: formValue.tokenAuthMethod,
      token_expiration_time_seconds: formValue.tokenExpirationTime,
      refresh_token_expiration_time_seconds: formValue.refreshTokenExpirationTime,
      scope: this.scopes.filter(s => s.isSelected).map(s => s.name).join(' '),
      redirect_uris: this.redirectUris,
      post_logout_redirect_uris: this.logoutUris,
      grant_types: this.grantTypes.filter(g => g.isSelected).map(g => g.name),
      application_kind: formValue.applicationKind
    };
    this.enrichTranslations(request, 'client_name', this.clientNames);
    this.enrichTranslations(request, 'logo_uri', this.clientLogoUris);
    let update = startUpdate({ id: this.application.ClientId, request });
    this.store.dispatch(update);
  }

  delete() {
    let deleteApplication = startDelete({ id: this.application.ClientId });
    this.store.dispatch(deleteApplication);
  }

  private clone(translations: Translation[]) {
    return translations.map(x => Object.assign({}, x));
  }

  private refreshApplication() {
    this.updateApplicationForm.get('clientName')?.setValue(this.translateApplicationProperty(this.clientNames));
    this.updateApplicationForm.get('clientId')?.setValue(this.application.ClientId);
    this.updateApplicationForm.get('clientSecret')?.setValue(this.application.ClientSecret);
    this.updateApplicationForm.get('logoUri')?.setValue(this.translateApplicationProperty(this.clientLogoUris));
    this.updateApplicationForm.get('tokenExpirationTime')?.setValue(this.application.TokenExpirationTimeInSeconds);
    this.updateApplicationForm.get('refreshTokenExpirationTime')?.setValue(this.application.RefreshTokenExpirationTimeInSeconds);
    this.updateApplicationForm.get('applicationKind')?.setValue(this.application.ApplicationKind);
    this.updateApplicationForm.get('tokenAuthMethod')?.setValue(this.application.TokenEndPointAuthMethod);
  }

  private refreshMetadata() {
    this.grantTypes = [];
    if (!this.conf || !this.application) {
      return;
    }

    this.conf['grant_types_supported'].forEach((v: string) => {
      const isSelected = this.application.GrantTypes.indexOf(v) > -1;
      const record: SelectionResult = { isSelected : isSelected, name: v, value: v };
      this.grantTypes.push(record);
    });
  }

  private refreshScopes() {
    if (!this.application || !this.receivedScopes) {
      return;
    }

    this.scopes = [];
    this.receivedScopes.forEach((s: OAuthScope) => {
      const isSelected = this.application.Scopes.indexOf(s.Name) > -1;
      const record: SelectionResult = { isSelected: isSelected, name: s.Name, value: s };
      record.isSelected = isSelected;
      this.scopes.push(record);
    });
  }

  private translateApplicationProperty(translations: Translation[]) : string {
    const defaultLang = this.translateService.currentLang;
    const filtered = translations.filter((t) => t.Language === defaultLang);
    if (filtered.length === 1) {
      return filtered[0].Value;
    }

    return "";
  }

  private enrichTranslations(request: any, key: string, translations: Translation[]) {
    translations.forEach((tr: Translation) => {
      request[key + "#" + tr.Language] = tr.Value;
    });
  }

  private setIsLoading() {
    this.isLoading = !(this.isApplicationLoaded && this.isScopesLoaded && this.isWellKnownConfigurationLoaded);
  }
}
