export class ProvisioningConfigurationRecord {
  constructor() {
    this.valuesString = [];
    this.values = [];
  }

  name: string;
  type: number;
  isArray: boolean;
  valuesString: string[];
  values: ProvisioningConfigurationRecord[];
}
