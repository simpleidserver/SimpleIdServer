
export class Language {
  Name: string;
  DisplayName: string;

  public static fromJson(json: any): Language {
    var result = new Language();
    result.Name = json["name"];
    result.DisplayName = json["display_name"];
    return result;
  }
}
