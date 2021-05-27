import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environments';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { OAuthScope } from '../models/oauthscope.model';

@Injectable()
export class OAuthScopeService {
    constructor(private http: HttpClient, private oauthService: OAuthService) { }

    getAll(): Observable<Array<OAuthScope>> {
        let headers = new HttpHeaders();
        headers = headers.set('Accept', 'application/json');
        headers = headers.set('Authorization', 'Bearer ' + this.oauthService.getIdToken());
        let targetUrl = environment.apiUrl + "/oauth/management/scopes"
        return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
            var result = res.map((s : any) => OAuthScope.fromJson(s));
            return result;
        }));
    }
}
