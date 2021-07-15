import { Component, Inject, OnDestroy } from "@angular/core";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { BaseTxtComponentDialog } from "../basetxt-dialog.component";
import { BaseTxtComponent } from "../basetxt.component";

@Component({
    selector: 'view-confirmpwd',
    templateUrl: '../basetxt.component.html',
    styleUrls: ['./confirmpwd.component.scss']
})
export class ConfirmPwdComponent extends BaseTxtComponent implements OnDestroy  {
    constructor(private dialog: MatDialog) {
        super();
    }

    open() {
        return this.dialog.open(ConfirmPwdComponentDialog, {
            data: { opt: this.option }
        });
    }

    getType() : string {
        return 'password';
    }
}

@Component({
    selector: 'view-confirm-pwd-dialog',
    templateUrl: '../basetxt-dialog.component.html',
})
export class ConfirmPwdComponentDialog extends BaseTxtComponentDialog {
    constructor(
        @Inject(MAT_DIALOG_DATA) public data: any,
        public dialogRef: MatDialogRef<ConfirmPwdComponentDialog>
    ) {
        super(data, dialogRef);
    }

    protected allValidationRules(): string[] {
        return ['REQUIRED', 'CONFIRMPWD'];
    }
}
