import { Component, OnInit, inject } from '@angular/core';
import { PropertyService } from '../../services/PropertiesService/property-service';
import { IImages } from '../../Interfaces/Iproperty';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-images', 
  standalone: true,
  imports: [CommonModule, RouterModule, RouterLink],
  templateUrl: './images.html',
  styleUrls: ['./images.css']
})
export class Images implements OnInit {
  apiUrl = environment.apiUrl;
  allImages: IImages[] = [];
  isLoading = true; 
  images$!: Observable<any[]>;
  private propertyService = inject(PropertyService);

  constructor() {
    this.images$ = this.propertyService.getAllImages();
  }
  ngOnInit() {
    this.loadAllImages();
  }
  loadAllImages() {
    this.isLoading = true;
    this.propertyService.getAllImages().subscribe({
      next: (data) => {
        this.allImages = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error("--- שגיאה בתקשורת עם השרת ---");
        console.error("קוד שגיאה:", err.status);
        console.error("הודעת שגיאה:", err.message);
        this.isLoading = false;
      }
    });
  }
  onImageError(event: any, imageUrl: string) {
    console.error("שגיאה בטעינת תמונה בכתובת:", imageUrl);
    console.error("פרטי השגיאה:", event);
  }
}