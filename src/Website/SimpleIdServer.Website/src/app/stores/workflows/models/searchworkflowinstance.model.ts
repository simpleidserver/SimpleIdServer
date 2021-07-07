import { WorkflowInstance } from "./workflowinstance.model";

export class SearchWorkflowInstanceResult {
  startIndex: number;
  totalLength: number;
  count: number;
  content: WorkflowInstance[];
}
