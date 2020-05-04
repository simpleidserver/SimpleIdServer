import { Component, ViewChild } from '@angular/core';
import { OnInit } from '@angular/core';
import { merge } from 'rxjs';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { select, Store, createSelector } from '@ngrx/store';
import { StartSearch } from '../../../stores/clients/oauth/actions/client.actions';
import { SearchResult } from '../../../stores/clients/oauth/models/search.model';
import { OAuthClient } from '../../../stores/clients/oauth/models/oauthclient.model';
import * as fromReducers from '../../../stores/appstate';

@Component({
    selector: 'list-oauth-clients',
    templateUrl: './list.component.html'
})
export class ListOauthClientsComponent implements OnInit {
    displayedColumns: string[] = ['picture', 'client_id', 'client_name', 'create_datetime', 'update_datetime', 'actions'];
    @ViewChild(MatPaginator) paginator : MatPaginator;
    @ViewChild(MatSort) sort : MatSort;
    length: number;
    oauthClients$: Array<OAuthClient> = [];

    constructor(private store : Store<fromReducers.AppState>) { }

    ngOnInit(): void {
        this.store.pipe(select(fromReducers.selectOAuthClientsResult)).subscribe((state : SearchResult<OAuthClient>) => {
            if (!state) {
                return;
            }

            this.oauthClients$ = state.Content;
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

        let request = new StartSearch(active, direction, count, startIndex);
        this.store.dispatch(request);
    }
}