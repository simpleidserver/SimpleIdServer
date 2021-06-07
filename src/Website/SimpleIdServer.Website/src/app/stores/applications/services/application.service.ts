import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environments';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Application } from '../models/application.model';
import { SearchResult } from '../models/search.model';

@Injectable()
export class ApplicationService {
  constructor(private http: HttpClient, private oauthService: OAuthService) { }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<Application>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/applications/.search?start_index=" + startIndex + "&count=" + count;
    if (order) {
      targetUrl = targetUrl + "&order_by=" + order;
    }

    if (direction) {
      targetUrl = targetUrl + "&order=" + direction;
    }

    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      var result = new SearchResult<Application>();
      result.StartIndex = res["start_index"];
      result.Count = res["count"];
      result.TotalLength = res["total_length"];
      result.Content = res['content'].map((c: any) => Application.fromJson(c));
      return result;
    }));
  }

  get(id: string): Observable<Application> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/applications/" + id
    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      return Application.fromJson(res);
    }));
  }

  update(id: string, request: any): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/applications/" + id
    return this.http.put(targetUrl, request, { headers: headers });
  }
}
