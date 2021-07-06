import { WorkflowFile } from "./workflowfile.model";

export class SearchWorkflowFileResult {
  startIndex: number;
  totalLength: number;
  count: number;
  content: WorkflowFile[];
}
