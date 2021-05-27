import { ChangeDetectorRef, Injectable, OnDestroy, Pipe, PipeTransform } from "@angular/core";
import { LangChangeEvent, TranslateService } from "@ngx-translate/core";
import { Subscription } from "rxjs";
import { Translation } from "../common/translation";

@Injectable()
@Pipe({
  name: 'translateenum',
  pure: false
})
export class TranslateEnumPipe implements PipeTransform, OnDestroy {
  lastValue: string | null= null;
  value: string | null = null;
  onLangChange: Subscription | null;
  onDefaultLangChange: Subscription | null;

  constructor(private translateService: TranslateService, private _ref: ChangeDetectorRef) { }

  transform(translations: Translation[]) {
    if (!translations || translations.length === 0) {
      return;
    }

    if (this.lastValue && this.value && this.lastValue == this.value) {
      return this.value;
    }

    let onTranslation = (translations: Translation[], lng: string) => {
      var filteredTranslations = translations.filter(function (tr: Translation) {
        return tr.Language == lng;
      });
      if (filteredTranslations.length == 0) {
        this.value = "unknown";
        return;
      }

      this.value = filteredTranslations[0].Value;
      this._ref.markForCheck();
    };

    this._dispose();
    var defaultLang = this.translateService.getDefaultLang();
    if (this.value == null) {
      onTranslation(translations, defaultLang);
    }

    if (!this.onLangChange) {
      this.onLangChange = this.translateService.onLangChange.subscribe((lng: LangChangeEvent) => {
        if (this.lastValue) {
          this.lastValue = null;
          onTranslation(translations, lng.lang);
        }
      });
    }

    if (!this.onDefaultLangChange) {
      this.onDefaultLangChange = this.translateService.onDefaultLangChange.subscribe((lng: any) => {
        if (this.lastValue) {
          this.lastValue = null;
          onTranslation(translations, lng.lang);
        }
      });
    }

    this.lastValue = this.value;
    return this.value;
  }

  private _dispose(): void {
    if (typeof this.onLangChange !== 'undefined' && this.onLangChange !== null) {
      this.onLangChange.unsubscribe();
      this.onLangChange = null;
    }
    if (typeof this.onDefaultLangChange !== 'undefined' && this.onDefaultLangChange !== null) {
      this.onDefaultLangChange.unsubscribe();
      this.onDefaultLangChange = null;
    }
  }

  ngOnDestroy(): void {
    this._dispose();
  }
}
