import { EventEmitter, Injectable, OnInit, ViewContainerRef } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { Translation } from "@app/common/translation";

@Injectable()
export class BaseUIComponent implements OnInit {
  option: any;
  uiOption: any;
  form: FormGroup;
  parent: ViewContainerRef;
  onInitialized = new EventEmitter();

  openDialog() {

  }

  ngOnInit(): void {
    this.onInitialized.emit();
    this.init();
  }

  getTranslations(translations: any): Translation[] {
    return translations.map((t: any) => Translation.fromJson(t));
  }

  init() {

  }
}
