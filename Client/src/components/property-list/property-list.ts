import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IProperty, IPagedResult } from '../../Interfaces/Iproperty';
import { PropertyService } from '../../services/PropertiesService/property-service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-property-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './property-list.html',
  styleUrl: './property-list.css'
})
export class PropertyList implements OnInit {
  properties: IProperty[] = [];
  filters: string[] = [];
  selectedCity: string = '';
  searchTitle: string = '';
  currentMaxPrice: number = 10000;
  currentCapacity: number = 0;

  // דפדוף (pagination) - טוענים את הנכסים בעמודים במקום את כולם בבת אחת
  currentPage: number = 1;
  pageSize: number = 20;
  totalCount: number = 0;
  totalPages: number = 0;

  propertyService = inject(PropertyService);
  router = inject(Router);
  cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.loadCities();
    this.loadProperties();
  }
  loadCities(): void {
    this.propertyService.getCities().subscribe({
      next: (cities: any) => {
        this.filters = Array.isArray(cities) ? cities : (cities?.$values ?? []);
        this.cdr.detectChanges();
      },
      error: (err) => console.error('שגיאה בטעינת רשימת הערים:', err)
    });
  }
  loadProperties(): void {
    const hasActiveFilters = !!this.selectedCity || !!this.searchTitle ||
      this.currentMaxPrice < 10000 || this.currentCapacity > 0;

    if (hasActiveFilters) {
      let params = new HttpParams();
      if (this.selectedCity) params = params.set('city', this.selectedCity);
      if (this.searchTitle) params = params.set('title', this.searchTitle);
      if (this.currentMaxPrice < 10000) params = params.set('maxPrice', this.currentMaxPrice.toString());
      if (this.currentCapacity > 0) params = params.set('capacity', this.currentCapacity.toString());

      this.propertyService.filters(params, this.currentPage, this.pageSize).subscribe({
        next: (data: IPagedResult<IProperty>) => this.applyPagedResult(data),
        error: (err) => {
          console.error('שגיאה בטעינת הנכסים המסוננים:', err);
          this.properties = [];
        }
      });
    } else {
      this.propertyService.getAllProperties(this.currentPage, this.pageSize).subscribe({
        next: (data: IPagedResult<IProperty>) => this.applyPagedResult(data),
        error: (err) => {
          console.error('שגיאה בטעינה ראשונית:', err);
          this.properties = [];
        }
      });
    }
  }
  private applyPagedResult(data: IPagedResult<IProperty>): void {
    this.properties = data?.Items ?? [];
    this.totalCount = data?.TotalCount ?? 0;
    this.totalPages = data?.TotalPages ?? 0;
    this.cdr.detectChanges();
  }
  getUserName(): string | null {
    return localStorage.getItem('userName');
  }
  applyFilters(): void {
    this.currentPage = 1;
    this.loadProperties();
  }
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.currentPage = page;
    this.loadProperties();
  }
  nextPage(): void {
    this.goToPage(this.currentPage + 1);
  }
  previousPage(): void {
    this.goToPage(this.currentPage - 1);
  }
  onViewDetails(id: number | undefined): void {
    if (id !== undefined && id !== null) {
      this.router.navigate(['/property-details', id]);
    } else {
      console.error('ניסיון ניווט עם ID לא תקין:', id);
    }
  }
  getPropertyImageUrl(property: IProperty): string {
    if (property && property.Images && property.Images.length > 0) {
      const imageUrl = property.Images[0].ImageUrl;
      if (imageUrl) {
        return `url(${environment.apiUrl}${imageUrl})`;
      }
    }
    return 'none';
  }
  onCityChange(city: string): void {
    this.selectedCity = city;
    this.applyFilters();
  }
  onTitleChange(title: string): void {
    this.searchTitle = title;
    this.applyFilters();
  }
  onPriceChange(maxPrice: number): void {
    this.currentMaxPrice = maxPrice;
    this.applyFilters();
  }
  onCapacityChange(capacity: number): void {
    this.currentCapacity = capacity;
    this.applyFilters();
  }
  clearFilters(): void {
    this.selectedCity = '';
    this.searchTitle = '';
    this.currentMaxPrice = 10000;
    this.currentCapacity = 0;
    this.currentPage = 1;
    this.loadProperties();
  }
}
