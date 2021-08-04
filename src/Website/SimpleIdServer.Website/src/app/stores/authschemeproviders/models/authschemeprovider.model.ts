export class AuthSchemeProvider {
  createDateTime: Date;
  displayName: string;
  handlerFullQualifiedName: string;
  id: string;
  name: string;
  options: any;
  jsonConverter: string;
  updateDateTime: Date;
  isEnabled: boolean;

  public static fromJson(json: any): AuthSchemeProvider {
    var result = new AuthSchemeProvider();
    result.createDateTime = json['create_datetime'];
    result.displayName = json['displayname'];
    result.handlerFullQualifiedName = json['handler_fullqualifiedname'];
    result.id = json['id'];
    result.name = json['name'];
    result.options = json['options'];
    result.jsonConverter = json['json_converter'];
    result.updateDateTime = json['update_datetime'];
    result.isEnabled = json['is_enabled'];
    return result;
  }
}
