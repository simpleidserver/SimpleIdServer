export class Translation {
    Language: string;
    Value: string;

    public static fromJson(json : any) : Translation {
        var result = new Translation();
        result.Language = json["language"];
        result.Value = json["value"];
        return result;
    }
}