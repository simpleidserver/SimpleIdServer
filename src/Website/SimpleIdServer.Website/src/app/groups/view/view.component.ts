import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import { startGet, startUpdate } from '@app/stores/groups/actions/groups.actions';
import { GroupMember } from '@app/stores/groups/models/group-member.model';
import { Group } from '@app/stores/groups/models/group.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';
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
    this.actions$.pipe(
      filter((action: any) => action.type === '[Groups] ERROR_UPDATE_GROUP'))
      .subscribe(() => {
        this.isLoading = false;
        this.snackbar.open(this.translateService.instant('groups.messages.errorUpdate'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Groups] COMPLETE_UPDATE_GROUP'))
      .subscribe(() => {
        this.group$.displayName = this.editGroupFormGroup.get('displayName')?.value;
        this.isLoading = false;
        this.snackbar.open(this.translateService.instant('groups.messages.update'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.store.pipe(select(fromReducers.selectGroupResult)).subscribe((group: Group | null) => {
      if (!group) {
        return;
      }

      this.group$ = JSON.parse(JSON.stringify(group)) as Group;
      this.members$ = group.members;
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

  saveGroup(evt: any, formValue: any) {
    evt.preventDefault();
    const req: any = {
      schemas: ["urn:ietf:params:scim:schemas:core:2.0:Group"],
      displayName: formValue.displayName,
      groupType: formValue.groupType,
      members: this.members$.map((m: GroupMember) => {
        return { value: m.value };
      })
    };
    const request = startUpdate({ groupId: this.group$.id, request: req });
    this.store.dispatch(request);
  }

  private refreshEditForm() {
    this.editGroupFormGroup.get('displayName')?.setValue(this.group$.displayName);
    this.editGroupFormGroup.get('groupType')?.setValue(this.group$.groupType);
  }
}
