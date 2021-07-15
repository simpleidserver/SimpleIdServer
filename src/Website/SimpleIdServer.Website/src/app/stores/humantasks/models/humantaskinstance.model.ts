export class HumanTaskInstance {
  id: string;
  name: string;
  status: string;
  priority: string;
  actualOwner: string;
  activationDeferralTime: Date;
  expirationTime: Date;
  presentationName: string;
  presentationSubject: string;
  parentTaskId: string;
  renderingMethodExists: boolean;
  createdTime: Date;
  lastModifiedTime: Date;
  hasPotentialOwners: boolean;
  possibleActions: string[];
}
