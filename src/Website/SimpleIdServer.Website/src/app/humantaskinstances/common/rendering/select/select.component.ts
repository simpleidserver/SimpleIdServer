import { Component, Inject } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { BaseUIComponent } from "../baseui.component";

@Component({
    selector: 'view-select',
    templateUrl: 'select.component.html',
    styleUrls: ['./select.component.scss']
})
export class SelectComponent extends BaseUIComponent {
    constructor(private dialog: MatDialog) {
        super();
    }

    openDialog() {
        if (!this.uiOption.editMode) {
            return;
        }

        const dialogRef = this.dialog.open(SelectComponentDialog, {
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
    selector: 'view-select-dialog',
    templateUrl: 'selectdialog.component.html',
})
export class SelectComponentDialog {
    configureSelectForm: FormGroup;

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: any,
        public dialogRef: MatDialogRef<SelectComponentDialog>) {
        this.configureSelectForm = new FormGroup({
        });
    }

    onSave(val: {}) {
        if (val) { }
        const opt = this.data.opt;
        this.dialogRef.close(opt);
    }
}
