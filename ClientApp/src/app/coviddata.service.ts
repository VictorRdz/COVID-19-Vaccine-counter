import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, interval } from 'rxjs';
import { Covid } from '../models/covid';
import { environment } from './../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CoviddataService {

  constructor( private http: HttpClient ) { }

  private url = 'api/covid/';

  getData(zone): Observable<Covid> {
    return this.http.get<Covid>(environment.apiUrl + this.url + zone)
  }

}
