import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IProperty } from '../../Interfaces/Iproperty';
import { PropertyService } from '../../services/PropertiesService/property-service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-property-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './property-list.html',
  styleUrl: './property-list.css'
})
export class PropertyList implements OnInit {
  allProperties: IProperty[] = [];
  properties: IProperty[] = [];
  filters: string[] = [];
  selectedCity: string = '';
  searchTitle: string = '';
  currentMaxPrice: number = 10000;
  currentCapacity: number = 0;
  propertyService = inject(PropertyService);
  router = inject(Router);
  cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.propertyService.getAllProperties().subscribe({
      next: (data: any) => {
        let rawList: IProperty[] = [];

        if (data && data.$values && Array.isArray(data.$values)) {
          rawList = data.$values;
        } else if (Array.isArray(data)) {
          rawList = data;
        }
        this.allProperties = rawList;
        this.properties = rawList;
        console.log(this.allProperties);

        this.filters = [...new Set(rawList.map(p => p.City).filter(Boolean))];
        this.cdr.detectChanges();
      },
      error: (err) => console.error('שגיאה בטעינה ראשונית:', err)
    });
  }
  getUserName(): string | null {
    return localStorage.getItem('userName');
  }
  applyFilters(): void {
    let params = new HttpParams();
    if (this.selectedCity) params = params.set('city', this.selectedCity);
    if (this.searchTitle) params = params.set('title', this.searchTitle);
    if (this.currentMaxPrice < 10000) params = params.set('maxPrice', this.currentMaxPrice.toString());
    if (this.currentCapacity > 0) params = params.set('capacity', this.currentCapacity.toString());

    this.propertyService.filters(params).subscribe({
      next: (data: any) => {
        this.properties = (data?.$values) ? data.$values : (Array.isArray(data) ? data : []);
        this.cdr.detectChanges();
      },
      error: () => this.properties = [...this.allProperties]
    });
  }
  onViewDetails(id: number | undefined): void {
    console.log("id= ", id)
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
        return `url(https://localhost:7011${imageUrl})`;
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
    this.properties = [...this.allProperties];
    this.cdr.detectChanges();
  }
}