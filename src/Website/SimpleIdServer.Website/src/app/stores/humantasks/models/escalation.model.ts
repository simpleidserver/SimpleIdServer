import { ToPart } from "./topart.model";

export class Escalation {
  constructor() {
    this.toParts = [];
  }

  id: string;
  condition: string;
  toParts: ToPart[];
  notificationId: string;
}
