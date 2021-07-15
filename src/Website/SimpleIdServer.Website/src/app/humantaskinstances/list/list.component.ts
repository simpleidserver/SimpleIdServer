import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ActivatedRoute, Router } from '@angular/router';
import { SearchResult } from '@app/stores/applications/models/search.model';
import * as fromReducers from '@app/stores/appstate';
import { startSearchInstances } from '@app/stores/humantasks/actions/humantasks.actions';
import { HumanTaskInstance } from '@app/stores/humantasks/models/humantaskinstance.model';
import { select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { merge } from 'rxjs';

@Component({
  selector: 'list-humantaskinstance',
  templateUrl: './list.component.html'
})
export class ListHumanTaskInstanceComponent implements OnInit  {
  isLoading: boolean = false;
  displayedColumns: string[] = ['presentationName', 'presentationSubject', 'priority', 'status', 'createdTime'];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  length: number;
  instances$: Array<HumanTaskInstance> = [];

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private translate: TranslateService,
    private store: Store<fromReducers.AppState>,) {
  }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectHumanTaskInstancesResult)).subscribe((searchResult: SearchResult<HumanTaskInstance> | null) => {
      if (searchResult) {
        this.isLoading = false;
        this.length = searchResult.totalLength;
        this.instances$ = searchResult.content;
        return;
      }
    });
  }

  ngAfterViewInit() {
    merge(this.sort.sortChange, this.paginator.page).subscribe(() => {
      this.refreshUrl();
    });
    this.activatedRoute.queryParamMap.subscribe((p: any) => {
      this.sort.active = p.get('active');
      this.sort.direction = p.get('direction');
      this.paginator.pageSize = p.get('pageSize');
      this.paginator.pageIndex = p.get('pageIndex');
      this.refresh()
    });
    this.translate.onLangChange.subscribe(() => {
      this.refresh();
    });
  }

  onSearchTasks() {
    this.refreshUrl();
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

    this.isLoading = true;
    const active = this.getOrder();
    const direction = this.getDirection();
    const request = startSearchInstances({ order: active, direction: direction, count: count, startIndex: startIndex });
    this.store.dispatch(request);
  }

  refreshUrl() {
    const queryParams: any = {
      pageIndex: this.paginator.pageIndex,
      pageSize: this.paginator.pageSize,
      active: this.sort.active,
      direction: this.sort.direction
    };

    this.router.navigate(['.'], {
      relativeTo: this.activatedRoute,
      queryParams: queryParams
    });
  }

  private getOrder() {
    let active = "createdTime";
    if (this.sort.active) {
      active = this.sort.active;
    }

    return active;
  }

  private getDirection() {
    let direction = "desc";
    if (this.sort.direction) {
      direction = this.sort.direction;
    }

    return direction;
  }
}
