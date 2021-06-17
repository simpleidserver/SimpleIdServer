import { GroupMember } from "./group-member.model";

export class Group {
  constructor() {
    this.members = [];
  }

  id: string;
  displayName: string;
  groupType: string;
  created: string;
  lastModified: string;
  nbMembers: number;
  members: GroupMember[];

  public static fromJson(json: any): Group {
    var result = new Group();
    result.id = json["id"];
    result.displayName = json["displayName"];
    result.groupType = json["groupType"];
    if (json["meta"]) {
      result.created = json["meta"]["created"];
      result.lastModified = json["meta"]["lastModified"];
    }

    if (json["members"]) {
      result.members = json["members"].map((r: any) => {
        var member: GroupMember = { $ref: r["$ref"], type: r["type"], value: r["value"], display: r["display"] };
        return member;
      });
      result.nbMembers = result.members.length;
    }

    return result;
  }
}
