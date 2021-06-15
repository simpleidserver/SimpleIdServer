import { UserPhoto } from "./userphoto.model";

export class User {
  constructor() {
    this.photos = [];
  }

  userName: string;
  givenName: string;
  id: string;
  familyName: string;
  created: string;
  lastModified: string;
  photos: UserPhoto[];

  public static fromJson(json: any): User {
    var result = new User();
    result.userName = json["userName"];
    result.id = json["id"];
    if (json["name"]) {
      result.familyName = json["name"]["familyName"];
      result.givenName = json["name"]["givenName"];
    }

    if (json["meta"]) {
      result.created = json["meta"]["created"];
      result.lastModified = json["meta"]["lastModified"];
    }

    if (json["photos"]) {
      result.photos = json["photos"].map((r: any) => {
        let record: UserPhoto = { display: r["display"], type: r["type"], value: r["value"] };
        return record;
      });
    }

    return result;
  }
}
