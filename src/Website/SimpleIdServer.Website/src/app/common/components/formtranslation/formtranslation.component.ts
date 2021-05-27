import { Component, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Translation } from '../../translation';

@Component({
  selector: 'formtranslation',
  templateUrl: './formtranslation.component.html'
})
export class FormTranslationComponent {
  @Input() translations: Array<Translation> = [];
  addTranslationForm: FormGroup;

  constructor() {
    this.addTranslationForm = new FormGroup({
      language: new FormControl(''),
      value: new FormControl('')
    });
  }

  addTranslation(form: any) {
    const translation = new Translation();
    translation.Language = form.language;
    translation.Value = form.value
    this.translations.push(translation);
    this.addTranslationForm.reset();
  }

  delete(index: number) {
    this.translations.splice(index, 1);
  }
}
