import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MaterialModule } from '@app/shared/material.module';
import { LoaderComponent } from './components/loader/loader.component';
import { MetadatadataSelector } from './components/metadataselector/metadataselector.component';
import { MultiSelector } from './components/multiselector/multiselector.component';

@NgModule({
  imports: [
    CommonModule,
    MaterialModule
  ],
  declarations: [
    LoaderComponent,
    MetadatadataSelector,
    MultiSelector
  ],
  exports: [
    LoaderComponent,
    MetadatadataSelector,
    MultiSelector
  ]
})

export class SIDCommonModule { }
