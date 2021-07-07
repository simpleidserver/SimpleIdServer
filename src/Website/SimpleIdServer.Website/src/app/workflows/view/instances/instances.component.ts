import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ActivatedRoute } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import * as fromWorkflowInstanceActions from '@app/stores/workflows/actions/workflow.actions';
import { SearchWorkflowInstanceResult } from '@app/stores/workflows/models/searchworkflowinstance.model';
import { WorkflowInstance } from '@app/stores/workflows/models/workflowinstance.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { merge } from 'rxjs';
import { WorkflowFile } from '@app/stores/workflows/models/workflowfile.model';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'view-workflow-instances',
  templateUrl: './instances.component.html',
  styleUrls: ['./instances.component.scss']
})
export class ViewInstancesComponent implements OnInit, OnDestroy {
  length: number;
  firstSubscription: any;
  secondSubscription: any;
  thirdSubscription: any;
  workflowFile$: WorkflowFile;
  isLoading: boolean = false;
  workflowInstances$: WorkflowInstance[];
  displayedColumns: string[] = ['status', 'nbExecutionPath', 'create_datetime', 'update_datetime',  'actions'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService,
    private snackbar: MatSnackBar,
    private actions$: ScannedActionsSubject) { }

  ngOnInit() {
    this.actions$.pipe(
      filter((action: any) => action.type === '[Workflows] COMPLETE_CREATE_INSTANCE'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('workflow.messages.createInstance'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Workflows] ERROR_CREATE_INSTANCE'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('workflow.messages.errorCreateInstance'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Workflows] COMPLETE_START_INSTANCE'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('workflow.messages.startInstance'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Workflows] ERROR_START_INSTANCE'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('workflow.messages.errorStartInstance'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.firstSubscription = this.store.pipe(select(fromReducers.selectWorkflowInstancesResult)).subscribe((search: SearchWorkflowInstanceResult | null) => {
      if (!search) {
        return;
      }

      this.isLoading = false;
      this.workflowInstances$ = search.content;
      this.length = search.totalLength;
    });
    this.secondSubscription = this.store.pipe(select(fromReducers.selectWorkflowFileResult)).subscribe((file: WorkflowFile | null) => {
      if (!file) {
        return;
      }

      this.workflowFile$ = file;
    });
    this.thirdSubscription = this.activatedRoute.parent?.params.subscribe((e) => {
      this.refresh();
    });
  }

  ngOnDestroy() {
    this.firstSubscription.unsubscribe();
    this.secondSubscription.unsubscribe();
    this.thirdSubscription.unsubscribe();
  }

  ngAfterViewInit() {
    merge(this.sort.sortChange, this.paginator.page).subscribe(() => this.refresh());
    this.refresh();
  }

  create(evt: any) {
    evt.preventDefault();
    evt.stopPropagation();
    const id = this.activatedRoute.parent?.snapshot.params['id'];
    const create = fromWorkflowInstanceActions.startCreateInstance({ id: id });
    this.store.dispatch(create);
  }

  start(instance: WorkflowInstance) {
    const start = fromWorkflowInstanceActions.startInstance({ id: instance.id });
    this.store.dispatch(start);
  }

  private refresh() {
    if (!this.paginator || !this.sort) {
      return;
    }

    const id = this.activatedRoute.parent?.snapshot.params['id'];
    let startIndex: number = 0;
    let count: number = 5;
    if (this.paginator.pageIndex && this.paginator.pageSize) {
      startIndex = this.paginator.pageIndex * this.paginator.pageSize;
    }

    if (this.paginator.pageSize) {
      count = this.paginator.pageSize;
    }

    let active = "create_datetime";
    let direction = "desc";
    if (this.sort.active) {
      active = this.sort.active;
    }

    if (this.sort.direction) {
      direction = this.sort.direction;
    }

    const searchBpmn = fromWorkflowInstanceActions.startSearchInstances({ count: count, direction: direction, order: active, processFileId: id, startIndex: startIndex });
    this.store.dispatch(searchBpmn);
  }
}
