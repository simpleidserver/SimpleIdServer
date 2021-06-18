import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { User } from '@app/stores/users/models/user.model';
import { select, Store } from '@ngrx/store';

@Component({
  selector: 'view-user',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewUserComponent implements OnInit {
  user$: User;
  id: string;

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private router: Router) {

  }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectUserResult)).subscribe((user: User | null) => {
      if (!user) {
        return;
      }

      this.user$ = user;
    });
    this.id = this.activatedRoute.snapshot.params['id'];
  }
}
