import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl || 'https://localhost:7001/api';

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    
    return headers;
  }

  executeCommand<T>(moduleName: string, requestName: string, parameter: any): Observable<T> {
    const payload = {
      RequestName: requestName,
      Parameter: parameter
    };

    return this.http.post<T>(`${this.baseUrl}/Command/Execute/${moduleName}`, payload, {
      headers: this.getHeaders()
    });
  }

  executeQuery<T>(moduleName: string, requestName: string, parameter: any): Observable<T> {
    const payload = {
      RequestName: requestName,
      Parameter: parameter
    };

    return this.http.post<T>(`${this.baseUrl}/Query/Execute/${moduleName}`, payload, {
      headers: this.getHeaders()
    });
  }
}
