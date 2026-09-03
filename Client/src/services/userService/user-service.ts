import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  url = `${environment.apiUrl}/api/Owners/`
  http = inject(HttpClient)

  update(id: number, propertyData: any): Observable<any> {
    return this.http.put(`${this.url}${id}`, propertyData);
  }
  getOwnerById(id: number): Observable<any> {
    return this.http.get(this.url + id)
  }
}
