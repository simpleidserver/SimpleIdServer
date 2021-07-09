import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { Router } from '@angular/router';
import { SearchResult } from '@app/stores/applications/models/search.model';
import * as fromReducers from '@app/stores/appstate';
import { startAdd, startSearch } from '@app/stores/scopes/actions/scope.actions';
import { OAuthScope } from '@app/stores/scopes/models/oauthscope.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { merge } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AddScopeComponent } from './add-scope.component';

@Component({
  selector: 'list-scopes',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListScopesComponent implements OnInit {
  displayedColumns: string[] = ['name', 'is_standard', 'update_datetime'];
  scopes$: OAuthScope[] = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  length: number;
  isLoading: boolean = true;

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private router: Router,
    private snackbar: MatSnackBar,
    private translateService: TranslateService) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.actions$.pipe(
      filter((action: any) => action.type === '[OAuthScopes] COMPLETE_ADD_SCOPE'))
      .subscribe((evt: any) => {
        this.snackbar.open(this.translateService.instant('scopes.messages.add'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.router.navigate(['/scopes/' + evt.name]);
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[OAuthScopes] ERROR_ADD_APPLICATION'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('scopes.messages.errorAdd'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectSearchOAuthScopesResult)).subscribe((state: SearchResult<OAuthScope> | null) => {
      if (!state) {
        return;
      }

      this.scopes$ = state.content;
      this.length = state.totalLength;
      this.isLoading = false;
    });
  }

  ngAfterViewInit() {
    merge(this.sort.sortChange, this.paginator.page).subscribe(() => this.refresh());
    this.refresh();
  }

  public addScope() {
    const dialogRef = this.dialog.open(AddScopeComponent, {
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((opt: any) => {
      if (!opt) {
        return;
      }

      const addScope = startAdd({ name: String(opt) });
      this.store.dispatch(addScope);
    });
  }

  private refresh() {
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
