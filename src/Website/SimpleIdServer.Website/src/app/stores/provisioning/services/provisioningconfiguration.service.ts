import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environments';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { SearchResult } from '../../applications/models/search.model';
import { ProvisioningConfiguration } from '../models/provisioningconfiguration.model';
import { ProvisioningConfigurationHistory } from '../models/provisioningconfigurationhistory.model';
import { ProvisioningConfigurationRecord } from '../models/provisioningconfigurationrecord.model';

@Injectable()
export class ProvisioningConfigurationService {
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService) { }

  searchHistory(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<ProvisioningConfigurationHistory>> {
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

  search(startIndex: number, count: number, order: string, direction: string): Observable<SearchResult<ProvisioningConfiguration>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/Provisioning/configurations/.search";
    const request: any = {
      startIndex: startIndex,
      count: count
    };
    if (order) {
      request['orderBy'] = order;
    }

    if (direction) {
      request['order'] = direction;
    }

    return this.http.post<SearchResult<ProvisioningConfiguration>>(targetUrl, request, { headers: headers });
  }

  get(id: string): Observable<ProvisioningConfiguration> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/Provisioning/configurations/" + id;
    return this.http.get<ProvisioningConfiguration>(targetUrl, { headers: headers });
  }

  update(id: string, records: ProvisioningConfigurationRecord[]): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/Provisioning/configurations/" + id;
    const request: any = {
      records: records
    };

    return this.http.put<any>(targetUrl, request, { headers: headers });
  }
}
