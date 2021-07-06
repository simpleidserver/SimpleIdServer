import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { WorkflowFile } from '@app/stores/workflows/models/workflowfile.model';
import { select, Store } from '@ngrx/store';

@Component({
  selector: 'view-workflow',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewWorkflowComponent implements OnInit, OnDestroy {
  subscription: any;
  workflow$: WorkflowFile;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.subscription = this.store.pipe(select(fromReducers.selectWorkflowFileResult)).subscribe((workflow: WorkflowFile | null) => {
      if (!workflow) {
        return;
      }

      this.workflow$ = workflow;
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
