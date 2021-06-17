import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '@app/shared/material.module';
import { TranslateModule } from '@ngx-translate/core';
import { LoaderComponent } from './components/loader/loader.component';
import { MetadatadataSelector } from './components/metadataselector/metadataselector.component';
import { MultiSelector } from './components/multiselector/multiselector.component';
import { UsersComponent } from './components/users/users.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    MaterialModule,
    TranslateModule 
  ],
  declarations: [
    LoaderComponent,
    MetadatadataSelector,
    MultiSelector,
    UsersComponent
  ],
  exports: [
    LoaderComponent,
    MetadatadataSelector,
    MultiSelector,
    UsersComponent
  ]
})

export class SIDCommonModule { }
