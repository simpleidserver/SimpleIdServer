import { Inject, Injectable } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Injectable()
export abstract class BaseTxtComponentDialog {
    languages: string[] = ['fr', 'en'];
    configureForm: FormGroup;
    validationRules: string[] = [];
    selectedValidationRules: string[] = [];
    constructor(
        @Inject(MAT_DIALOG_DATA) public data: any,
        public dialogRef: MatDialogRef<BaseTxtComponentDialog>
    ) {
        const self = this;
        self.configureForm = new FormGroup({
            name: new FormControl(''),
            validationRule: new FormControl('')
        });
        self.languages.forEach((lng: string) => {
            self.configureForm.addControl('label#' + lng, new FormControl(''));
        });
        self.configureForm.get('name')?.setValue(data.opt.name);
        if (data.opt.translations) {
            data.opt.translations.forEach(function (tr: any) {
                self.configureForm.get('label#' + tr.language)?.setValue(tr.value);
            });
        }

        this.allValidationRules().forEach((v: string) => {
            if (data.opt.validationRules.includes(v)) {
                this.selectedValidationRules.push(v);
            } else {
                this.validationRules.push(v);
            }
        });
    }

    protected abstract allValidationRules(): string[];

    onSave(val: { name: string, type: string, validationRule: string }) {
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
            type: val.type,
            validationRules: this.selectedValidationRules,
            translations: translations
        };        
        this.dialogRef.close(newOpt);
    }

    addValidationRule(evt: any) {
        evt.preventDefault();
        const validationRule = this.configureForm.get('validationRule')?.value;
        const index = this.validationRules.indexOf(validationRule);
        this.configureForm.get('validationRule')?.setValue('');
        this.validationRules.splice(index, 1);
        this.selectedValidationRules.push(validationRule);
    }

    remove(validationRule: string, index: number) {
        this.selectedValidationRules.splice(index, 1);
        this.validationRules.push(validationRule);
    }

    close(evt: any) {
        evt.preventDefault();
        evt.stopPropagation();
        this.dialogRef.close(null);
    }
}
