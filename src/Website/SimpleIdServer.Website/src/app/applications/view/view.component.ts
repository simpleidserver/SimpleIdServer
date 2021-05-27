import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { startGet } from '@app/stores/applications/actions/applications.actions';
import { Application } from '@app/stores/applications/models/application.model';
import * as fromReducers from '@app/stores/appstate';
import { select, Store } from '@ngrx/store';

@Component({
  selector: 'view-application',
  templateUrl: './view.component.html'
})
export class ViewApplicationsComponent implements OnInit {
  clientId: string | null = null;
  updateApplicationForm: FormGroup = new FormGroup({
    clientId: new FormControl({value: '', disabled: true })
  });

  constructor(private store: Store<fromReducers.AppState>, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectApplicationResult)).subscribe((application: Application | null) => {
      if (!application) {
        return;
      }

      this.clientId = application.ClientId;
      this.setApplication(application);
    });
    this.refresh();
  }

  refresh() {
    var id = this.route.snapshot.params['id'];
    let getClient = startGet({ id: id });
    this.store.dispatch(getClient);
  }

  private setApplication(application : Application) {
    this.updateApplicationForm.get('clientId')?.setValue(application.ClientId);
  }
}
