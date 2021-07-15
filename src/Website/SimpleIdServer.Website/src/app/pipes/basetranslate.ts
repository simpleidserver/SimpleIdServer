import { Injectable } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { Translation } from "../common/translation";

@Injectable()
export class BaseTranslate {
  constructor(private translateService: TranslateService) { }

  translate(translations : Translation[]) {
    const lng = this.translateService.currentLang;
    const filteredTranslations = translations.filter(function (tr: Translation) {
      return tr.Language === lng;
    });
    if (filteredTranslations.length === 0) {
      return "unknown";
    }

    return filteredTranslations[0].Value;
  }
}
