import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import * as fromReducers from '@app/stores/appstate';
import * as fromWorkflowActions from '@app/stores/workflows/actions/workflow.actions';
import { WorkflowFile } from '@app/stores/workflows/models/workflowfile.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'view-workflow-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class ViewDetailsComponent implements OnInit, OnDestroy {
  subscription: any;
  isLoading: boolean;
  workflow$: WorkflowFile;
  saveFormGroup: FormGroup = new FormGroup({
    id: new FormControl({ value: '', disabled: true }),
    name: new FormControl(),
    description: new FormControl()
  });

  constructor(
    private store: Store<fromReducers.AppState>,
    private translateService: TranslateService,
    private snackbar: MatSnackBar,
    private actions$: ScannedActionsSubject) { }

  ngOnInit() {
    this.isLoading = true;
    this.actions$.pipe(
      filter((action: any) => action.type === '[Workflows] COMPLETE_UPDATE_FILE'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('workflow.messages.update'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Workflows] ERROR_UPDATE_FILE'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('workflow.messages.errorUpdate'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.subscription = this.store.pipe(select(fromReducers.selectWorkflowFileResult)).subscribe((workflow: WorkflowFile | null) => {
      if (!workflow) {
        return;
      }

      this.saveFormGroup.get('name')?.setValue(workflow.name);
      this.saveFormGroup.get('description')?.setValue(workflow.description);
      this.saveFormGroup.get('id')?.setValue(workflow.id);
      this.workflow$ = workflow;
      this.isLoading = false;
    });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  save(form: any) {
    if (this.workflow$.status !== 'Edited') {
      return;
    }

    const action = fromWorkflowActions.startUpdateFile({ id: this.workflow$.id, name: form.name, description: form.description });
    this.store.dispatch(action);
  }
}
