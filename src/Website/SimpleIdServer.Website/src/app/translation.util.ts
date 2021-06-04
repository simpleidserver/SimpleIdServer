import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EventEmitter, Injectable, Output } from '@angular/core';
import { environment } from '@envs/environments';
import { TranslateLoader } from '@ngx-translate/core';
import { forkJoin, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

enum HttpPathType {
  Relative = 0,
  Absolute = 1
}

class HttpFileLocationOption {
  constructor(public type: HttpPathType) { }
}

class RelativeHttpFileLocationOption extends HttpFileLocationOption {
  constructor(public prefix: string = './assets/i18n/', public suffix: string = '.json') {
    super(HttpPathType.Relative);
  }
}

class AbsoluteHttpFileLocationOption extends HttpFileLocationOption {
  constructor(public jsonKey : string, public url: string) {
    super(HttpPathType.Absolute);
  }
}

class MultipleFilesHttpLoaderOptions {
  private options: HttpFileLocationOption[];

  constructor() {
    this.options = [];
  }

  addRelativeHttpFileLocation(prefix: string = './assets/i18n/', suffix: string = '.json'): MultipleFilesHttpLoaderOptions {
    this.options.push(new RelativeHttpFileLocationOption(prefix, suffix));
    return this;
  }

  addAbsoluteHttpFileLocation(jsonKey: string, url: string): MultipleFilesHttpLoaderOptions {
    this.options.push(new AbsoluteHttpFileLocationOption(jsonKey, url));
    return this;
  }

  build(): HttpFileLocationOption[] {
    return this.options;
  }
}

@Injectable()
export class MultipleFilesHttpLoader implements TranslateLoader {
  @Output() loaded: EventEmitter<any> = new EventEmitter();

  constructor(private http: HttpClient, private option: MultipleFilesHttpLoaderOptions) { }

  getTranslation(lang: string): Observable<object[]> {
    const requestLst: any[] = [];
    const options = this.option.build();
    let headers = new HttpHeaders();
    headers = headers.set('Accept-Language', lang);
    options.forEach((location: HttpFileLocationOption) => {
      switch (location.type) {
        case HttpPathType.Relative:
          const relativeFileLocation = location as RelativeHttpFileLocationOption;
          requestLst.push(this.http.get(`${relativeFileLocation.prefix}${lang}${relativeFileLocation.suffix}`));
          break;
        case HttpPathType.Absolute:
          const absoluteFileLocation = location as AbsoluteHttpFileLocationOption;
          requestLst.push(this.http.get(absoluteFileLocation.url, { headers: headers }));
          break;
      }
    });

    return forkJoin(
      requestLst
    ).pipe(map((res) => {
      let index = 0;
      let result: any = {};
      options.forEach((opt: HttpFileLocationOption) => {  
        if (opt.type === HttpPathType.Relative) {
          result = Object.assign(result, res[index]);
        } else {
          const absoluteFileLocation = opt as AbsoluteHttpFileLocationOption;
          result[absoluteFileLocation.jsonKey] = res[index];
        }
        index++;
      });

      this.loaded.emit(null);
      return result;
    }));
  }
}

export function translationFactory(http: HttpClient) {
  const option = new MultipleFilesHttpLoaderOptions();
  const apiUrl = environment.openidUrl;
  option.addRelativeHttpFileLocation();
  option.addAbsoluteHttpFileLocation('lngs', `${apiUrl}/metadata/languages`);
  option.addAbsoluteHttpFileLocation('metadata', `${apiUrl}/metadata`);
  return new MultipleFilesHttpLoader(http, option);
}
