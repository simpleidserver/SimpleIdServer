import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environments';
import { TranslateService } from '@ngx-translate/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { SearchResult } from '../../applications/models/search.model';
import { RelyingParty } from '../models/relyingparty.model';

@Injectable()
export class RelyingPartyService {
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService,
    private translateService: TranslateService) { }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<RelyingParty>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/relyingparties/.search?start_index=" + startIndex + "&count=" + count;
    if (order) {
      targetUrl = targetUrl + "&order_by=" + order;
    }

    if (direction) {
      targetUrl = targetUrl + "&order=" + direction;
    }

    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      var result = new SearchResult<RelyingParty>();
      result.startIndex = res["start_index"];
      result.count = res["count"];
      result.totalLength = res["total_length"];
      result.content = res['content'].map((c: any) => RelyingParty.fromJson(c));
      return result;
    }));
  }

  get(id: string): Observable<RelyingParty> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/relyingparties/" + id
    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      return RelyingParty.fromJson(res);
    }));
  }

  update(id: string, request: any): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    let targetUrl = environment.apiUrl + "/relyingparties/" + id
    return this.http.put(targetUrl, request, { headers: headers });
  }

  add(metadataUrl: string): Observable<string> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    headers = headers.set('Accept-Language', this.translateService.currentLang);
    let targetUrl = environment.apiUrl + "/relyingparties";
    const request: any = {
      metadata_url: metadataUrl
    };
    return this.http.post(targetUrl, request, { headers: headers }).pipe(map((res: any) => {
      return res['id'];
    }));
  }

  delete(id : string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    headers = headers.set('Accept-Language', this.translateService.currentLang);
    let targetUrl = environment.apiUrl + "/relyingparties/" + id;
    return this.http.delete(targetUrl, { headers: headers });
  }
}
