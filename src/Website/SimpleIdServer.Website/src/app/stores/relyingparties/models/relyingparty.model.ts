import { RelyingPartyClaimMapping } from "./relyingparty-claimapping.model";

export class RelyingParty {
  constructor() {
    this.claimMappings = [];
  }

  id: string;
  assertionExpirationTimeInSeconds: number;
  claimMappings: RelyingPartyClaimMapping[];
  createDateTime: Date;
  updateDateTime: Date;
  metadataUrl: string;

  public static fromJson(json: any): RelyingParty {
    const result = new RelyingParty();
    result.id = json['id'];
    result.assertionExpirationTimeInSeconds = json['assertion_expiration_time_seconds'];
    result.metadataUrl = json['metadata_url'];
    result.createDateTime = json['create_datetime'];
    result.updateDateTime = json['update_datetime'];
    if (json['claim_mappings']) {
      result.claimMappings = json['claim_mappings'].map((rec: any) => {
        var result: RelyingPartyClaimMapping = { claimFormat: rec['claim_format'], claimName: rec['claim_name'], userAttribute : rec['user_attribute'] };
        return result;
      });
    }

    return result;
  }
}
