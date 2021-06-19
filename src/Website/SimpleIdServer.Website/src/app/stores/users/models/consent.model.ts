export class Consent {
  id: string;
  clientId: string;
  scopes: string[];
  claims: string[];

  public static fromJson(json: any): Consent {
    const result = new Consent();
    result.id = json["id"];
    result.clientId = json["client_id"];
    result.scopes = json["scopes"];
    result.claims = json["claims"];
    return result;
  }
}
