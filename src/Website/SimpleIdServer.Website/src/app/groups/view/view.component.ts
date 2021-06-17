import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { startGet } from '@app/stores/groups/actions/groups.actions';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { Group } from '@app/stores/groups/models/group.model';
import { FormControl, FormGroup } from '@angular/forms';
import { GroupMember } from '@app/stores/groups/models/group-member.model';
import { SelectUsersComponent } from './selectusers.component';

@Component({
  selector: 'view-user',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewGroupComponent implements OnInit {
  isLoading: boolean;
  group$: Group;
  members$: GroupMember[];
  editGroupFormGroup: FormGroup = new FormGroup({
    displayName: new FormControl({ value: '' }),
    groupType: new FormControl({ value: '' })
  });

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private dialog: MatDialog,
    private actions$: ScannedActionsSubject,
    private snackbar: MatSnackBar,
    private translateService: TranslateService,
    private router: Router) { }

  ngOnInit(): void {
    this.store.pipe(select(fromReducers.selectGroupResult)).subscribe((group: Group | null) => {
      if (!group) {
        return;
      }

      this.group$ = group;
      this.members$ = JSON.parse(JSON.stringify(group.members)) as GroupMember[];
      this.isLoading = false;
      this.refreshEditForm();
    });
    this.refresh();
  }

  refresh() {
    this.isLoading = true;
    const groupId = this.activatedRoute.snapshot.params['id'];
    const request = startGet({ groupId: groupId });
    this.store.dispatch(request);
  }

  selectUsers(evt: any) {
    evt.preventDefault();
    const dialogRef = this.dialog.open(SelectUsersComponent, {
      width: '1500px',
      height: '700px',
      data: this.members$
    });
    dialogRef.afterClosed().subscribe((r: any) => {
      if (!r) {
        return;
      }

      this.members$ = r as GroupMember[];
    });
  }

  private refreshEditForm() {
    this.editGroupFormGroup.get('displayName')?.setValue(this.group$.displayName);
    this.editGroupFormGroup.get('groupType')?.setValue(this.group$.groupType);
  }
}
