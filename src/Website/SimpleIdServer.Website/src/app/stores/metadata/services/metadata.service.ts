import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@envs/environments';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Language } from '../models/language.model';

@Injectable()
export class MetadataService {
  constructor(private http: HttpClient, private oauthService: OAuthService) { }

  getLanguages(): Observable<Array<Language>> {
    let headers = new HttpHeaders();
    headers = headers.set('Accept', 'application/json');
    let targetUrl = environment.apiUrl + "/metadata/languages";
    return this.http.get(targetUrl, { headers: headers }).pipe(map((res: any) => {
      var result : Array<Language> = [];
      res.forEach((r: any) => {
        result.push(Language.fromJson(r));
      });

      return result;
    }));
  }
}
