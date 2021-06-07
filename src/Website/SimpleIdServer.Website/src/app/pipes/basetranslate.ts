import { TranslateService } from "@ngx-translate/core";
import { Translation } from "../common/translation";

export class BaseTranslate {
  constructor(private translateService: TranslateService) { }

  translate(translations : Translation[]) {
    const lng = this.translateService.currentLang;
    var filteredTranslations = translations.filter(function (tr: Translation) {
      return tr.Language === lng;
    });
    if (filteredTranslations.length === 0) {
      return "unknown";
    }

    return filteredTranslations[0].Value;
  }
}
