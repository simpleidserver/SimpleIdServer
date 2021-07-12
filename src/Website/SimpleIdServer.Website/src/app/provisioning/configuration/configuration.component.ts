import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { startGet, startUpdate } from '@app/stores/provisioning/actions/provisioning.actions';
import { ProvisioningConfiguration } from '@app/stores/provisioning/models/provisioningconfiguration.model';
import { ProvisioningConfigurationRecord } from '@app/stores/provisioning/models/provisioningconfigurationrecord.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'provisioning-configurations',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.scss']
})
export class ProvisioningConfigurationComponent implements OnInit, OnDestroy {
  isLoading: boolean;
  subscription: any;
  provisioningConfigurations$: ProvisioningConfiguration | null = null;
  updateProvisioningConfigurationForm: FormGroup = new FormGroup({
    type: new FormControl({ value: '', disabled: true }),
    resourceType: new FormControl({ value: '', disabled: true }),
    parameters: new FormGroup({})
  });

  constructor(
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService,
    private actions$: ScannedActionsSubject,
    private snackbar: MatSnackBar,
    private store: Store<fromReducers.AppState>) { }

  ngOnInit(): void {
    this.provisioningConfigurations$ = null;
    this.isLoading = true;
    this.actions$.pipe(
      filter((action: any) => action.type === '[ProvisioningConfigurationHistories] COMPLETE_UPDATE'))
      .subscribe(() => {
        this.isLoading = false;
        this.snackbar.open(this.translateService.instant('provisioning.configuration.messages.update'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[ProvisioningConfigurationHistories] ERROR_UPDATE'))
      .subscribe(() => {
        this.isLoading = false;
        this.snackbar.open(this.translateService.instant('provisioning.configuration.messages.errorUpdate'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.subscription = this.store.pipe(select(fromReducers.selectProvisioningConfigurationResult)).subscribe((state: ProvisioningConfiguration| null) => {
      if (state) {
        this.isLoading = false;
        this.provisioningConfigurations$ = state;
        this.init();
      }
    });
  }

  get parameters() : string[] {
    const parameters = this.updateProvisioningConfigurationForm.get('parameters') as FormGroup;
    return Object.keys(parameters.controls);
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  ngAfterViewInit() {
    this.refresh();
  }

  updateProvisioningConfiguration(form: any) {
    this.isLoading = true;
    const records = this.buildConfigurationRecord(form.parameters);
    const id = this.activatedRoute.snapshot.params['id'];
    const request = startUpdate({ id: id, records: records });
    this.store.dispatch(request);
  }

  refresh() {
    this.isLoading = true;
    const id = this.activatedRoute.snapshot.params['id'];
    const request = startGet({ id: id });
    this.store.dispatch(request);
  }

  private init() {
    this.updateProvisioningConfigurationForm.get('type')?.setValue(this.provisioningConfigurations$?.type.toString());
    this.updateProvisioningConfigurationForm.get('resourceType')?.setValue(this.provisioningConfigurations$?.resourceType);
    if (this.provisioningConfigurations$?.records) {
      this.populateParameters(this.provisioningConfigurations$.records, null);
    }
  }

  private populateParameters(records: ProvisioningConfigurationRecord[], parent: string | null) {
    const parameters = this.updateProvisioningConfigurationForm.get('parameters') as FormGroup;
    records.forEach((v: ProvisioningConfigurationRecord) => {
      if (v.type === 1) {
        if (!parent) {
          parent = v.name;
        } else {
          parent = parent + ' \\ ' + v.name;
        }

        this.populateParameters(v.values, parent);
      } else {
        let name = v.name;
        if (parent) {
          name = parent + ' \\ ' + name;
        }

        const newFormControl = new FormControl({ value: v.valuesString[0] });
        newFormControl.setValue(v.valuesString[0]);
        parameters.addControl(name, newFormControl);
      }
    });
  }

  private buildConfigurationRecord(form: any) : ProvisioningConfigurationRecord[] {
    var result: ProvisioningConfigurationRecord[] = [];
    var records = result;
    Object.keys(form).forEach((k: any) => {
      const splitted = k.split(' \\ ');
      var record: ProvisioningConfigurationRecord;
      splitted.forEach((str: string, index: number) => {
        var records = result;
        if (index > 0) {
          records = record.values;
        }
        const filtered = records.filter((r: ProvisioningConfigurationRecord) => r.name === str);
        if (filtered.length === 1) {
          record = filtered[0];
        } else {
          record = new ProvisioningConfigurationRecord();
          record.name = str;
          record.type = index < splitted.length - 1 ? 1 : 0;
          record.isArray = index < splitted.length - 1;
          if (!record.isArray) {
            record.valuesString = [form[k]];
          }

          records.push(record);
        }
      });
    });

    return result;
  }

  private getConfigurationRecord(records: ProvisioningConfigurationRecord[]) {

  }
}
