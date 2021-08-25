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
  selector: 'addrelyingparty',
  templateUrl: './add-relyingparty.component.html',
  styleUrls: ['./add-relyingparty.component.scss']
})
export class AddRelyingPartyComponent {
  applicationKinds: ApplicationKind[] = [];
  addRelyingPartyFormGroup: FormGroup = new FormGroup({
    metadataUrl: new FormControl('', [Validators.required])
  });

  constructor(
    private translateService: TranslateService,
    private dialogRef: MatDialogRef<AddRelyingPartyComponent>) {
  }

  save() {
    if (!this.addRelyingPartyFormGroup.valid) {
      return;
    }

    this.dialogRef.close(this.addRelyingPartyFormGroup.value);
  }
}
