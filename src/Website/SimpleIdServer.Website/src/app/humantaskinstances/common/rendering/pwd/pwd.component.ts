import { Component, Inject, OnDestroy } from "@angular/core";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { BaseTxtComponentDialog } from "../basetxt-dialog.component";
import { BaseTxtComponent } from "../basetxt.component";

@Component({
    selector: 'view-pwd',
    templateUrl: '../basetxt.component.html',
    styleUrls: ['./pwd.component.scss']
})
export class PwdComponent extends BaseTxtComponent implements OnDestroy  {
    constructor(private dialog: MatDialog) {
        super();
    }

    open() {
        return this.dialog.open(PwdComponentDialog, {
            data: { opt: this.option }
        });
    }

    getType() : string {
        return 'password';
    }
}

@Component({
    selector: 'view-pwd-dialog',
    templateUrl: '../basetxt-dialog.component.html',
})
export class PwdComponentDialog extends BaseTxtComponentDialog {
    constructor(
        @Inject(MAT_DIALOG_DATA) public data: any,
        public dialogRef: MatDialogRef<PwdComponentDialog>
    ) {
        super(data, dialogRef);
    }

    protected allValidationRules(): string[] {
        return ['REQUIRED'];
    }
}
