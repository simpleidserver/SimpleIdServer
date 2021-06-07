import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

interface DialogData {
  jwk: any;
}

@Component({
  selector: 'displayjwk',
  templateUrl: './displayjwk.component.html',
  styleUrls: ['./displayjwk.component.scss'],
})
export class DisplayJwkComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: DialogData) {
  }
}
