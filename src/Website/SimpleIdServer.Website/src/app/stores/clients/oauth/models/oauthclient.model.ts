import { Translation } from "../../../../common/translation";
import { OAuthClientSecret } from './oauthclientsecret.model';

export class OAuthClient {
    constructor() {
        this.LogoUris = [];
        this.ClientNames = [];
        this.Contacts = [];
        this.PolicyUris = [];
        this.ClientUris = [];
        this.Scopes = [];
        this.RedirectUris = [];
        this.GrantTypes = [];
        this.ResponseTypes = [];
    }

    ClientId: string;
    SoftwareId: string;
    SoftwareVersion: string;
    CreateDateTime : Date;
    UpdateDateTime : Date;
    PreferredTokenProfile: string;
    TokenEndPointAuthMethod: string;
    TokenSignedResponseAlg : string;
    TokenEncryptedResponseAlg : string;
    TokenEncryptedResponseEnc: string;
    RefreshTokenExpirationTimeInSeconds: number;
    TokenExpirationTimeInSeconds: number;
    Scopes : Array<string>;
    RedirectUris: Array<string>;
    GrantTypes : Array<string>;
    Secrets: Array<OAuthClientSecret>;
    ResponseTypes : Array<string>;
    LogoUris: Translation[];
    ClientNames: Translation[];
    PolicyUris: Translation[];
    ClientUris: Translation[];
    TosUris : Translation[];
    Contacts : Array<string>;

    public static fromJson(json : any) : OAuthClient {
        var result = new OAuthClient();
        result.ClientId = json["client_id"];
        result.SoftwareId = json["software_id"];
        result.SoftwareVersion = json["software_version"];
        result.CreateDateTime = json["create_datetime"];
        result.UpdateDateTime = json["update_datetime"];
        result.PreferredTokenProfile = json["preferred_token_profile"];
        result.TokenEndPointAuthMethod = json["token_endpoint_auth_method"];
        result.TokenSignedResponseAlg = json["token_signed_response_alg"];
        result.TokenEncryptedResponseAlg = json["token_encrypted_response_alg"];
        result.TokenEncryptedResponseEnc = json["token_encrypted_response_enc"];
        result.RefreshTokenExpirationTimeInSeconds = json["refresh_token_expiration_time_seconds"];
        result.TokenExpirationTimeInSeconds = json["token_expiration_time_seconds"];
        if (json["logo_uri"]) {
            result.LogoUris = json["logo_uri"].map(j => Translation.fromJson(j));
        }

        if (json["client_name"]) {
            result.ClientNames = json["client_name"].map(j => Translation.fromJson(j));
        }

        if (json["contacts"]) {
            result.Contacts = json["contacts"];
        }

        if (json["policy_uri"]) {
            result.PolicyUris = json["policy_uri"].map(j => Translation.fromJson(j));
        }

        if (json["client_uri"]) {
            result.ClientUris = json["client_uri"].map(j => Translation.fromJson(j));
        }

        if (json["tos_uri"]) {
            result.TosUris = json["tos_uri"].map(j => Translation.fromJson(j));
        }

        if (json["scope"]) {
            result.Scopes = json["scope"];
        }

        if (json["redirect_uri"]) {
            result.RedirectUris = json["redirect_uris"];
        }

        if (json["grant_types"]) {
            result.GrantTypes = json["grant_types"];
        }

        if (json["response_types"]) {
            result.ResponseTypes = json["response_types"];
        }

        if (json["secrets"]) {
            result.Secrets = json["secrets"].map(s => OAuthClientSecret.fromJson(s));
        }

        return result;
    }
}