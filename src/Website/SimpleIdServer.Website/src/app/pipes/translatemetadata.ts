import { Injectable, Pipe, PipeTransform } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { map } from "rxjs/operators";

@Injectable()
@Pipe({
  name: 'translatemetadata',
  pure: false
})
export class TranslateMetadataPipe implements PipeTransform {
  constructor(private translateService: TranslateService) { }

  transform(value: any, ...args: any[]) {
    if (!value) {
      return '';
    }

    const translated = this.translateService.instant(value).translations;
    return translated ? translated[0].value : '';
  }
}
