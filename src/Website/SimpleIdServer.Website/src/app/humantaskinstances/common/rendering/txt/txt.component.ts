import { Component, Inject, OnDestroy } from "@angular/core";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { BaseTxtComponentDialog } from "../basetxt-dialog.component";
import { BaseTxtComponent } from "../basetxt.component";

@Component({
    selector: 'view-txt',
    templateUrl: '../basetxt.component.html',
    styleUrls: ['./txt.component.scss']
})
export class TxtComponent extends BaseTxtComponent implements OnDestroy {
    constructor(private dialog: MatDialog) {
        super();
    }

    open() {
        return this.dialog.open(TxtComponentDialog, {
            data: { opt: this.option }
        });
    }

    getType(): string {
        return 'text';
    }
}

@Component({
    selector: 'view-txt-dialog',
    templateUrl: '../basetxt-dialog.component.html',
})
export class TxtComponentDialog extends BaseTxtComponentDialog {
    constructor(
        @Inject(MAT_DIALOG_DATA) public data: any,
        public dialogRef: MatDialogRef<TxtComponentDialog>
    ) {
        super(data, dialogRef);
    }

    protected allValidationRules(): string[] {
        return ['REQUIRED'];
    }
}
