import { Component, ComponentFactoryResolver, ComponentRef, Input, OnDestroy, OnInit, ViewChild, ViewContainerRef } from "@angular/core";
import { BaseUIComponent } from "./baseui.component";
import { ColumnComponent } from "./column/column.component";
import { ContainerComponent } from "./container/container.component";
import { RowComponent } from "./row/row.component";
import { SelectComponent } from "./select/select.component";
import { TxtComponent } from "./txt/txt.component";
import { HeaderComponent } from "./header/header.component";
import { FormGroup } from "@angular/forms";
import { PwdComponent } from "./pwd/pwd.component";
import { ConfirmPwdComponent } from "./confirmpwd/confirmpwd.component";
import { SubmitBtnComponent } from "./submitbtn/submitbtn.component";

@Component({
  selector: 'dynamic-component',
  templateUrl: 'dynamic.component.html',
  styleUrls: ['./dynamic.component.scss']

})
export class DynamicComponent implements OnInit, OnDestroy {
  private componentRef: ComponentRef<unknown> | null;
  private _option: any;
  private _form: FormGroup;
  private _uiOption: { editMode: boolean } = {
    editMode: true
  };
  private _dic: any = {
    'row': RowComponent,
    'column': ColumnComponent,
    'txt': TxtComponent,
    'select': SelectComponent,
    'container': ContainerComponent,
    'header': HeaderComponent,
    'pwd': PwdComponent,
    'confirmpwd': ConfirmPwdComponent,
    'submitbtn': SubmitBtnComponent
  };
  @ViewChild('container', { read: ViewContainerRef }) container: ViewContainerRef;
  @ViewChild('parent', { read: ViewContainerRef }) parent: ViewContainerRef;
  baseUIComponent: BaseUIComponent | null = null;
  @Input()
  get option() {
    return this._option;
  }
  set option(val: any) {
    if (!val) {
      return;
    }

    this._option = val;
    this.refresh();
  }
  @Input()
  get form() {
    return this._form;
  }
  set form(f: any) {
    if (!f) {
      return;
    }

    this._form = f;
    this.refresh();
  }
  @Input()
  get uiOption() {
    return this._uiOption;
  }
  set uiOption(val: any) {
    if (!val) {
      return;
    }

    this._uiOption = val;
    this.refresh();
  }
  @Input() parentOption: any = null;
  isSelected: boolean = false;
  isFocused: boolean = false;
  title: string | null = null;

  constructor(private compFactoryResolver: ComponentFactoryResolver) { }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.refresh();
  }

  ngOnDestroy() {
    if (!this.componentRef) {
      return;
    }

    this.componentRef.destroy();
    this.componentRef = null;
  }

  clickComponent(evt: any) {
    if (!this._uiOption.editMode) {
      return;
    }

    evt.preventDefault();
    evt.stopPropagation();
    this.isSelected = !this.isSelected;
  }

  dropElt(evt: any) {
    if (!this._uiOption.editMode) {
      return;
    }

    evt.preventDefault();
    evt.stopPropagation();
    if (!this.option.children) {
      return;
    }

    const str = evt.dataTransfer.getData('json');
    if (!str) {
      return;
    }

    const json = JSON.parse(evt.dataTransfer.getData('json'));
    this.option.children.push(json);
    this.isFocused = false;
  }

  dragLeave(evt: any) {
    if (!this._uiOption.editMode) {
      return;
    }

    evt.preventDefault();
    evt.stopPropagation();
    if (!this.option.children) {
      return;
    }

    this.isFocused = false;
  }

  dragOver(evt: any) {
    if (!this._uiOption.editMode) {
      return;
    }

    evt.preventDefault();
    evt.stopPropagation();
    if (!this.option.children) {
      return;
    }

    this.isFocused = true;
  }

  openSettings(evt: any) {
    if (!this._uiOption.editMode) {
      return;
    }

    evt.preventDefault();
    evt.stopPropagation();
    if (!this.baseUIComponent) {
      return;
    }

    this.baseUIComponent.openDialog();
  }

  remove(evt: any) {
    if (!this._uiOption.editMode) {
      return;
    }

    evt.preventDefault();
    evt.stopPropagation();
    if (!this.baseUIComponent || !this.parentOption || !this.parentOption.children) {
      return;
    }

    const self = this;
    const filtered = this.parentOption.children.filter((r: any) => {
      return self.baseUIComponent?.option.id === r.id;
    });
    if (filtered.length !== 1) {
      return;
    }

    const index = this.parentOption.children.indexOf(filtered[0]);
    this.parentOption.children.splice(index, 1);
  }

  private refresh() {
    if (!this.option || !this.form || !this.container) {
      return;
    }

    this.container.clear();
    const type = this._dic[this.option.type];
    if (!type) {
      return;
    }

    const factory = this.compFactoryResolver.resolveComponentFactory(type);
    this.componentRef = this.container.createComponent(factory);
    this.baseUIComponent = this.componentRef.instance as BaseUIComponent;
    this.baseUIComponent.option = this.option;
    this.baseUIComponent.form = this.form;
    this.baseUIComponent.onInitialized.subscribe(() => {
      if (this.baseUIComponent) {
        this.baseUIComponent.uiOption = this.uiOption;
        this.baseUIComponent.parent = this.parent;
      }
    });
    this.title = this.option.type;
  }
}
