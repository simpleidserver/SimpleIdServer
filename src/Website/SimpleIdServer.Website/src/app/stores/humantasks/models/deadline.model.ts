import { Escalation } from "./escalation.model";

export class Deadline {
  constructor() {
    this.escalations = [];
  }

  id: string;
  name: string;
  for: string;
  until: string;
  usage: string;
  escalations: Escalation[];
}
