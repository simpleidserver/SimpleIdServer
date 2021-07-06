import { DelegateConfiguration } from "./delegateconfiguration.model";

export class SearchDelegateConfigurationResult {
  startIndex: number;
  count: number;
  totalLength: number;
  content: DelegateConfiguration[];
}
