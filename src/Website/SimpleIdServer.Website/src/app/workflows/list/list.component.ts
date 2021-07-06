import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import * as fromReducers from '@app/stores/appstate';
import * as fromWorkflowActions from '@app/stores/workflows/actions/workflow.actions';
import { SearchWorkflowFileResult } from '@app/stores/workflows/models/searchworkflowfile.model';
import { WorkflowFile } from '@app/stores/workflows/models/workflowfile.model';
import { select, Store } from '@ngrx/store';
import { merge } from 'rxjs';

@Component({
  selector: 'list-workflows',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListWorkflowsComponent implements OnInit, OnDestroy {
  displayedColumns: string[] = ['name', 'nbInstances', 'create_datetime', 'update_datetime'];
  isLoading: boolean;
  subscription: any;
  workflows$: WorkflowFile[] = [];
  length: number;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private store: Store<fromReducers.AppState>) { }

  ngOnInit() {
    this.subscription = this.store.pipe(select(fromReducers.selectWorkflowFilesResult)).subscribe((state: SearchWorkflowFileResult| null) => {
      if (!state) {
        return;
      }

      this.workflows$ = state.content;
      this.length = state.totalLength;
      this.isLoading = false;
    });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  ngAfterViewInit() {
    merge(this.sort.sortChange, this.paginator.page).subscribe(() => this.refresh());
    this.refresh();
  }

  private refresh() {
    this.isLoading = true;
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

    const request = fromWorkflowActions.startSearchFiles({ startIndex: startIndex, count: count, direction: direction, order: active });
    this.store.dispatch(request);
  }
}
