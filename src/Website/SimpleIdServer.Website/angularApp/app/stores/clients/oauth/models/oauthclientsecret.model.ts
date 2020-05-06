export class OAuthClientSecret {
    Type: string;
    Value: string;

    public static fromJson(json : any) : OAuthClientSecret  {
        var result = new OAuthClientSecret();
        result.Type = json["type"];
        result.Value = json["value"];
        return result;
    }
}