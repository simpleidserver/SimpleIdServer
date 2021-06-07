import { Injectable, Pipe, PipeTransform } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { Translation } from "../common/translation";
import { BaseTranslate } from "./basetranslate";

@Injectable()
@Pipe({
  name: 'translatemetadata',
  pure: false
})
export class TranslateMetadataPipe extends BaseTranslate implements PipeTransform {
  constructor(private ts: TranslateService) {
    super(ts);
  }

  transform(value: any, ...args: any[]) {
    const translated = this.ts.instant(value).translations;
    var translations = translated.map((r: any) => {
      var record: Translation = { Language: r.languageCode, Value: r.value };
      return record;
    });
    return this.translate(translations);
  }
}
