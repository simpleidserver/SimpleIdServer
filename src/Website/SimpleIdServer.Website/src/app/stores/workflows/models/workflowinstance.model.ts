import { WorkflowInstanceExecutionPath } from "./workflowinstance-executionpath.model";

export class WorkflowInstance {
  constructor() {
    this.executionPaths = [];
  }

  id: string;
  status: string;
  processFileId: string;
  createDateTime: Date;
  updateDateTime: Date;
  executionPaths: WorkflowInstanceExecutionPath[];
}
