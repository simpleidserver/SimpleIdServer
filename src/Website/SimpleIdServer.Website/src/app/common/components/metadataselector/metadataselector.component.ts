import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';

interface MetadataOption {
  key: string;
  value: string;
}

@Component({
  selector: 'metadataselector',
  templateUrl: './metadataselector.component.html'
})
export class MetadatadataSelector implements OnInit, OnDestroy {
  @Input() label: string;
  @Input() metadata: string;
  @Output() changed = new EventEmitter<string>();
  options: MetadataOption[] = [];
  formControl: FormControl = new FormControl();
  onLangChange: Subscription | null;

  constructor(private translateService: TranslateService) { }

  ngOnInit(): void {
    this.refresh();
    this.onLangChange = this.translateService.onLangChange.subscribe((lng: LangChangeEvent) => {
      this.refresh();
    });
    this.formControl.valueChanges.subscribe(() => {
      this.changed.emit(this.formControl.value);
    });
  }

  ngOnDestroy(): void {
    if (this.onLangChange) {
      this.onLangChange.unsubscribe();
    }
  }

  private refresh() {
    this.options = [];
    const obj = this.translateService.instant(this.metadata);
    for (var key in obj.children) {
      const translation = obj.children[key].translations[0].value;
      this.options.push({ key: key, value: translation });
    }
  }
}
