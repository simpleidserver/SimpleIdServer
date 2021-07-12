import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { SearchResult } from '@app/stores/applications/models/search.model';
import * as fromReducers from '@app/stores/appstate';
import { startSearch } from '@app/stores/provisioning/actions/provisioning.actions';
import { ProvisioningConfiguration } from '@app/stores/provisioning/models/provisioningconfiguration.model';
import { select, Store } from '@ngrx/store';
import { merge } from 'rxjs';

@Component({
  selector: 'provisioning-configurations',
  templateUrl: './configurations.component.html',
  styleUrls: ['./configurations.component.scss']
})
export class ProvisioningConfigurationsComponent implements OnInit {
  displayedColumns: string[] = ['type', 'resourceType', 'updateDateTime'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  length: number;
  isLoading: boolean;
  provisioningConfigurations$: Array<ProvisioningConfiguration> = [];

  constructor(
    private store: Store<fromReducers.AppState>) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.store.pipe(select(fromReducers.selectProvisioningConfigurationsResult)).subscribe((state: SearchResult<ProvisioningConfiguration> | null) => {
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

    let request = startSearch({ order: active, direction, count, startIndex });
    this.store.dispatch(request);
  }
}
