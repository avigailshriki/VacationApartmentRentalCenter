import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IProperty } from '../../Interfaces/Iproperty';

@Injectable({
  providedIn: 'root'
})
export class PropertyService {
  searchParams = new HttpParams();
  url = 'https://localhost:7011/api/';
  http = inject(HttpClient);

  getAllProperties(): Observable<IProperty[]> {
    return this.http.get<IProperty[]>(this.url + 'Properties');
  }
  getByID(id: number): Observable<IProperty> {
    console.log("מבקש את הנכס עם ID:", id);
    return this.http.get<IProperty>(this.url + 'Properties/' + id);
  }
  filters(searchParams: HttpParams): Observable<any> {
    return this.http.get(this.url + 'Properties' + '/Search', { params: searchParams });
  }
  addPropertyByOwner(ownerId: number, propertyData: any): Observable<any> {
    return this.http.post(this.url + "Properties/AddByOwner/" + ownerId, propertyData);
  }
  DeleteProperty(id: number): Observable<any> {
    return this.http.delete(this.url + 'Properties/' + id);
  }
  getMyProperties(ownerId: number): Observable<IProperty[]> {
    return this.http.get<IProperty[]>(this.url + 'Properties/MyProperties/' + ownerId);
  }
  changeStatus(id: number): Observable<IProperty> {
    return this.http.patch<IProperty>(this.url + 'Properties' + '/ChangeStatus/' + id, null);
  }
  updateProperty(id: number, propertyData: any): Observable<any> {
    return this.http.put(this.url + 'Properties/' + id, propertyData);
  }
  //Review
  addReview(reviewData: any): Observable<any> {
    return this.http.post(this.url + 'Review/', reviewData);
  }
  getAllReviews(): Observable<any> {
    return this.http.get(this.url + 'Review/');
  }
  getAllImages(): Observable<any> {
    return this.http.get(this.url + 'Images/');
  }
  uploadImage(file: File, propertyId: number) {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('propertyId', propertyId.toString());
    return this.http.post(this.url + "/Upload", formData);
  }
  addPropertyWithImage(ownerId: number, formData: FormData) {
    return this.http.post(`${this.url}Properties/AddByOwner/${ownerId}`, formData);
  }
}
