import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SearchResult } from '@app/stores/applications/models/search.model';
import { environment } from '@envs/environments';
import { TranslateService } from '@ngx-translate/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Group } from '../models/group.model';

@Injectable()
export class GroupService {
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService,
    private translateService: TranslateService) { }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<Group>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/groups?startIndex=" + startIndex + "&count=" + count;
    if (order) {
      targetUrl = targetUrl + "&sortBy=" + order;
    }

    if (direction) {
      targetUrl = targetUrl + "&sortOrder=" + direction;
    }

    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      var result = new SearchResult<Group>();
      result.StartIndex = res["startIndex"];
      result.TotalLength = res["totalResults"];
      result.Content = res['Resources'].map((c: any) => Group.fromJson(c));
      return result;
    }));
  }

  get(groupId: string): Observable<Group> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/groups/" + groupId;
    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      return Group.fromJson(res);
    }));
  }

  update(groupId: string, request: any) {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/groups/" + groupId;
    return this.http.put(targetUrl, request, { headers: headers });
  }

  delete(groupId: string) {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/groups/" + groupId;
    return this.http.delete(targetUrl, { headers: headers });
  }
}
