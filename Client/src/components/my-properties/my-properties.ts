import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PropertyService } from '../../services/PropertiesService/property-service';
import { IProperty } from '../../Interfaces/Iproperty';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { ToastService } from '../../services/ToastService/toast-service';
import { ConfirmService } from '../../services/ConfirmService/confirm-service';

@Component({
  selector: 'app-my-properties',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-properties.html',
  styleUrl: './my-properties.css'
})
export class MyProperties implements OnInit {
  apiUrl = environment.apiUrl;
  router = inject(Router);
  cdr = inject(ChangeDetectorRef);
  toastService = inject(ToastService);
  confirmService = inject(ConfirmService);
  allProperties: IProperty[] = [];
  properties: IProperty[] = [];
  myProperties: any[] = [];
  isLoading: boolean = true;
  propertyService = inject(PropertyService)
  currentOwnerId: number = Number(localStorage.getItem('Id'))

  ngOnInit() {
    const savedId = localStorage.getItem('Id');
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
      this.toastService.error("לא זוהית כמשתמש מחובר. אנא התחבר מחדש.");
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
  async deleteProperty(id: number): Promise<void> {
    if (!id || id === 0) return;

    const confirmed = await this.confirmService.confirm('האם אתה בטוח שברצונך למחוק נכס זה?');
    if (confirmed) {
      this.myProperties = this.myProperties.filter(p => (p.Id || p.Id || p.propeIdrtyId) !== id);
      this.cdr.detectChanges();

      this.propertyService.DeleteProperty(id).subscribe({
        error: (err) => {
          console.error('שגיאה בזמן מחיקה בשרת:', err);
          this.toastService.error('התרחשה שגיאה בזמן המחיקה בשרת.');
          this.loadMyProperties();
        }
      });
    }
  }
  updateProperty(id: number): void {
    if (!id) return;
    this.router.navigate([`/add-property/edit/${id}`]);
  }
}