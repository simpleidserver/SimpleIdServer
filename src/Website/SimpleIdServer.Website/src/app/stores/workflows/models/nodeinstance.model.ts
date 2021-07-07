import { ActivityStateHistory } from "./activitystatehistory.model";

export class NodeInstance {
  constructor() {
    this.activityStates = [];
  }

  id: string;
  flowNodeId: string;
  state: string;
  activityState: string;
  metadata: any;
  activityStates: ActivityStateHistory[];
}
