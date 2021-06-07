import { Injectable, Pipe, PipeTransform } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { Translation } from "../common/translation";
import { BaseTranslate } from "./basetranslate";

@Injectable()
@Pipe({
  name: 'translateenum',
  pure: false
})
export class TranslateEnumPipe extends BaseTranslate implements PipeTransform {
  constructor(private ts: TranslateService) {
    super(ts);
  }

  transform(translations: Translation[]) {
    if (!translations || translations.length === 0) {
      return null;
    }

    return this.translate(translations);
  }
}
