import { UserAddress } from "./useraddress.model";
import { UserEmail } from "./useremail.model";
import { UserPhoneNumber } from "./userphonenumber.model";
import { UserPhoto } from "./userphoto.model";
import { UserRole } from "./userrole.model";

export class User {
  constructor() {
    this.photos = [];
    this.emails = [];
    this.phoneNumbers = [];
    this.addresses = [];
    this.roles = [];
  }

  userName: string;
  displayName: string;
  nickName: string;
  profileUrl: string;
  title: string;
  userType: string;
  givenName: string;
  id: string;
  familyName: string;
  created: string;
  lastModified: string;
  photos: UserPhoto[];
  emails: UserEmail[];
  phoneNumbers: UserPhoneNumber[];
  addresses: UserAddress[];
  roles: UserRole[];

  public static fromJson(json: any): User {
    var result = new User();
    result.userName = json["userName"];
    result.displayName = json["displayName"];
    result.nickName = json["nickName"];
    result.profileUrl = json["profileUrl"];
    result.title = json["title"];
    result.userType = json["userType"];
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

    if (json["emails"]) {
      result.emails = json["emails"].map((r: any) => {
        let record: UserEmail = { display: r["display"], type: r["type"], value: r["value"] };
        return record;
      });
    }

    if (json["phoneNumbers"]) {
      result.phoneNumbers = json["phoneNumbers"].map((r: any) => {
        let record: UserPhoneNumber = { display: r["display"], type: r["type"], value: r["value"] };
        return record;
      });
    }

    if (json["addresses"]) {
      result.addresses = json["addresses"].map((r: any) => {
        let record: UserAddress = { country: r["country"], formatted: r["formatted"], locality: r["locality"], postalCode: r["postalCode"], region: r["region"], streetAddress: r["streetAddress"], type: r["type"] };
        return record;
      });
    }

    if (json["phoneNumbers"]) {
      result.roles = json["roles"].map((r: any) => {
        let record: UserRole = { display: r["display"], type: r["type"], value: r["value"] };
        return record;
      });
    }

    return result;
  }
}
