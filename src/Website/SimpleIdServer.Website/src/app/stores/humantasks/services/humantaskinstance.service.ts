import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { OAuthService } from "angular-oauth2-oidc";
import { Observable } from "rxjs";
import { environment } from '@envs/environments';
import { HumanTaskInstance } from "../models/humantaskinstance.model";
import { TranslateService } from "@ngx-translate/core";
import { SearchResult } from "../../applications/models/search.model";

@Injectable()
export class HumanTaskInstanceService {
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService,
    private translate: TranslateService) { }

  getRendering(humanTaskInstanceId: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getAccessToken());
    headers = headers.set('Content-Type', 'application/json');
    const targetUrl = environment.apiUrl + "/humantaskinstances/" + humanTaskInstanceId + "/rendering";
    return this.http.get<any>(targetUrl, { headers: headers });
  }

  getDetails(humanTaskInstanceId: string): Observable<HumanTaskInstance> {
    let headers = new HttpHeaders();
    const defaultLang = this.translate.currentLang;
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getAccessToken());
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Accept-Language', defaultLang);
    const targetUrl = environment.apiUrl + "/humantaskinstances/" + humanTaskInstanceId + "/details";
    return this.http.get<HumanTaskInstance>(targetUrl, { headers: headers });
  }

  getDescription(humanTaskInstanceId: string): Observable<string> {
    let headers = new HttpHeaders();
    const defaultLang = this.translate.currentLang;
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getAccessToken());
    headers = headers.set('Accept-Language', defaultLang);
    const targetUrl = environment.apiUrl + "/humantaskinstances/" + humanTaskInstanceId + "/description";
    return this.http.get<string>(targetUrl, { headers: headers });
  }


  completeTask(humanTaskInstanceId: string, operationParameters: any): Observable<boolean> {
    let headers = new HttpHeaders();
    const defaultLang = this.translate.currentLang;
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getAccessToken());
    headers = headers.set('Accept-Language', defaultLang);
    const targetUrl = environment.apiUrl + "/humantaskinstances/" + humanTaskInstanceId + "/complete";
    const request: any = { operationParameters: operationParameters };
    return this.http.post<boolean>(targetUrl, JSON.stringify(request), { headers: headers });
  }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<HumanTaskInstance>> {
    let headers = new HttpHeaders();
    const defaultLang = this.translate.currentLang;
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getAccessToken());
    headers = headers.set('Accept-Language', defaultLang);
    const targetUrl = environment.apiUrl + "/humantaskinstances/.search";
    const request: any = { startIndex: startIndex, count: count };
    if (order) {
      request["orderBy"] = order;
    }

    if (direction) {
      request["order"] = direction;
    }

    return this.http.post<SearchResult<HumanTaskInstance>>(targetUrl, JSON.stringify(request), { headers: headers });
  }
}
