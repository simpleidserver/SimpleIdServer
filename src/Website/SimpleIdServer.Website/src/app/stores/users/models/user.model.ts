export class User {
  constructor() {
  }

  userName: string;
  id: string;
  familyName: string;
  created: string;
  lastModified: string;

  public static fromJson(json: any): User {
    var result = new User();
    result.userName = json["userName"];
    result.id = json["id"];
    if (json["name"]) {
      result.familyName = json["name"]["familyName"];
    }

    if (json["meta"]) {
      result.created = json["meta"]["created"];
      result.lastModified = json["meta"]["lastModified"];
    }

    return result;
  }
}
