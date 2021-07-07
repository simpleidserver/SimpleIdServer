import { MessageToken } from "./messagetoken.model";
import { NodeInstance } from "./nodeinstance.model";

export class WorkflowInstanceExecutionPointer {
  constructor() {
    this.incomingTokens = [];
    this.outgoingTokens = [];
  }

  id: string;
  isActive: boolean;
  flowNodeId: string;
  incomingTokens: MessageToken[];
  outgoingTokens: MessageToken[];
  flowNodeInstance: NodeInstance;
}
