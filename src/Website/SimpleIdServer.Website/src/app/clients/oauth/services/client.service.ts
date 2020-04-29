import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { SearchResult } from '../models/search.model';
import { OAuthClient } from '../models/oauthclient.model';
import { environment } from '../../../../environments/environment';

@Injectable()
export class OAuthClientService {
    constructor(private http: HttpClient, private oauthService: OAuthService) { }

    search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<OAuthClient>> {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
        let targetUrl = environment.apiUrl + "/oauthclients/.search?start_index=" + startIndex + "&count=" + count;
        if (order) {
            targetUrl = targetUrl + "&order_by=" + order;
        }

        if (direction) {
            targetUrl = targetUrl + "&order=" + direction;
        }

        return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
            var result = new SearchResult<OAuthClient>();
            result.StartIndex = res["start_index"];
            result.Count = res["count"];
            result.TotalLength = res["total_length"];
            result.Content = res["content"].map(c => OAuthClient.fromJson(c));
            return result;
        }));
    }
}