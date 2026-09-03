import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { PropertyService } from '../../services/PropertiesService/property-service';
import { IProperty } from '../../Interfaces/Iproperty';

declare var google: any;

@Component({
  selector: 'app-property-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './property-details.html',
  styleUrl: './property-details.css'
})
export class PropertyDetails implements OnInit {
  property: IProperty | null = null;
  nearbyAttractions: any[] = []; 

  route = inject(ActivatedRoute);
  propertyService = inject(PropertyService);
  cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    
    this.propertyService.getByID(id).subscribe({
      next: (data: any) => {
        this.property = data;
        
        if (this.property) {
          // המתנה קלה כדי לוודא שגוגל טעון, או הפעלה מיידית אם הוא כבר זמין
          this.checkGoogleAndLoadAttractions(this.property);
        }
        this.cdr.detectChanges();
      },
      error: (err) => console.error('שגיאה בטעינת הנכס:', err)
    });
  }

  private checkGoogleAndLoadAttractions(property: IProperty): void {
    // הגנה: אם גוגל לא טעון, ננסה שוב בעוד חצי שנייה
    if (typeof google === 'undefined' || !google.maps || !google.maps.places) {
      console.warn('Google Maps API עדיין לא מוכן, מנסה שוב...');
      setTimeout(() => this.checkGoogleAndLoadAttractions(property), 500);
      return;
    }
    this.getNearbyAttractions(property);
  }

  getNearbyAttractions(property: IProperty): void {
    const dummyElement = document.createElement('div');
    const service = new google.maps.places.PlacesService(dummyElement);
    const geocoder = new google.maps.Geocoder();
    
    // שימוש בכתובת ברורה יותר כדי למנוע טעויות בחיפוש
    const searchAddress = `${property.Address || ''}, ${property.City || ''}, Israel`;

    geocoder.geocode({ address: searchAddress }, (results: any, status: any) => {
      if (status === 'OK' && results && results[0]) {
        const location = results[0].geometry.location;

        const request = {
          location: location,
          radius: 2000, 
          type: 'tourist_attraction' 
        };

        service.nearbySearch(request, (placesResult: any, placesStatus: any) => {
          if (placesStatus === 'OK' && placesResult) {
            this.nearbyAttractions = placesResult.slice(0, 5).map((place: any) => ({
              name: place.name,
              rating: place.rating || 'אין דירוג',
              address: place.vicinity,
              icon: place.icon
            }));
            this.cdr.detectChanges(); 
          }
        });
      } else {
        console.warn('לא הצלחנו למצוא מיקום גיאוגרפי עבור:', searchAddress);
      }
    });
  }
}