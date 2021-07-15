import { NgModule } from '@angular/core';
import { PipesModule } from '@app/pipes/pipes.module';
import { MaterialModule } from '@app/shared/material.module';
import { SharedModule } from '@app/shared/shared.module';
import { ColumnComponent } from './column/column.component';
import { ConfirmPwdComponent, ConfirmPwdComponentDialog } from './confirmpwd/confirmpwd.component';
import { ContainerComponent } from './container/container.component';
import { DynamicComponent } from './dynamic.component';
import { HeaderComponent, HeaderComponentDialog } from './header/header.component';
import { PwdComponent, PwdComponentDialog } from './pwd/pwd.component';
import { RowComponent, RowComponentDialog } from './row/row.component';
import { SelectComponent, SelectComponentDialog } from './select/select.component';
import { SubmitBtnComponent, SubmitBtnComponentDialog } from './submitbtn/submitbtn.component';
import { TxtComponent, TxtComponentDialog } from './txt/txt.component';

@NgModule({
  imports: [
    SharedModule,
    MaterialModule,
    PipesModule
  ],
  declarations: [
    DynamicComponent,
    ColumnComponent,
    ConfirmPwdComponent,
    ContainerComponent,
    HeaderComponent,
    ConfirmPwdComponentDialog,
    HeaderComponentDialog,
    PwdComponent,
    PwdComponentDialog,
    RowComponent,
    RowComponentDialog,
    SelectComponent,
    SelectComponentDialog,
    SubmitBtnComponent,
    SubmitBtnComponentDialog,
    TxtComponent,
    TxtComponentDialog
  ],
  entryComponents: [
    TxtComponentDialog,
    SubmitBtnComponentDialog,
    SelectComponentDialog,
    RowComponentDialog,
    PwdComponentDialog,
    HeaderComponentDialog,
    ConfirmPwdComponentDialog
  ],
  exports: [
    DynamicComponent,
    ColumnComponent,
    ConfirmPwdComponent,
    ContainerComponent,
    HeaderComponent,
    ConfirmPwdComponentDialog,
    HeaderComponentDialog,
    PwdComponent,
    PwdComponentDialog,
    RowComponent,
    RowComponentDialog,
    SelectComponent,
    SelectComponentDialog,
    SubmitBtnComponent,
    SubmitBtnComponentDialog,
    TxtComponent,
    TxtComponentDialog
  ]
})

export class RenderingModule { }
