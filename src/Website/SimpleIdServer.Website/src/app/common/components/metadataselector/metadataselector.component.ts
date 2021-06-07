import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';

interface MetadataOption {
  key: number;
  value: string;
}

@Component({
  selector: 'metadataselector',
  templateUrl: './metadataselector.component.html'
})
export class MetadatadataSelector implements OnInit, OnDestroy {
  private _value: number;
  @Input() label: string;
  @Input() metadata: string;
  @Input()
  get value() {
    return this._value;
  }
  set value(value: number) {
    if (value === null || value === undefined) {
      return;
    }

    this._value = value;
    this.formControl.setValue(this.value, { emitEvent: false });
  }
  formControl: FormControl = new FormControl();
  @Output() changed = new EventEmitter<string>();
  options: MetadataOption[] = [];
  onLangChange: Subscription | null;
  onValueChange: Subscription | null;

  constructor(private translateService: TranslateService) { }

  ngOnInit(): void {
    this.refresh();
    this.onLangChange = this.translateService.onLangChange.subscribe((lng: LangChangeEvent) => {
      this.refresh();
    });
    this.onValueChange = this.formControl.valueChanges.subscribe(() => {
      this.changed.emit(this.formControl.value);
    });
  }

  ngOnDestroy(): void {
    if (this.onLangChange) {
      this.onLangChange.unsubscribe();
    }

    if (this.onValueChange) {
      this.onValueChange.unsubscribe();
    }
  }

  private refresh() {
    const lng = this.translateService.currentLang;
    this.options = [];
    const obj = this.translateService.instant(this.metadata);
    for (var key in obj.children) {
      var filteredTranslations = obj.children[key].translations.filter(function (tr: any) {
        return tr.languageCode === lng;
      });
      var translation = "unknown";
      if (filteredTranslations.length > 0) {
        translation = filteredTranslations[0].value;
      }

      this.options.push({ key: parseInt(key), value: translation });
    }
  }
}
