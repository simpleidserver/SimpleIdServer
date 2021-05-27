export class ApplicationSecret {
  Type: string;
  Value: string;

  public static fromJson(json: any): ApplicationSecret {
    var result = new ApplicationSecret();
    result.Type = json["type"];
    result.Value = json["value"];
    return result;
  }
}
