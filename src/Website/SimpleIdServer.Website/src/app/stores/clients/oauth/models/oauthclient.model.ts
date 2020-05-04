import { Translation } from "../../../../common/translation";

export class OAuthClient {
    ClientId: string;
    CreateDateTime : Date;
    UpdateDateTime : Date;
    PreferredTokenProfile: string;
    LogoUris: Translation[];
    ClientNames: Translation[];

    public static fromJson(json : any) : OAuthClient {
        var result = new OAuthClient();
        result.ClientId = json["client_id"];
        result.CreateDateTime = json["create_datetime"];
        result.UpdateDateTime = json["update_datetime"];
        result.PreferredTokenProfile = json["preferred_token_profile"];
        if (json["logo_uri"]) {
            result.LogoUris = json["logo_uri"].map(j => Translation.fromJson(j));
        }

        if (json["client_name"]) {
            result.ClientNames = json["client_name"].map(j => Translation.fromJson(j));
        }

        return result;
    }
}