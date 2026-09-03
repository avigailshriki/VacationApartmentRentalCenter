import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IProperty, IPagedResult } from '../../Interfaces/Iproperty';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PropertyService {
  searchParams = new HttpParams();
  url = `${environment.apiUrl}/api/`;
  http = inject(HttpClient);

  getAllProperties(page: number = 1, pageSize: number = 20): Observable<IPagedResult<IProperty>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<IPagedResult<IProperty>>(this.url + 'Properties', { params });
  }
  getByID(id: number): Observable<IProperty> {
    return this.http.get<IProperty>(this.url + 'Properties/' + id);
  }
  filters(searchParams: HttpParams, page: number = 1, pageSize: number = 20): Observable<IPagedResult<IProperty>> {
    const params = searchParams.set('page', page).set('pageSize', pageSize);
    return this.http.get<IPagedResult<IProperty>>(this.url + 'Properties' + '/Search', { params });
  }
  getCities(): Observable<string[]> {
    return this.http.get<string[]>(this.url + 'Properties/Cities');
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
  uploadImage(file: File, propertyId: number): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('propertyId', propertyId.toString());
    return this.http.post(this.url + "Images/upload", formData);
  }
  deleteImage(imageId: number): Observable<any> {
    return this.http.delete(this.url + 'Images/' + imageId);
  }
  //Availability
  getAvailability(propertyId: number): Observable<any[]> {
    return this.http.get<any[]>(this.url + 'PropertyAvailability/Property/' + propertyId);
  }
  blockDates(propertyId: number, startDate: string, endDate: string): Observable<any> {
    return this.http.post(this.url + 'PropertyAvailability', {
      PropertyId: propertyId,
      StartDate: startDate,
      EndDate: endDate
    });
  }
  unblockDates(id: number): Observable<any> {
    return this.http.delete(this.url + 'PropertyAvailability/' + id);
  }
  addPropertyWithImage(ownerId: number, formData: FormData) {
    return this.http.post(`${this.url}Properties/AddByOwner/${ownerId}`, formData);
  }
}
