import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from '@envs/environments';
import { OAuthService } from "angular-oauth2-oidc";
import { Observable } from "rxjs";
import { SearchWorkflowFileResult } from "../models/searchworkflowfile.model";

@Injectable()
export class WorkflowFileService {
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService) { }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchWorkflowFileResult> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/workflows/search";
    const request: any = { startIndex: startIndex, count: count, takeLatest: true };
    if (order) {
      request["orderBy"] = order;
    }

    if (direction) {
      request["order"] = direction;
    }

    return this.http.post<SearchWorkflowFileResult>(targetUrl, JSON.stringify(request), { headers: headers });
  }
}
