import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  http = inject(HttpClient);
  url = 'https://localhost:7011/api/Owners';

  register(data: any): Observable<any> {
    const body = {
      Email: data.email,
      Password: data.password,
      FullName: data.fullName,
      Phone: data.phone
    };
    return this.http.post(`${this.url}/Register`, body);
  }
  login(email: string, password: string): Observable<any> {
    const body = {
      Email: email,
      Password: password
    };
    return this.http.post(`${this.url}/Login`, body);
  }
}