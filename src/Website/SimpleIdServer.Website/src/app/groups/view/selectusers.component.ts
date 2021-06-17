import { Component, Inject, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatListOption, MatSelectionList } from '@angular/material/list';
import { User } from '@app/stores/users/models/user.model';
import { GroupMember } from '@app/stores/groups/models/group-member.model';

interface DialogData {
  jwk: any;
}

@Component({
  selector: 'selectusers',
  templateUrl: './selectusers.component.html',
  styleUrls: ['./selectusers.component.scss']
})
export class SelectUsersComponent {
  selectedUsers: User[] = [];
  @ViewChild('selectionList') selectionList: MatSelectionList; 

  constructor(
    @Inject(MAT_DIALOG_DATA) private members: GroupMember[],
    private dialogRef: MatDialogRef<SelectUsersComponent>) {
    this.selectedUsers = members.map((g: GroupMember) => {
      const u = new User();
      u.id = g.value;
      u.userName = g.display;
      return u;
    });
  }

  displayUsers(users: User[]) {
    users.forEach((u: User) => {
      const filtered = this.selectedUsers.filter((su: User) => su.id === u.id);
      if (filtered.length > 0) {
        return;
      }

      this.selectedUsers.push(u);
    });
  }

  remove() {
    const indexes = this.selectionList.selectedOptions.selected.map((v: MatListOption) => {
      return this.selectedUsers.indexOf(v.value as User);
    });
    this.selectedUsers = this.selectedUsers.filter((u: User, i: number) => !indexes.includes(i));
  }

  confirm() {
    this.dialogRef.close(this.selectedUsers.map((u: User) => {
      var record = new GroupMember();
      record.value = u.id;
      record.display = u.userName;
      return record;
    }));
  }
}
