import { Translation } from "@app/common/translation";

export class Application {
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
    this.PostLogoutRedirectUris = [];
  }

  ClientId: string;
  SoftwareId: string;
  SoftwareVersion: string;
  CreateDateTime: Date;
  UpdateDateTime: Date;
  PreferredTokenProfile: string;
  TokenEndPointAuthMethod: string;
  TokenSignedResponseAlg: string;
  TokenEncryptedResponseAlg: string;
  TokenEncryptedResponseEnc: string;
  RefreshTokenExpirationTimeInSeconds: number;
  TokenExpirationTimeInSeconds: number;
  Scopes: Array<string>;
  RedirectUris: Array<string>;
  PostLogoutRedirectUris: Array<string>;
  GrantTypes: Array<string>;
  ClientSecret: string;
  ResponseTypes: Array<string>;
  LogoUris: Translation[];
  ClientNames: Translation[];
  PolicyUris: Translation[];
  ClientUris: Translation[];
  TosUris: Translation[];
  Contacts: Array<string>;

  public static fromJson(json: any): Application {
    var result = new Application();
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
    result.ClientSecret = json["client_secret"];
    result.RefreshTokenExpirationTimeInSeconds = json["refresh_token_expiration_time_seconds"];
    result.TokenExpirationTimeInSeconds = json["token_expiration_time_seconds"];
    result.ClientNames = this.extractTranslations("client_name", json);
    result.ClientUris = this.extractTranslations("client_uri", json);
    result.LogoUris = this.extractTranslations("logo_uri", json);
    result.TosUris = this.extractTranslations("tos_uri", json);
    result.PolicyUris = this.extractTranslations("policy_uri", json);
    if (json["contacts"]) {
      result.Contacts = json["contacts"];
    }

    if (json["scope"]) {
      result.Scopes = json["scope"];
    }

    if (json["redirect_uri"]) {
      result.RedirectUris = json["redirect_uris"];
    }

    if (json["post_logout_redirect_uris"]) {
      result.PostLogoutRedirectUris = json["post_logout_redirect_uris"];
    }

    if (json["grant_types"]) {
      result.GrantTypes = json["grant_types"];
    }

    if (json["response_types"]) {
      result.ResponseTypes = json["response_types"];
    }

    return result;
  }

  private static extractTranslations(key: string, json: any) : Translation[]{
    let result: Translation[] = [];
    for (var rec in json) {
      if (rec.startsWith(key)) {
        let splitted = rec.split('#');
        let record = new Translation();
        record.Language = splitted[1];
        record.Value = json[rec];
        result.push(record);
      }
    }

    return result;
  }
}
