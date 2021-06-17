import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { SearchResult } from '@app/stores/applications/models/search.model';
import * as fromReducers from '@app/stores/appstate';
import { startSearch } from '@app/stores/users/actions/users.actions';
import { User } from '@app/stores/users/models/user.model';
import { select, Store } from '@ngrx/store';
import { merge } from 'rxjs';

@Component({
  selector: 'users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  displayedColumns: string[] = ['userName', 'name.familyName', 'displayName', 'meta.lastModified'];
  displayedFilterColumns: string[] = ['userName-filter', 'familyName-filter', 'displayName-filter', 'lastModified-filter'];
  userNameControl: FormControl = new FormControl();
  familyNameControl: FormControl = new FormControl();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  length: number;
  isLoading: boolean;
  users$: Array<User> = [];
  @Input() isNavigationDisabled: boolean = false;
  @Input() isSelectionDisabled: boolean = true;
  @Input() isShadowEnabled: boolean = true;
  @Output() usersSeleted: EventEmitter<User[]> = new EventEmitter();
  selection: SelectionModel<User> = new SelectionModel<User>(true);

  constructor(
    private store: Store<fromReducers.AppState>) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectUsersResult)).subscribe((state: SearchResult<User> | null) => {
      if (!state || !state.Content) {
        return;
      }

      this.users$ = JSON.parse(JSON.stringify(state.Content)) as User[];
      this.length = state.TotalLength;
      this.isLoading = false;
    });
    if (!this.isSelectionDisabled) {
      this.displayedColumns.push('action');
      this.displayedFilterColumns.push('action-filter');
    }
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
    const filter = this.buildFilter();
    let request = startSearch({ order: active, direction, count, startIndex, filter });
    this.store.dispatch(request);
  }

  select(row: User) {
    this.selection.toggle(row);
    row.isSelected = !row.isSelected;
  }

  addSelectedUsers(evt: any) {
    evt.preventDefault();
    const filteredUsers = this.users$.filter((u: User) => u.isSelected);
    filteredUsers.forEach((f: User) => {
      f.isSelected = false;
    });
    this.selection.clear();
    this.usersSeleted.emit(filteredUsers);
  }

  private buildFilter() {
    const familyName = this.familyNameControl.value;
    const userName = this.userNameControl.value;
    const filters : string[] = [];
    if (userName) {
      filters.push("userName co \""+ userName +"\"");
    }

    if (familyName) {
      filters.push("name.familyName co \"" + familyName + "\"");
    }

    return filters.join(' and ');
  }
}
