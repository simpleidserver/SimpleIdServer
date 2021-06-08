import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { Router } from '@angular/router';
import { startAdd, startSearch } from '@app/stores/applications/actions/applications.actions';
import { Application } from '@app/stores/applications/models/application.model';
import { SearchResult } from '@app/stores/applications/models/search.model';
import * as fromReducers from '@app/stores/appstate';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { merge } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AddApplicationComponent } from './add-application.component';

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

  constructor(
    private store: Store<fromReducers.AppState>,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private router: Router,
    private snackbar: MatSnackBar,
    private translateService: TranslateService) { }

  ngOnInit(): void {
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] COMPLETE_ADD_APPLICATON'))
      .subscribe((evt: any) => {
        this.snackbar.open(this.translateService.instant('applications.messages.add'), this.translateService.instant('undo'), {
          duration: 2000
        });
        this.router.navigate(['/applications/' + evt.clientId]);
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Applications] ERROR_ADD_APPLICATION'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('applications.messages.errorAdd'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
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

  addApplication() {
    const dialogRef = this.dialog.open(AddApplicationComponent, {
      width: '800px'
    });
    dialogRef.afterClosed().subscribe((opt : any) => {
      if (!opt) {
        return;
      }

      const addApp = startAdd({ applicationKind: opt.applicationKind, name: opt.clientName });
      this.store.dispatch(addApp);
    });
  }
}
