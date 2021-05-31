import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { startGet } from '@app/stores/applications/actions/applications.actions';
import { Application } from '@app/stores/applications/models/application.model';
import * as fromReducers from '@app/stores/appstate';
import { startGetLanguages } from '@app/stores/metadata/actions/metadata.actions';
import { select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { Translation } from '../../common/translation';
import { EditTranslationComponent } from './edit-translation.component';

@Component({
  selector: 'view-application',
  templateUrl: './view.component.html'
})
export class ViewApplicationsComponent implements OnInit {
  clientId: string | null = null;
  clientNames: Translation[] = [];
  clientLogoUris: Translation[] = [];
  application: Application;
  clientSecretInputType: string = "password";
  updateApplicationForm: FormGroup = new FormGroup({
    clientName: new FormControl({ value: '', disabled: true }),
    clientId: new FormControl({ value: '', disabled: true }),
    clientSecret: new FormControl({ value: '', disabled: true }),
    logoUri: new FormControl({ value: '', disabled: true })
  });

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
      this.application = application;
      this.refreshApplication();
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
    this.store.dispatch(getClient);
    this.store.dispatch(getLanguages);
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

  private clone(translations: Translation[]) {
    return translations.map(x => Object.assign({}, x));
  }

  private refreshApplication() {
    this.updateApplicationForm.get('clientName')?.setValue(this.translateApplicationProperty(this.clientNames));
    this.updateApplicationForm.get('clientId')?.setValue(this.application.ClientId);
    this.updateApplicationForm.get('clientSecret')?.setValue(this.application.ClientSecret);
    this.updateApplicationForm.get('logoUri')?.setValue(this.translateApplicationProperty(this.clientLogoUris));
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
