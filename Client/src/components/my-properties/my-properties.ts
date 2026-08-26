import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PropertyService } from '../../services/PropertiesService/property-service';
import { IProperty } from '../../Interfaces/Iproperty';
import { Router } from '@angular/router';

@Component({
  selector: 'app-my-properties',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-properties.html',
  styleUrl: './my-properties.css'
})
export class MyProperties implements OnInit {
  router = inject(Router);
  cdr = inject(ChangeDetectorRef);
  allProperties: IProperty[] = [];
  properties: IProperty[] = [];
  myProperties: any[] = [];
  isLoading: boolean = true;
  propertyService = inject(PropertyService)
  currentOwnerId: number = Number(localStorage.getItem('Id'))

  ngOnInit() {
    const savedId = localStorage.getItem('Id');
    console.log("The Id = ", savedId)
    if (savedId) {
      this.currentOwnerId = Number(savedId);
    }
    this.loadMyProperties();
  }
  loadMyProperties() {
    const savedId = localStorage.getItem('Id');

    const ownerId = Number(savedId);

    if (!savedId || isNaN(ownerId)) {
      console.error("שגיאה: לא נמצא ID תקין ב-localStorage");
      this.isLoading = false;
      alert("לא זוהית כמשתמש מחובר. אנא התחבר מחדש.");
      return; 
    }
    this.isLoading = true;
    this.propertyService.getMyProperties(ownerId).subscribe({
      next: (data: any) => {
        this.myProperties = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("שגיאה בטעינת הדירות:", err);
        this.isLoading = false;
      }
    });
  }
  deleteProperty(id: number): void {
    if (!id || id === 0) return;

    if (confirm('האם אתה בטוח שברצונך למחוק נכס זה?')) {
      this.myProperties = this.myProperties.filter(p => (p.Id || p.Id || p.propeIdrtyId) !== id);
      this.cdr.detectChanges();

      this.propertyService.DeleteProperty(id).subscribe({
        next: (response) => {
          console.log('נמחק בהצלחה מהשרת:', response);
        },
        error: (err) => {
          console.error('שגיאה בזמן מחיקה בשרת:', err);
          alert('התרחשה שגיאה בזמן המחיקה בשרת.');
          this.loadMyProperties();
        }
      });
    }
  }
  updateProperty(id: number): void {
    if (!id) return;
    this.router.navigate([`/add-property/edit/${id}`]);
  }
  changeStatus(property: any): void {
    const id = property.Id || property.Id;
    if (!id) {
      console.error("לא נמצא מזהה דירה תקין");
      return;
    }
    this.propertyService.changeStatus(id).subscribe({
      next: (updatedProperty: any) => {
        console.log("הסטטוס עודכן בשרת בהצלחה");

        property.IsAvailable = !property.IsAvailable;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("העדכון בשרת נכשל", err);
        alert("שגיאה בעדכון הסטטוס בשרת");
      }
    });
  }
}