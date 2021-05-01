import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Display } from 'src/models/display';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  constructor( private http: HttpClient ) { }

  private url = 'api/covid/';

  getData(type): Observable<Display> {
    return this.http.get<Display>(environment.apiUrl + this.url + type)
  }
}
