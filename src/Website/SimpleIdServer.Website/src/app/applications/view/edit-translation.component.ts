import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Translation } from '@app/common/translation';
import * as fromReducers from '@app/stores/appstate';
import { Language } from '@app/stores/metadata/models/language.model';
import { select, Store } from '@ngrx/store';

interface DialogData {
  translations: Translation[],
  title: string
}

interface TranslationRecord {
  value: string;
  language: string;
}

@Component({
  selector: 'edit-translation',
  templateUrl: './edit-translation.component.html'
})
export class EditTranslationComponent {
  records: TranslationRecord[] = [];
  languages: string[] = [];
  editTranslationsForm: FormGroup = new FormGroup({});

  constructor(
    private store: Store<fromReducers.AppState>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData,
    private dialogRef: MatDialogRef<EditTranslationComponent>) {
    this.store.pipe(select(fromReducers.selectLanguagesResult)).subscribe((languages: Array<Language> | null) => {
      if (!languages) {
        return;
      }

      languages.forEach((l: Language) => {
        const filtered = data.translations.filter((t) => {
          return t.Language === l.Name;
        });
        let value : string = "";
        if (filtered.length === 1) {
          value = filtered[0].Value;
        }

        this.languages.push(l.Name);
        this.editTranslationsForm.addControl(l.Name, new FormControl(value));
      });
    });
  }

  save() {
    this.dialogRef.close(this.editTranslationsForm.value);
  }
}
