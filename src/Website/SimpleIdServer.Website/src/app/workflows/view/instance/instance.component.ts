import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import * as fromWorkflowInstanceActions from '@app/stores/workflows/actions/workflow.actions';
import { WorkflowFile } from '@app/stores/workflows/models/workflowfile.model';
import { WorkflowInstanceExecutionPath } from '@app/stores/workflows/models/workflowinstance-executionpath.model';
import { WorkflowInstanceExecutionPointer } from '@app/stores/workflows/models/workflowinstance-executionpointer.model';
import { WorkflowInstance } from '@app/stores/workflows/models/workflowinstance.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
declare var require: any
declare var $: any;
let BpmnViewer = require('bpmn-js/lib/Viewer');

@Component({
  selector: 'view-workflow-instance',
  templateUrl: './instance.component.html',
  styleUrls: ['./instance.component.scss']
})
export class ViewInstanceComponent implements OnInit, OnDestroy {
  displayedColumns: string[] = ['name', 'content'];
  isLoading: boolean;
  firstSubscription: any;
  secondSubscription: any;
  workflow: WorkflowFile;
  workflowInstance$: WorkflowInstance;
  execPointer: WorkflowInstanceExecutionPointer = new WorkflowInstanceExecutionPointer();
  viewer: any;
  execPathId: string | null;
  executionPathFormControl: FormControl = new FormControl();

  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService,
    private snackbar: MatSnackBar,
    private actions$: ScannedActionsSubject) { }

  ngOnInit() {
    const instanceId = this.activatedRoute.snapshot.params['instanceid'];
    const id = this.activatedRoute.parent?.snapshot.params['id'];
    this.execPathId = null
    this.workflowInstance$ = new WorkflowInstance();
    this.workflow = new WorkflowFile();
    this.viewer = new BpmnViewer.default({
      container: "#canvasView",
      keyboard: {
        bindTo: window
      }
    });
    this.firstSubscription = this.store.pipe(select(fromReducers.selectWorkflowInstanceResult)).subscribe((workflowInstance: WorkflowInstance | null) => {
      if (!workflowInstance || instanceId !== workflowInstance.id) {
        return;
      }

      this.isLoading = false;
      this.workflowInstance$ = workflowInstance;
      this.refreshCanvas();
    });
    this.secondSubscription = this.store.pipe(select(fromReducers.selectWorkflowFileResult)).subscribe((workflow: WorkflowFile | null) => {
      if (!workflow || id !== workflow.id) {
        return;
      }

      this.workflow = workflow;
      this.refreshCanvas();
    });
    this.refresh();
  }

  ngOnDestroy() {
    this.firstSubscription.unsubscribe();
    this.secondSubscription.unsubscribe();
  }

  ngAfterViewInit() {

  }

  updateExecutionPath() {
    this.execPathId = this.executionPathFormControl.value;
    this.refreshCanvas();
  }

  getException() {
    if (!this.execPointer || !this.execPointer.flowNodeInstance || this.execPointer.flowNodeInstance.activityState !== 'FAILING') {
      return '';
    }

    return this.execPointer.flowNodeInstance.activityStates.filter(s => s.state === 'FAILING')[0].message;
  }

  private refresh() {
    const instanceId = this.activatedRoute.snapshot.params['instanceid'];
    const id = this.activatedRoute.parent?.snapshot.params['id'];
    const action = fromWorkflowInstanceActions.startGetFile({ id: id });
    const act = fromWorkflowInstanceActions.getInstance({ id: instanceId });
    this.store.dispatch(action);
    this.store.dispatch(act);
  }

  private refreshCanvas() {
    if (!this.workflow.id || !this.workflowInstance$.id) {
      return;
    }

    const self = this;
    if (!self.execPathId) {
      var inst: any = null;
      self.workflowInstance$.executionPaths.forEach((e: WorkflowInstanceExecutionPath) => {
        if (!inst) {
          inst = e;
          return;
        }

        if (inst.createDateTime < e.createDateTime) {
          inst = e;
        }
      });

      if (inst) {
        self.execPathId = inst.id;
        self.executionPathFormControl.setValue(inst.id);
      }
    }

    this.viewer.importXML(self.workflow.payload).then(function () {
      if (self.workflowInstance$.executionPaths && self.workflowInstance$.executionPaths.length > 0) {
        const canvas = self.viewer.get('canvas');
        canvas.zoom('fit-viewport');
        const filtered = self.workflowInstance$.executionPaths.filter(function (v: WorkflowInstanceExecutionPath) {
          return v.id === self.execPathId;
        })

        if (filtered.length !== 1) {
          return;
        }

        self.displayExecutionPath(null, filtered[0]);
      }
    });
  }

  private displayExecutionPath(evt: any, execPath: WorkflowInstanceExecutionPath) {
    if (evt) {
      evt.preventDefault();
    }

    const self = this;
    let overlays = self.viewer.get('overlays');
    let elementRegistry = self.viewer.get('elementRegistry');
    execPath.executionPointers.forEach(function (execPointer: WorkflowInstanceExecutionPointer) {
      let elt = execPointer.flowNodeInstance;
      let eltReg = elementRegistry.get(elt.flowNodeId);
      overlays.remove({ element: elt.flowNodeId });
      let errorOverlayHtml = "<div class='{0}' data-id='" + execPointer.id + "' style='width:" + eltReg.width + "px;height:" + eltReg.height + "px;'></div>";
      let completeOverlayHtml = "<div class='{0}' data-id='" + execPointer.id + "' style='width:" + eltReg.width + "px;height:" + eltReg.height + "px;'></div>";
      let selectedOverlayHtml: any = "<div class='{0}'></div>";
      const isCircle = eltReg.type === "bpmn:StartEvent" ? true : false;
      const isDiamond = eltReg.type === "bpmn:ExclusiveGateway" ? true : false;
      var errorOverlayCl = "error-overlay";
      var completeOverlayCl = "complete-overlay";
      var selectedOverlayCl = "selected-overlay";
      if (isCircle) {
        errorOverlayCl = errorOverlayCl + " circle";
        completeOverlayCl = completeOverlayCl + " circle";
        selectedOverlayCl = selectedOverlayCl + " selected-circle";
      }

      if (isDiamond) {
        errorOverlayCl = errorOverlayCl + " diamond";
        completeOverlayCl = completeOverlayCl + " diamond";
        selectedOverlayCl = selectedOverlayCl + " selected-diamond";
      }

      errorOverlayHtml = errorOverlayHtml.replace('{0}', errorOverlayCl);
      completeOverlayHtml = completeOverlayHtml.replace('{0}', completeOverlayCl);
      selectedOverlayHtml = selectedOverlayHtml.replace('{0}', selectedOverlayCl);
      errorOverlayHtml = $(errorOverlayHtml);
      completeOverlayHtml = $(completeOverlayHtml);
      selectedOverlayHtml = $(selectedOverlayHtml);
      selectedOverlayHtml.hide();
      if (elt.activityState && elt.activityState === 'FAILING') {
        overlays.add(elt.flowNodeId, {
          position: {
            top: 0,
            left: 0,
          },
          html: errorOverlayHtml
        });
      } else if (elt.state === 'Complete') {
        overlays.add(elt.flowNodeId, {
          position: {
            top: 0,
            left: 0,
          },
          html: completeOverlayHtml
        });
      }

      overlays.add(elt.flowNodeId, {
        position: {
          top: -1,
          left: -1,
        },
        html: selectedOverlayHtml
      });
      $(completeOverlayHtml).click(function(e : any) {
        const eltid = $(e.target).data('id');
        $(".selected-overlay").hide();
        selectedOverlayHtml.show();
        self.displayElt(eltid);
      });
      $(errorOverlayHtml).click(function (e : any) {
        const eltid = $(e.target).data('id');
        $(".selected-overlay").hide();
        selectedOverlayHtml.show();
        self.displayElt(eltid);
      });
    });
  }

  private displayElt(eltId: string) {
    const self = this;
    const filteredPath = self.workflowInstance$.executionPaths.filter((execPath: WorkflowInstanceExecutionPath) => {
      return execPath.id === self.execPathId;
    });
    if (filteredPath.length !== 1) {
      return;
    }

    const execPointer = filteredPath[0].executionPointers.filter(function (p: WorkflowInstanceExecutionPointer) {
      return p.id === eltId;
    })[0];
    this.execPointer = execPointer;
    console.log(this.execPointer);
  }
}
