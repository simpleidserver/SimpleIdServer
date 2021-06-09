export class OAuthScope {
  Name: string;
  IsExposed: boolean;
  IsStandard: boolean;
  CreateDateTime: Date;
  UpdateDateTime: Date;

  public static fromJson(json: any): OAuthScope {
    var result = new OAuthScope();
    result.Name = json["name"];
    result.IsExposed = json["is_exposed"];
    result.IsStandard = json["is_standard"];
    result.CreateDateTime = json["create_datetime"];
    result.UpdateDateTime = json["update_datetime"];
    return result;
  }
}
