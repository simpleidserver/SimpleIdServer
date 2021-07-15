import { Component, Inject } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { BaseUIComponent } from "../baseui.component";

@Component({
    selector: 'view-submitbtn',
    templateUrl: 'submitbtn.component.html',
    styleUrls: ['./submitbtn.component.scss']
})
export class SubmitBtnComponent extends BaseUIComponent {
    constructor(private dialog: MatDialog) {
        super();
    }

    openDialog() {
        const dialogRef = this.dialog.open(SubmitBtnComponentDialog, {
            data: { opt: this.option }
        });
        dialogRef.afterClosed().subscribe((r: any) => {
            if (!r) {
                return;
            }

            this.option.name = r.name;
            this.option.translations = r.translations;
        });
    }
}

@Component({
    selector: 'view-select-dialog',
    templateUrl: 'submitbtndialog.component.html',
})
export class SubmitBtnComponentDialog {
    languages: string[] = ['fr', 'en'];
    configureForm: FormGroup;
    validationRules: string[] = [];
    selectedValidationRules: string[] = [];

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: any,
        public dialogRef: MatDialogRef<SubmitBtnComponentDialog>) {
        const self = this;
        self.configureForm = new FormGroup({
            name: new FormControl('')
        });
        self.languages.forEach((lng: string) => {
            this.configureForm.addControl('label#' + lng, new FormControl(''));
        });
        self.configureForm.get('name')?.setValue(data.opt.name);
        if (data.opt.translations) {
            data.opt.translations.forEach(function (tr: any) {
                self.configureForm.get('label#' + tr.language)?.setValue(tr.value);
            });
        }
    }

    onSave(val: { name: string }) {
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

        const newOpt = {
            name: val.name,
            translations: translations
        };
        this.dialogRef.close(newOpt);
    }

    close(evt: any) {
        evt.preventDefault();
        evt.stopPropagation();
        this.dialogRef.close(null);
    }
}
