import { Component, Input } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
    selector: 'formlist',
    templateUrl: './formlist.component.html'
})
export class FormListComponent {
    @Input() values : Array<String> = [];
    addValueForm : FormGroup;

    constructor() {
        this.addValueForm = new FormGroup({
            value: new FormControl('')
        });
    }

    addValue(form : any) {
        this.values.push(form.value);
    }

    delete(index) {
        this.values.splice(index, 1);
    }
}