import { Component, Inject } from "@angular/core";
import { FormGroup, FormControl } from "@angular/forms";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { BaseUIComponent } from "../baseui.component";
import { GuidGenerator } from '../../guidgenerator';

@Component({
    selector: 'view-row',
    templateUrl: 'row.component.html',
    styleUrls: ['./row.component.scss']
})
export class RowComponent extends BaseUIComponent {
    constructor(private dialog: MatDialog) {
        super();
    }

    openDialog() {
        const dialogRef = this.dialog.open(RowComponentDialog, {
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
    selector: 'view-row-dialog',
    templateUrl: 'rowdialog.component.html',
})
export class RowComponentDialog {
    configureRowForm: FormGroup;
    constructor(
        @Inject(MAT_DIALOG_DATA) public data: any,
        public dialogRef: MatDialogRef<RowComponentDialog>
    ) {
        this.configureRowForm = new FormGroup({
            nbColumns: new FormControl({ value: '' })
        });
        this.configureRowForm.get('nbColumns')?.setValue(data.opt.children.length);
    }

    onSave(val: { nbColumns: number }) {
        const opt = this.data.opt;
        opt.children = [];
        const percentage = (100 / val.nbColumns) + '%';
        for (let i = 0; i < val.nbColumns; i++) {
            opt.children.push({ id: GuidGenerator.newGUID(), children: [], type: 'column', width: percentage });
        }

        this.dialogRef.close(opt);
    }

    close(evt: any) {
        evt.preventDefault();
        evt.stopPropagation();
        this.dialogRef.close(null);
    }
}
