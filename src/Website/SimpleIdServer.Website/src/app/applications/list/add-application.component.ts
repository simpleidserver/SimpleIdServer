import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Translation } from '@app/common/translation';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

interface ApplicationKind {
  key: string;
  translations: Translation[];
  isSelected: boolean;
}

@Component({
  selector: 'addapplication',
  templateUrl: './add-application.component.html',
  styleUrls: ['./add-application.component.scss']
})
export class AddApplicationComponent implements OnInit {
  applicationKinds: ApplicationKind[] = [];
  addApplicationFormGroup: FormGroup = new FormGroup({
    clientName: new FormControl('', [Validators.required]),
    applicationKind: new FormControl('', [Validators.required])
  });

  constructor(
    private translateService: TranslateService,
    private dialogRef: MatDialogRef<AddApplicationComponent>) {
  }
  ngOnInit(): void {
    const children : any = this.translateService.instant('metadata.content.applicationKind.children');
    for (var record in children) {
      this.applicationKinds.push({
        key: record, isSelected: false, translations: children[record].translations.map((r: any) => {
          var record: Translation = { Language: r.languageCode, Value: r.value };
          return record;
        })
      });
    }
  }

  toggle(applicationKind: ApplicationKind) {
    this.applicationKinds.forEach((ap) => ap.isSelected = false);
    applicationKind.isSelected = true;
    this.addApplicationFormGroup.get('applicationKind')?.setValue(applicationKind.key);
  }

  save() {
    if (!this.addApplicationFormGroup.valid) {
      return;
    }

    const data = this.addApplicationFormGroup.value;
    data.applicationKind = parseInt(data.applicationKind);
    this.applicationKinds.forEach((ap) => ap.isSelected = false);
    this.dialogRef.close(this.addApplicationFormGroup.value);
  }
}
