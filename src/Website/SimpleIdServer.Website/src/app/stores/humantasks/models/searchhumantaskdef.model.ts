import { HumanTaskDef } from "./humantaskdef.model";

export class SearchHumanTaskDefsResult {
  constructor() {
    this.content = [];
  }

  startIndex: number;
  totalLength: number;
  count: string;
  content: HumanTaskDef[];
}
