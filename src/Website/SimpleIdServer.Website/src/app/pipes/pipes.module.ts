import { NgModule } from '@angular/core';
import { TranslateEnumPipe } from '@app/pipes/translateenum.pipe';
import { TranslateMetadataPipe } from './translatemetadata';

@NgModule({
  imports: [
  ],
  declarations: [
    TranslateEnumPipe,
    TranslateMetadataPipe
  ],
  exports: [
    TranslateEnumPipe,
    TranslateMetadataPipe
  ]
})

export class PipesModule { }
