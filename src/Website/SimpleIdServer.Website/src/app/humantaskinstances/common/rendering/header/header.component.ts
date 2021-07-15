import { Component, Inject } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { BaseUIComponent } from "../baseui.component";

@Component({
  selector: 'view-header',
  templateUrl: 'header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent extends BaseUIComponent {
  constructor(private dialog: MatDialog) {
    super();
  }

  openDialog() {
    const dialogRef = this.dialog.open(HeaderComponentDialog, {
      data: { opt: this.option }
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }
    });
  }
}

@Component({
  selector: 'view-header-dialog',
  templateUrl: 'headerdialog.component.html',
})
export class HeaderComponentDialog {
  configureForm: FormGroup;
  languages: string[] = ['fr', 'en'];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<HeaderComponentDialog>
  ) {
    const self = this;
    this.configureForm = new FormGroup({
      txt: new FormControl({ value: '' }),
      class: new FormControl({ value: '' })
    });
    self.languages.forEach((lng: string) => {
      this.configureForm.addControl('label#' + lng, new FormControl(''));
    });
    this.configureForm.get('class')?.setValue(data.opt.class);
    if (data.opt.translations) {
      data.opt.translations.forEach(function (tr: any) {
        self.configureForm.get('label#' + tr.language)?.setValue(tr.value);
      });
    }
  }

  onSave(val: { class: string }) {
    const translations: any[] = [];
    for (const key in this.configureForm.controls) {
      if (key.startsWith('label')) {
        const language = key.split('#')[1];
        translations.push({
          language: language,
          value: this.configureForm.get(key)?.value
        });
      }
    }

    const opt = this.data.opt;
    opt.translations = translations;
    opt.class = val.class;
    this.dialogRef.close(opt);
  }

  close(evt: any) {
    evt.preventDefault();
    evt.stopPropagation();
    this.dialogRef.close(null);
  }
}
