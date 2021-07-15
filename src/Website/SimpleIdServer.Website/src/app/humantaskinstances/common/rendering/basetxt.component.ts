import { Injectable } from "@angular/core";
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from "@angular/forms";
import { MatDialogRef } from "@angular/material/dialog";
import { BaseUIComponent } from "./baseui.component";


export function confirmPasswordValidator(formGroup: FormGroup): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const pwdControl = formGroup.get('pwd');
        if (!pwdControl) {
            return null;
        }

        const badPassword = pwdControl.value !== control.value;
        return badPassword ? { badPassword: true } : null;
    };
}

@Injectable()
export abstract class BaseTxtComponent extends BaseUIComponent {
    subscription: any;
    control: FormControl = new FormControl('');
    constructor() {
        super();
    }

    get errors(): string[] {
        const result: string[] = [];
        for (const key in this.control.errors) {
            result.push(key);
        }

        return result;
    }

    public abstract getType(): string;

    init(): void {
        this.control = new FormControl('', this.buildValidationRules());
        this.form.addControl(this.option.name, this.control);
    }

    ngOnDestroy(): void {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }

    openDialog() {
        if (!this.uiOption.editMode) {
            return;
        }

        const dialogRef = this.open();
        dialogRef.afterClosed().subscribe((r: any) => {
            if (!r) {
                return;
            }

            this.option.name = r.name;
            this.option.validationRules = r.validationRules;
            this.option.translations = r.translations;
            this.control = new FormControl('', this.buildValidationRules());
            this.form.removeControl(this.option.name);
            this.form.addControl(this.option.name, this.control);
        });
    }

    protected abstract open() : MatDialogRef<any>;

    private buildValidationRules(): ValidatorFn[] {
        const result: ValidatorFn[] = [];
        if (this.option.validationRules && this.option.validationRules.length > 0) {
            this.option.validationRules.map((vr: string) => {
                if (vr === "REQUIRED") {
                    result.push(Validators.required);
                }
                else if (vr === "CONFIRMPWD") {
                    result.push(confirmPasswordValidator(this.form));
                }
            });
        }

        return result;
    }
}
