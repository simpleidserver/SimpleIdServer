import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { SearchResult } from '@app/stores/applications/models/search.model';
import * as fromReducers from '@app/stores/appstate';
import { startSearchHistory } from '@app/stores/provisioning/actions/provisioning.actions';
import { ProvisioningConfigurationHistory } from '@app/stores/provisioning/models/provisioningconfigurationhistory.model';
import { select, Store } from '@ngrx/store';
import { merge } from 'rxjs';

@Component({
  selector: 'history-provisioning-configuration',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.scss']
})
export class ProvisioningConfigurationHistoryComponent implements OnInit {
  displayedColumns: string[] = ['representationId', 'workflow', 'status', 'exception', 'executionDateTime'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  length: number;
  isLoading: boolean;
  provisioningConfigurations$: Array<ProvisioningConfigurationHistory> = [];

  constructor(
    private store: Store<fromReducers.AppState>) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.store.pipe(select(fromReducers.selectProvisioningHistoriesResult)).subscribe((state: SearchResult<ProvisioningConfigurationHistory> | null) => {
      if (!state || !state.content) {
        return;
      }

      this.provisioningConfigurations$ = state.content;
      this.length = state.totalLength;
      this.isLoading = false;
    });
  }

  ngAfterViewInit() {
    merge(this.sort.sortChange, this.paginator.page).subscribe(() => this.refresh());
    this.refresh();
  }

  refresh() {
    this.isLoading = true;
    let startIndex: number = 0;
    let count: number = 5;
    if (this.paginator.pageIndex && this.paginator.pageSize) {
      startIndex = this.paginator.pageIndex * this.paginator.pageSize;
    }

    if (this.paginator.pageSize) {
      count = this.paginator.pageSize;
    }

    let active = "executionDateTime";
    let direction = "desc";
    if (this.sort.active) {
      active = this.sort.active;
    }

    if (this.sort.direction) {
      direction = this.sort.direction;
    }

    const request = startSearchHistory({ order: active, direction, count, startIndex });
    this.store.dispatch(request);
  }
}
