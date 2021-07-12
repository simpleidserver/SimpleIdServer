import { ProvisioningConfigurationRecord } from "./provisioningconfigurationrecord.model";

export class ProvisioningConfiguration {
  constructor() {
    this.records = [];
  }

  id: string;
  type: number;
  resourceType: string;
  updateDateTime: Date;
  records: ProvisioningConfigurationRecord[];
}
