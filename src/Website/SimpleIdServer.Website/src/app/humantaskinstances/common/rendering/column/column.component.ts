import { Component, OnInit } from "@angular/core";
import { BaseUIComponent } from "../baseui.component";

@Component({
    selector: 'view-column',
    templateUrl: 'column.component.html',
    styleUrls: ['./column.component.scss']
})
export class ColumnComponent extends BaseUIComponent implements OnInit {
    ngOnInit() {
    }
}