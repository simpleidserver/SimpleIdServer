import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SearchResult } from '@app/stores/applications/models/search.model';
import { environment } from '@envs/environments';
import { TranslateService } from '@ngx-translate/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../models/user.model';

@Injectable()
export class UserService {
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService,
    private translateService: TranslateService) { }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<User>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/users?startIndex=" + startIndex + "&count=" + count;
    if (order) {
      targetUrl = targetUrl + "&sortBy=" + order;
    }

    if (direction) {
      targetUrl = targetUrl + "&sortOrder=" + direction;
    }

    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      var result = new SearchResult<User>();
      result.StartIndex = res["startIndex"];
      result.TotalLength = res["totalResults"];
      result.Content = res['Resources'].map((c: any) => User.fromJson(c));
      return result;
    }));
  }

  get(userId: string): Observable<User> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/users/" + userId;
    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      return User.fromJson(res);
    }));
  }

  update(userId: string, request: any): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Content-Type', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/users/" + userId;
    return this.http.put(targetUrl, request, { headers: headers });
  }
}
