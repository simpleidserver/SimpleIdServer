import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environments';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { SearchResult } from '../../applications/models/search.model';
import { OAuthScope } from '../models/oauthscope.model';

@Injectable()
export class OAuthScopeService {
  constructor(private http: HttpClient, private oauthService: OAuthService) { }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<OAuthScope>> {
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/scopes/.search?start_index=" + startIndex + "&count=" + count;
    if (order) {
      targetUrl = targetUrl + "&order_by=" + order;
    }

    if (direction) {
      targetUrl = targetUrl + "&order=" + direction;
    }

    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      var result = new SearchResult<OAuthScope>();
      result.StartIndex = res["start_index"];
      result.Count = res["count"];
      result.TotalLength = res["total_length"];
      result.Content = res['content'].map((c: any) => OAuthScope.fromJson(c));
      return result;
    }));
  }

  getAll(): Observable<OAuthScope[]> {
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/scopes";
    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      return res.map((r : any) => OAuthScope.fromJson(r));
    }));
  }

  get(scope : string): Observable<OAuthScope> {
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/scopes/" + scope;
    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      return OAuthScope.fromJson(res);
    }));
  }

  update(scope: string, claims: string[]): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/scopes/" + scope;
    const request: any = {
      claims: claims
    };
    return this.http.put(targetUrl, request, { headers: headers });
  }

  add(scope: string): Observable<string> {
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/scopes";
    const request: any = {
      name: scope
    };
    return this.http.post(targetUrl, request, { headers: headers }).pipe(map((res: any) => {
      return res["id"];
    }));
  }

  delete(scope: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/scopes/" + scope;
    return this.http.delete(targetUrl, { headers: headers });
  }
}
