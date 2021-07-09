import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environments';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { SearchResult } from '../../applications/models/search.model';
import { ProvisioningConfigurationHistory } from '../models/provisioningconfigurationhistory.model';

@Injectable()
export class ProvisioningConfigurationHistoryService {
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService) { }

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<ProvisioningConfigurationHistory>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/Provisioning/histories/.search";
    const request : any = {
      startIndex: startIndex,
      count: count
    };
    if (order) {
      request['orderBy'] = order;
    }

    if (direction) {
      request['order'] = direction;
    }

    return this.http.post<SearchResult<ProvisioningConfigurationHistory>>(targetUrl, request, { headers: headers });
  }
}
