import { Component, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'formlist',
  templateUrl: './formlist.component.html'
})
export class FormListComponent {
  @Input() values: Array<string> = [];
  addValueForm: FormGroup;

  constructor() {
    this.addValueForm = new FormGroup({
      value: new FormControl('')
    });
  }

  addValue(form: any) {
    this.values.push(form.value);
    this.addValueForm.reset();
  }

  delete(index : any) {
    this.values.splice(index, 1);
  }
}
