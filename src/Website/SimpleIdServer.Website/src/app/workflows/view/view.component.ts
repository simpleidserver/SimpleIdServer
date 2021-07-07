import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { WorkflowFile } from '@app/stores/workflows/models/workflowfile.model';
import * as fromWorkflowActions from '@app/stores/workflows/actions/workflow.actions';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { SearchWorkflowFileResult } from '@app/stores/workflows/models/searchworkflowfile.model';
import { FormControl, FormGroup } from '@angular/forms';
import { startPublishFile } from '@app/stores/workflows/actions/workflow.actions';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'view-workflow',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ViewWorkflowComponent implements OnInit, OnDestroy {
  subscription: any;
  secondSubscription: any;
  thirdSubscription: any;
  workflow$: WorkflowFile;
  workflowHistory$: Array<WorkflowFile>;
  isLoading: boolean;
  isHistoryLoading: boolean;
  publishFormGroup: FormGroup = new FormGroup({
    version: new FormControl()
  });

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService,
    private snackbar: MatSnackBar,
    private router: Router,
    private actions$: ScannedActionsSubject) {

  }

  ngOnInit(): void {
    const self = this;
    this.actions$.pipe(
      filter((action: any) => action.type === '[Workflows] ERROR_PUBLISH_FILE'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('workflow.messages.errorPublish'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.publishFormGroup.get('version')?.valueChanges.subscribe(() => {
      const version = this.publishFormGroup.get('version')?.value;
      if (this.workflowHistory$) {
        const filtered = this.workflowHistory$.filter((w) => w.version === version);
        if (filtered && filtered.length === 1) {
          const splitted = this.router.url.split('/');
          const r = splitted[splitted.length - 1];
          this.router.navigate(['/workflows/' + filtered[0].id + '/' + r]);
        }
      }
    });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Workflows] COMPLETE_PUBLISH_FILE'))
      .subscribe((evt) => {
        this.router.navigate(['/workflows/' + evt.id + '/details']);
        this.snackbar.open(this.translateService.instant('workflow.messages.publish'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.subscription = this.store.pipe(select(fromReducers.selectWorkflowFileResult)).subscribe((workflow: WorkflowFile | null) => {
      if (!workflow) {
        return;
      }

      this.workflow$ = workflow;
      this.publishFormGroup.get('version')?.setValue(workflow.version);
      this.refresh();
    });
    this.secondSubscription = this.store.pipe(select(fromReducers.selectWorkflowFilesResult)).subscribe((workflows: SearchWorkflowFileResult | null) => {
      if (!workflows && !this.isHistoryLoading) {
        return;
      }

      this.isHistoryLoading = false;
      this.isLoading = false;
      if (workflows?.content) {
        this.workflowHistory$ = [];
        workflows?.content.forEach((wf: WorkflowFile) => {
          const filtered = self.workflowHistory$.filter((wh: WorkflowFile) => wh.id === wf.id);
          if (filtered.length === 0) {
            self.workflowHistory$.push(wf);
          }
        });
      }
    });
    this.thirdSubscription = this.activatedRoute.params.subscribe(() => {
      this.isLoading = true;
      const id = this.activatedRoute.parent?.snapshot.params['id'];
      const action = fromWorkflowActions.startGetFile({ id: id });
      this.store.dispatch(action);
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
    this.secondSubscription.unsubscribe();
    this.thirdSubscription.unsubscribe();
  }

  publish() {
    const id = this.activatedRoute.snapshot.params['id'];
    const act = startPublishFile({ id: id });
    this.store.dispatch(act);
  }

  private refresh() {
    this.isHistoryLoading = true;
    const active = "create_datetime";
    const direction = "desc";
    const request = fromWorkflowActions.startSearchFiles({ startIndex: 0, count: 500, direction: direction, order: active, takeLatest: false, fileId: this.workflow$.fileId });
    this.store.dispatch(request);
  }
}
