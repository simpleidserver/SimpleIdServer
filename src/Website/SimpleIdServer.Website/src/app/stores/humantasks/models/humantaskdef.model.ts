import { Deadline } from "./deadline.model";
import { Parameter } from "./parameter.model";
import { PeopleAssignment } from "./peopleassignment.model";
import { PresentationElement } from "./presentationelement.model";
import { PresentationParameter } from "./presentationparameter.model";

export class HumanTaskDef {
  id: string;
  version: number;
  name: string;
  nbInstances: number;
  updateDateTime: Date;
  createDateTime: Date;
  actualOwnerRequired: boolean;
  priority: number;
  outcome: string;
  searchBy: string;
  operationParameters: Parameter[];
  static getInputOperationParameters(hd: HumanTaskDef): Parameter[] {
    return hd.operationParameters.filter(function (v: Parameter) {
      return v.usage === 'INPUT';
    });
  }
  static getOutputOperationParameters(hd: HumanTaskDef): Parameter[] {
    return hd.operationParameters.filter(function (v: Parameter) {
      return v.usage === 'OUTPUT';
    });
  }
  peopleAssignments: PeopleAssignment[];
  static getPotentialOwners(hd: HumanTaskDef) {
    return hd.peopleAssignments.filter(function (p: PeopleAssignment) {
      return p.usage === 'POTENTIALOWNER';
    });
  }
  static getExcludedOwners(hd: HumanTaskDef) {
    return hd.peopleAssignments.filter(function (p: PeopleAssignment) {
      return p.usage === 'EXCLUDEDOWNER';
    });
  }
  static getTaskInitiators(hd: HumanTaskDef) {
    return hd.peopleAssignments.filter(function (p: PeopleAssignment) {
      return p.usage === 'TASKINITIATOR';
    });
  }
  static getTaskStakeHolders(hd: HumanTaskDef) {
    return hd.peopleAssignments.filter(function (p: PeopleAssignment) {
      return p.usage === 'TASKSTAKEHOLDER';
    });
  }
  static getBusinessAdministrators(hd: HumanTaskDef) {
    return hd.peopleAssignments.filter(function (p: PeopleAssignment) {
      return p.usage === 'BUINESSADMINISTRATOR';
    });
  }
  static getRecipients(hd: HumanTaskDef) {
    return hd.peopleAssignments.filter(function (p: PeopleAssignment) {
      return p.usage === 'RECIPIENT';
    });
  }
  presentationElements: PresentationElement[];
  static getNames(hd: HumanTaskDef) {
    return hd.presentationElements.filter(function (pe: PresentationElement) {
      return pe.usage === 'NAME';
    });
  }
  static getDescriptions(hd: HumanTaskDef) {
    return hd.presentationElements.filter(function (pe: PresentationElement) {
      return pe.usage === 'DESCRIPTION';
    });
  }
  static getSubjects(hd: HumanTaskDef) {
    return hd.presentationElements.filter(function (pe: PresentationElement) {
      return pe.usage === 'SUBJECT';
    });
  }
  static getStartDeadlines(hd: HumanTaskDef) {
    return hd.deadLines.filter(function (d: Deadline) {
      return d.usage === 'START';
    });
  }
  static getCompletionDeadlines(hd: HumanTaskDef) {
    return hd.deadLines.filter(function (d: Deadline) {
      return d.usage === 'COMPLETION';
    });
  }
  rendering: string[];
  presentationParameters: PresentationParameter[];
  deadLines: Deadline[];
}
