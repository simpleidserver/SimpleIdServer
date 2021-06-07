import { Component, Input } from '@angular/core';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { TranslateService } from '@ngx-translate/core';

export class SelectionResult {
  isSelected: boolean;
  name: string;
  value: any;
}

@Component({
  selector: 'multiselector',
  templateUrl: './multiselector.component.html'
})
export class MultiSelector {
  @Input() label: string;
  @Input() selections: SelectionResult[] = [];
  @Input() displayName: Function;

  constructor(private translateService: TranslateService) { }

  handleChecked(evt: MatCheckboxChange, selection: SelectionResult) {
    selection.isSelected = evt.checked;
  }

  display(selection: SelectionResult) {
    if (!this.displayName) {
      return selection.name;
    }

    return this.displayName(selection);
  }
}
