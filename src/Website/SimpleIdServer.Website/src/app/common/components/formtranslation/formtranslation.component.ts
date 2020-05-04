import { Component, ViewChild, Input } from '@angular/core';
import { Translation } from '../../translation';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
    selector: 'formtranslation',
    templateUrl: './formtranslation.component.html'
})
export class FormTranslationComponent {
    @Input() translations : Array<Translation> = [];
    addTranslationForm : FormGroup;

    constructor() {
        this.addTranslationForm = new FormGroup({
            language: new FormControl(''),
            value: new FormControl('')
        });
    }

    addTranslation(form : any) {
        const translation = new Translation();
        translation.Language = form.language;
        translation.Value = form.value
        this.translations.push(translation);
    }

    delete(index) {
        this.translations.splice(index, 1);
    }
}