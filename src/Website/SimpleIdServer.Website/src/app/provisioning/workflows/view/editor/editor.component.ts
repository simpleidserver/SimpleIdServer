import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import * as fromReducers from '@app/stores/appstate';
import * as fromWorkflowActions from '@app/stores/workflows/actions/workflow.actions';
import * as fromDelegateConfigurationActions from '@app/stores/delegateconfigurations/actions/delegateconfigurations.actions';
import * as fromHumanTaskDefActions from '@app/stores/humantasks/actions/humantasks.actions';
import { WorkflowFile } from '@app/stores/workflows/models/workflowfile.model';
import { ScannedActionsSubject, select, Store } from '@ngrx/store';
import { HumanTaskDef } from '@app/stores/humantasks/models/humantaskdef.model';
import { Parameter } from '@app/stores/humantasks/models/parameter.model';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
declare var require: any
let BpmnViewer = require('bpmn-js/lib/Modeler'),
  propertiesPanelModule = require('bpmn-js-properties-panel'),
  propertiesProviderModule = require('bpmn-js-properties-panel/lib/provider/bpmn');
let caseMgtBpmnModdle = require('@app/moddlextensions/casemanagement-bpmn.json');

@Component({
  selector: 'view-workflow-editor',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.scss']
})
export class ViewEditorComponent implements OnInit, OnDestroy {
  isLoadingDelegateConfigurations: boolean;
  isLoadingHumanTaskDefs: boolean;
  viewer: any;
  subscription: any;
  secondSubscription: any;
  thirdSubscription: any;
  fourthSubscription: any;
  selectedElt: any;
  buildingForm: boolean = true;
  isEltSelected: boolean = false;
  workflow: WorkflowFile;
  inputParameters: Parameter[] = [];
  humanTaskDefs: HumanTaskDef[] = [];
  outgoingElts: string[] = [];
  delegateIds: string[] = [];
  parameters: { key: string, value: string }[] = [];
  updatePropertiesForm: FormGroup = new FormGroup({
    id: new FormControl(''),
    name: new FormControl(''),
    implementation: new FormControl(''),
    delegateId: new FormControl(''),
    wsHumanTaskDefName: new FormControl(''),
    default: new FormControl(''),
    gatewayDirection: new FormControl(''),
    sequenceFlowCondition: new FormControl('')
  });
  addParameterForm: FormGroup = new FormGroup({
    key: new FormControl(''),
    value: new FormControl('')
  });
  
  constructor(
    private store: Store<fromReducers.AppState>,
    private activatedRoute: ActivatedRoute,
    private translateService: TranslateService,
    private snackbar: MatSnackBar,
    private actions$: ScannedActionsSubject) { }

  get isLoading() {
    return this.isLoadingDelegateConfigurations || this.isLoadingHumanTaskDefs;
  }

  ngOnInit() {
    const self = this;
    this.viewer = new BpmnViewer.default({
      additionalModules: [
        propertiesPanelModule,
        propertiesProviderModule
      ],
      container: "#canvas",
      keyboard: {
        bindTo: window
      },
      moddleExtensions: {
        cmg: caseMgtBpmnModdle
      }
    });
    const evtBus = this.viewer.get('eventBus');
    evtBus.on('element.click', function (evt: any) {
      self.updateProperties(evt.element);
    });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Workflows] COMPLETE_UPDATE_FILE_PAYLOAD'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('workflow.messages.update'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.actions$.pipe(
      filter((action: any) => action.type === '[Workflows] ERROR_UPDATE_FILE_PAYLOAD'))
      .subscribe(() => {
        this.snackbar.open(this.translateService.instant('workflow.messages.errorUpdate'), this.translateService.instant('undo'), {
          duration: 2000
        });
      });
    this.subscription = this.store.pipe(select(fromReducers.selectWorkflowFileResult)).subscribe((workflow: WorkflowFile | null) => {
      if (!workflow) {
        return;
      }

      this.workflow = workflow;
      this.viewer.importXML(workflow.payload);
    });
    this.secondSubscription = this.store.pipe(select(fromReducers.selectDelegateConfigurationsResult)).subscribe((delegateIds: string[] | null) => {
      if (!delegateIds || delegateIds.length === 0) {
        return;
      }

      this.isLoadingDelegateConfigurations = false;
      this.delegateIds = delegateIds;
    });
    this.thirdSubscription = this.store.pipe(select(fromReducers.selectHumanTaskDefsResult)).subscribe((humanTaskDefs : HumanTaskDef[] | null) => {
      if (!humanTaskDefs || humanTaskDefs.length === 0) {
        return;
      }

      this.isLoadingHumanTaskDefs = false;
      this.humanTaskDefs = humanTaskDefs;
    });
    this.fourthSubscription = this.activatedRoute.parent?.params.subscribe((e) => {
      this.refresh();
    });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.secondSubscription.unsubscribe();
    this.thirdSubscription.unsubscribe();
    this.fourthSubscription.unsubscribe();
  }

  save() {
    if (this.workflow.status !== 'Edited') {
      return;
    }

    const self = this;
    this.viewer.saveXML({}, function (e: any, x: any) {
      if (e) {
        return;
      }

      const act = fromWorkflowActions.startUpdateFilePayload({id: self.workflow.id, payload: x});
      self.store.dispatch(act);
    });
  }

  ngAfterViewInit() {
    this.refresh();
  }

  onHumanTaskChanged(evt: any) {
    const value: string = evt.value;
    this.selectHumanTask(value);
  }

  addParameter(form: any) {
    if (!this.selectedElt) {
      return;
    }

    const bo = this.selectedElt.businessObject;
    if (bo.$type === 'bpmn:UserTask') {
      this.parameters.push(form);
      this.addParameterForm.reset();
      this.saveProperties(this.updatePropertiesForm.value);
    }
  }

  removeParameter(elt: any) {
    if (!this.selectedElt) {
      return;
    }

    const bo = this.selectedElt.businessObject;
    if (bo.$type === 'bpmn:UserTask') {
      const index = this.parameters.indexOf(elt);
      this.parameters.splice(index, 1);
      this.saveProperties(this.updatePropertiesForm.value);
    }
  }

  private refresh() {
    this.isLoadingDelegateConfigurations = true;
    this.isLoadingHumanTaskDefs = true;
    const startGetDelegateConfigurations = fromDelegateConfigurationActions.startGetAll();
    const startGetAllHumanTaskDefs = fromHumanTaskDefActions.startGetAll();
    this.store.dispatch(startGetDelegateConfigurations);
    this.store.dispatch(startGetAllHumanTaskDefs);
  }

  private saveProperties(form: any) {
    if (!this.selectedElt) {
      return;
    }

    const moddle = this.viewer.get('moddle');
    const modeling = this.viewer.get('modeling');
    const elementRegistry = this.viewer.get('elementRegistry');
    const obj: any = {
      id: form.id,
      name: form.name
    };

    const bo = this.selectedElt.businessObject;
    if (bo.$type === 'bpmn:ServiceTask') {
      obj['cmg:delegateId'] = form.delegateId;
      obj['implementation'] = form.implementation;
    }

    if (bo.$type === 'bpmn:UserTask') {
      obj['implementation'] = form.implementation;
      obj['cmg:wsHumanTaskDefName'] = form.wsHumanTaskDefName;
      let extensionElements = bo.extensionElements || moddle.create('bpmn:ExtensionElements');
      let parameters = this.getExtension(bo, 'cmg:Parameters');
      if (!parameters) {
        parameters = moddle.create('cmg:Parameters');
        extensionElements.get('values').push(parameters);
      }

      parameters.parameter = [];
      this.parameters.forEach(function (p: any) {
        let parameter = moddle.create('cmg:Parameter');
        parameter.key = p.key;
        parameter.value = p.value;
        parameters.parameter.push(parameter);
      });
    }

    if (bo.$type === 'bpmn:ExclusiveGateway') {
      if (form.default) {
        obj['default'] = elementRegistry.get(form.default).businessObject;
      }

      obj['gatewayDirection'] = form.gatewayDirection;
    }

    if (bo.$type === 'bpmn:SequenceFlow') {
      var newCondition = moddle.create('bpmn:FormalExpression', {
        body: form.sequenceFlowCondition
      });
      obj['conditionExpression'] = newCondition;
    }

    modeling.updateProperties(this.selectedElt, obj);
  }

  private updateProperties(elt: any) {
    this.buildingForm = true;
    this.updatePropertiesForm.reset();
    this.selectedElt = elt;
    this.isEltSelected = true;
    const self = this;
    const bo = elt.businessObject;
    this.updatePropertiesForm.get('id')?.setValue(bo.id);
    this.updatePropertiesForm.get('name')?.setValue(bo.name);
    if (bo.$type === 'bpmn:ServiceTask') {
      this.updatePropertiesForm.get('implementation')?.setValue(bo.implementation);
      this.updatePropertiesForm.get('delegateId')?.setValue(bo.get('cmg:delegateId'));
    }

    if (bo.$type === 'bpmn:UserTask') {
      this.updatePropertiesForm.get('implementation')?.setValue(bo.implementation);
      this.updatePropertiesForm.get('wsHumanTaskDefName')?.setValue(bo.get('cmg:wsHumanTaskDefName'));
      this.selectHumanTask(bo.get('cmg:wsHumanTaskDefName'));
      const parameters = this.getExtension(bo, 'cmg:Parameters');
      self.parameters = [];
      if (parameters && parameters.parameter) {
        parameters.parameter.forEach(function (p: any) {
          self.parameters.push({ key: p.key, value: p.value });
        });
      }
    }

    if (bo.$type === 'bpmn:ExclusiveGateway') {
      if (bo.default) {
        this.updatePropertiesForm.get('default')?.setValue(bo.default.id);
      }

      this.updatePropertiesForm.get('gatewayDirection')?.setValue(bo.gatewayDirection);
      if (bo.outgoing) {
        this.outgoingElts = bo.outgoing.map(function (o: any) {
          return o.id;
        });
      }
    }

    if (bo.$type === 'bpmn:SequenceFlow') {
      if (bo.conditionExpression) {
        this.updatePropertiesForm.get('sequenceFlowCondition')?.setValue(bo.conditionExpression.body);
      }
    }

    this.buildingForm = false;
  }

  private selectHumanTask(name: string) {
    const filteredHumanTaskDefs = this.humanTaskDefs.filter(function (ht: HumanTaskDef) {
      return ht.name === name;
    })
    if (filteredHumanTaskDefs.length !== 1) {
      this.inputParameters = [];
      return;
    }

    this.inputParameters = HumanTaskDef.getInputOperationParameters(filteredHumanTaskDefs[0]);
  }

  private getExtension(elt: any, type: string) {
    if (!elt.extensionElements) {
      return null;
    }

    return elt.extensionElements.values.filter(function (e: any) {
      return e.$type === type;
    })[0];
  }
}
