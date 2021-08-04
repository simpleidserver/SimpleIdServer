import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environments';
import { TranslateService } from '@ngx-translate/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthSchemeProvider } from '../models/authschemeprovider.model';

@Injectable()
export class AuthSchemeProviderService {
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService,
    private translateService: TranslateService) { }

  getAll(): Observable<AuthSchemeProvider[]> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/authschemeproviders";
    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      return res.map((c: any) => AuthSchemeProvider.fromJson(c));
    }));
  }

  get(id: string): Observable<AuthSchemeProvider> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/authschemeproviders/" + id
    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      return AuthSchemeProvider.fromJson(res);
    }));
  }

  updateOptions(id: string, request: any): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/authschemeproviders/" + id + '/options'
    return this.http.put(targetUrl, request, { headers: headers });
  }

  enable(id: string) {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/authschemeproviders/" + id + '/enable'
    return this.http.get(targetUrl, { headers: headers });
  }

  disable(id: string) {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
    const targetUrl = environment.apiUrl + "/authschemeproviders/" + id + '/disable'
    return this.http.get(targetUrl, { headers: headers });
  }
}
