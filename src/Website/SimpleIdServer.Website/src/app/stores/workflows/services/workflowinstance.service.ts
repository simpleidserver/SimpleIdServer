import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from '@envs/environments';
import { OAuthService } from "angular-oauth2-oidc";
import { Observable } from "rxjs";
import { SearchWorkflowInstanceResult } from "../models/searchworkflowinstance.model";
import { WorkflowInstance } from "../models/workflowinstance.model";

@Injectable()
export class WorkflowInstanceService {
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService) { }

  search(startIndex: number, count: number, order: string, direction: string, processFileId: string): Observable<SearchWorkflowInstanceResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/workflowinstances/search";
    const request: any = { startIndex: startIndex, count: count };
    if (order) {
      request["orderBy"] = order;
    }

    if (direction) {
      request["order"] = direction;
    }

    if (processFileId) {
      request["processFileId"] = processFileId;
    }

    return this.http.post<SearchWorkflowInstanceResult>(targetUrl, JSON.stringify(request), { headers: headers });
  }

  create(processFileId: string): Observable<SearchWorkflowInstanceResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/workflowinstances";
    const request: any = { processFileId: processFileId };
    return this.http.post<SearchWorkflowInstanceResult>(targetUrl, JSON.stringify(request), { headers: headers });
  }

  start(id: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/workflowinstances/" + id + "/start";
    return this.http.get<any>(targetUrl, { headers: headers });
  }

  get(id: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/workflowinstances/" + id;
    return this.http.get<any>(targetUrl, { headers: headers });
  }
}
