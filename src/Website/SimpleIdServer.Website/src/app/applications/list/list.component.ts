import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { startSearch } from '@app/stores/applications/actions/applications.actions';
import { Application } from '@app/stores/applications/models/application.model';
import { SearchResult } from '@app/stores/applications/models/search.model';
import * as fromReducers from '@app/stores/appstate';
import { select, Store } from '@ngrx/store';
import { merge } from 'rxjs';

@Component({
  selector: 'list-applications',
  templateUrl: './list.component.html'
})
export class ListApplicationsComponent implements OnInit {
  displayedColumns: string[] = ['picture', 'client_id', 'client_name', 'application_kind', 'update_datetime'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  length: number;
  applications$: Array<Application> = [];

  constructor(private store: Store<fromReducers.AppState>) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectApplicationsResult)).subscribe((state: SearchResult<Application> | null) => {
      if (!state) {
        return;
      }

      this.applications$ = state.Content;
      this.length = state.TotalLength;
    });
  }

  ngAfterViewInit() {
    merge(this.sort.sortChange, this.paginator.page).subscribe(() => this.refresh());
    this.refresh();
  }

  refresh() {
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

    let request = startSearch({ order: active, direction, count, startIndex });
    this.store.dispatch(request);
  }
}
