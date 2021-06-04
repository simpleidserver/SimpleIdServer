import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { startGet } from '@app/stores/applications/actions/applications.actions';
import { Application } from '@app/stores/applications/models/application.model';
import * as fromReducers from '@app/stores/appstate';
import { startGetLanguages, startGetWellKnownConfiguration } from '@app/stores/metadata/actions/metadata.actions';
import { select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { Translation } from '../../common/translation';
import { EditTranslationComponent } from './edit-translation.component';

class GrantTypeResult {
  constructor(public name: string, public isSelected: boolean) { }
}

@Component({
  selector: 'view-application',
  templateUrl: './view.component.html'
})
export class ViewApplicationsComponent implements OnInit {
  clientId: string | null = null;
  clientNames: Translation[] = [];
  clientLogoUris: Translation[] = [];
  redirectUris: string[];
  logoutUris: string[];
  grantTypes: GrantTypeResult[] = [];
  application: Application;
  conf: any;
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
    private dialog: MatDialog) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectApplicationResult)).subscribe((application: Application | null) => {
      if (!application) {
        return;
      }

      this.clientNames = this.clone(application.ClientNames);
      this.clientLogoUris = this.clone(application.LogoUris);
      this.redirectUris = [...application.RedirectUris];
      this.logoutUris = [...application.PostLogoutRedirectUris]
      this.application = application;
      this.refreshApplication();
    });
    this.store.pipe(select(fromReducers.selectWellKnownConfigurationResult)).subscribe((conf: any | null) => {
      if (!conf || !conf['grant_types_supported']) {
        return;
      }

      this.conf = conf;
      this.refreshMetadata();
    });
    this.translateService.onLangChange.subscribe(() => {
      this.refreshApplication();
    });
    this.refresh();
  }

  refresh() {
    var id = this.route.snapshot.params['id'];
    let getClient = startGet({ id: id });
    let getLanguages = startGetLanguages();
    let getWellKnownConfiguration = startGetWellKnownConfiguration();
    this.store.dispatch(getClient);
    this.store.dispatch(getLanguages);
    this.store.dispatch(getWellKnownConfiguration);
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
    const index = this.redirectUris.indexOf(redirectUri, 0);
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

  grantTypeChanged(evt: MatCheckboxChange, grantType: GrantTypeResult) {
    grantType.isSelected = evt.checked;
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
  }

  private refreshMetadata() {
    this.grantTypes = [];
    this.conf['grant_types_supported'].forEach((v: string) => {
      const isSelected = this.application.GrantTypes.indexOf(v) > -1;
      this.grantTypes.push(new GrantTypeResult(v, isSelected));
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
}
