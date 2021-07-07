import { WorkflowInstanceExecutionPointer } from "./workflowinstance-executionpointer.model";

export class WorkflowInstanceExecutionPath {
  constructor() {
    this.executionPointers = [];
  }

  id: string;
  createDateTime: Date;
  executionPointers: WorkflowInstanceExecutionPointer[];
}
