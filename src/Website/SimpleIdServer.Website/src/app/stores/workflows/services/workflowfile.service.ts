import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from '@envs/environments';
import { OAuthService } from "angular-oauth2-oidc";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { SearchWorkflowFileResult } from "../models/searchworkflowfile.model";
import { WorkflowFile } from "../models/workflowfile.model";

@Injectable()
export class WorkflowFileService {
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService) { }

  search(startIndex: number, count: number, order: string, direction: string, takeLatest: boolean, fileId: string): Observable<SearchWorkflowFileResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/workflows/search";
    const request: any = { startIndex: startIndex, count: count, takeLatest: takeLatest };
    if (order) {
      request["orderBy"] = order;
    }

    if (direction) {
      request["order"] = direction;
    }

    if (fileId) {
      request["fileId"] = fileId;
    }

    return this.http.post<SearchWorkflowFileResult>(targetUrl, JSON.stringify(request), { headers: headers });
  }

  get(id: string): Observable<WorkflowFile> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/workflows/" + id;
    return this.http.get<WorkflowFile>(targetUrl, { headers: headers });
  }

  update(id: string, name: string, description: string) : Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/workflows/" + id;
    const request: any = { name: name, description: description };
    return this.http.put<any>(targetUrl, JSON.stringify(request), { headers: headers });
  }

  updatePayload(id: string, payload: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/workflows/" + id + "/payload";
    const request: any = { payload: payload };
    return this.http.put<any>(targetUrl, JSON.stringify(request), { headers: headers });
  }

  publish(id: string): Observable<string> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/workflows/" + id + "/publish";
    return this.http.get<any>(targetUrl, { headers: headers }).pipe(map((res: any) => {
      return res['id'];
    }));
  }
}
