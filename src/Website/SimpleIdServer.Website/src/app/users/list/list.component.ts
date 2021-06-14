import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { Router } from '@angular/router';
import { SearchResult } from '@app/stores/applications/models/search.model';
import * as fromReducers from '@app/stores/appstate';
import { startSearch } from '@app/stores/users/actions/users.actions';
import { User } from '@app/stores/users/models/user.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { merge } from 'rxjs';

@Component({
  selector: 'list-applications',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListUsersComponent implements OnInit {
  displayedColumns: string[] = ['userName', 'name.familyName', 'meta.lastModified'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  length: number;
  isLoading: boolean;
  users$: Array<User> = [];

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private router: Router,
    private snackbar: MatSnackBar,
    private translateService: TranslateService) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectUsersResult)).subscribe((state: SearchResult<User> | null) => {
      if (!state) {
        return;
      }

      this.users$ = state.Content;
      this.length = state.TotalLength;
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

    let active = "userName";
    let direction = "desc";
    if (this.sort.active) {
      active = this.sort.active;
    }

    if (this.sort.direction) {
      direction = this.sort.direction;
    }

    if (direction === "desc") {
      direction = "descending";
    }
    else if (direction === "asc") {
      direction = "ascending";
    }

    startIndex += 1;
    let request = startSearch({ order: active, direction, count, startIndex });
    this.store.dispatch(request);
  }
}
