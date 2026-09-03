import { Component, OnInit, ChangeDetectorRef, ViewChild, ElementRef, inject } from '@angular/core';
import { PropertyService } from '../../services/PropertiesService/property-service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { IProperty, IReview } from '../../Interfaces/Iproperty';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { environment } from '../../environments/environment';
import { AuthService } from '../../services/AuthService/auth-service';
import { ToastService } from '../../services/ToastService/toast-service';
import { AvailabilityCalendar } from '../availability-calendar/availability-calendar';

declare var google: any;

@Component({
  selector: 'app-property-details',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, AvailabilityCalendar],
  templateUrl: './property-details.html',
  styleUrl: './property-details.css'
})
export class PropertyDetails implements OnInit {
  apiUrl = environment.apiUrl;
  authService = inject(AuthService);
  toastService = inject(ToastService);
  property?: IProperty;
  nearbyAttractions: any[] = [];
  showMap: boolean = false;
  reviewForm!: FormGroup;
  showImages: boolean = false;
  @ViewChild('mapElement') mapElement!: ElementRef;
  propertyService = inject(PropertyService)
  mapLat: number = 0;
  mapLng: number = 0;

  constructor(
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.reviewForm = this.fb.group({
      Name: ['', Validators.required],
      Rating: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
      Comment: ['', Validators.required]
    });
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadProperty(id);
  }
  toggleImages() {
    this.showImages = !this.showImages;
  }
  toggleMap() {
    this.showMap = !this.showMap;
    if (this.showMap) {
      setTimeout(() => {
        this.initMap(this.mapLat, this.mapLng);
      }, 100);
    }
  }
  loadProperty(Id: number) {
    this.propertyService.getByID(Id).subscribe({
      next: (data: any) => {
        this.property = data;
        if (this.property) {
          this.getAllReviews();
          this.geolocateAddress(this.property);
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('--- שגיאה בקריאת השרת:', err); 
      }
    });
  }
  geolocateAddress(property: IProperty) {
    if (typeof google === 'undefined') return;
    const geocoder = new google.maps.Geocoder();
    const address = `${property.Address}, ${property.City}, Israel`;

    geocoder.geocode({ address: address }, (results: any, status: any) => {
      if (status === 'OK' && results && results[0]) {
        this.mapLat = results[0].geometry.location.lat();
        this.mapLng = results[0].geometry.location.lng();
      }
    });
  }
  initMap(lat: number, lng: number): void {
    if (!this.mapElement || !this.mapElement.nativeElement) {
      console.error("אלמנט המפה לא נמצא ב-DOM");
      return;
    }
    const map = new google.maps.Map(this.mapElement.nativeElement, {
      center: { lat: lat, lng: lng },
      zoom: 15,
    });
    new google.maps.Marker({
      position: { lat: lat, lng: lng },
      map: map,
      title: this.property?.Title || 'מיקום הנכס'
    });
  }
 getAllReviews() {
  this.propertyService.getAllReviews().subscribe({
    next: (reviews: IReview[]) => {
      if (this.property) {
        this.property.Reviews = reviews.filter(r => Number(r.PropertyId) === Number(this.property!.Id));
        this.cdr.detectChanges();
      }
    },
    error: (err) => console.error('שגיאה בטעינת חוות דעת:', err)
  });
}
  submitReview() {
    if (!this.authService.isLoggedIn()) return;
    if (this.reviewForm.invalid || !this.property) return;
    const payload = {
      PropertyId: this.property.Id,
      Rating: Number(this.reviewForm.value.Rating),
      Comment: this.reviewForm.value.Comment,
      Name: this.reviewForm.value.Name,
      Date: new Date().toISOString()
    };
    this.propertyService.addReview(payload).subscribe({
      next: () => {
        this.toastService.success('תודה! חוות הדעת שלך נשלחה בהצלחה.');
        this.reviewForm.reset({ Rating: 5 });
        this.loadProperty(this.property!.Id!); 
      },
      error: (err) => {
        console.error('שגיאה בשליחת חוות דעת:', err);
        this.toastService.error('אירעה שגיאה בשליחת חוות הדעת.');
      }
    });
  }
  handleImageError(event: any) {
    const element = event.target as HTMLImageElement;
    element.style.display = 'none';
    console.warn('התמונה לא נמצאה בשרת בנתיב:', element.src);
  }
}