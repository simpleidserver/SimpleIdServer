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
  selector: 'addscope',
  templateUrl: './add-scope.component.html'
})
export class AddScopeComponent implements OnInit {
  applicationKinds: ApplicationKind[] = [];
  addScopeFormGroup: FormGroup = new FormGroup({
    scopeName: new FormControl('', [Validators.required])
  });

  constructor(
    private translateService: TranslateService,
    private dialogRef: MatDialogRef<AddScopeComponent>) {
  }
  ngOnInit(): void {
  }

  save() {
    if (!this.addScopeFormGroup.valid) {
      return;
    }

    const data = this.addScopeFormGroup.value;
    this.dialogRef.close(data.scopeName);
  }
}
