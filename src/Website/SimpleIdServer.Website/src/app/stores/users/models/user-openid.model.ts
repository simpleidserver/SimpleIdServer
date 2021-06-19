import { Consent } from "./consent.model";

export class UserOpenId {
  constructor() {
    this.consents = [];
  }

  id: string;
  status: number;
  claims: any;
  consents: Consent[];

  public static fromJson(json: any): UserOpenId {
    const result = new UserOpenId();
    result.id = json["id"];
    result.status = json["status"];
    result.claims = json["claims"];
    if (json["consents"]) {
      result.consents = json["consents"].map((c: any) => Consent.fromJson(c));
    }

    return result;
  }
}
