// addproprty.ts
import { Component, inject, OnInit, AfterViewInit, ElementRef, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PropertyService } from '../../../services/PropertiesService/property-service';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastService } from '../../../services/ToastService/toast-service';
import { ConfirmService } from '../../../services/ConfirmService/confirm-service';
import { AvailabilityCalendar } from '../../availability-calendar/availability-calendar';
import { forkJoin } from 'rxjs';

declare var google: any;

@Component({
  selector: 'app-addproprty',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, AvailabilityCalendar],
  templateUrl: './addproprty.html',
  styleUrl: './addproprty.css'
})
export class Addproprty implements OnInit, AfterViewInit {
  @ViewChild('cityInput') cityInput!: ElementRef;
  @ViewChild('addressInput') addressInput!: ElementRef;
  cityAutocompleteInstance: any;
  addressAutocompleteInstance: any;
  propertyService = inject(PropertyService);
  http = inject(HttpClient)
  router = inject(Router);
  route = inject(ActivatedRoute);
  cdr = inject(ChangeDetectorRef);
  toastService = inject(ToastService);
  confirmService = inject(ConfirmService);
  selectedFiles: File[] = [];
  imagePreviews: string[] = [];
  existingImages: any[] = [];
  propertyForm: FormGroup;
  cityBounds: any = null;
  selectedFile: File | null = null;
  isEditMode: boolean = false;
  propertyId: number | null = null;
  ownerIdFromLocalStorage: number | null = null;

  constructor(private fb: FormBuilder) {
    this.propertyForm = this.fb.group({
      Property: this.fb.group({
        PropertyID: [0],
        OwnerID: [0],
        Title: ['', Validators.required],
        City: ['', Validators.required],
        Address: ['', Validators.required],
        PricePerNight: [0, [Validators.required, Validators.min(1)]],
        Capacity: [0, [Validators.required, Validators.min(1)]],
        Description: [''],
        ImageUrl: ['']
      })
    });
  }
  removeImage(index: number) {
    this.selectedFiles.splice(index, 1);
    this.imagePreviews.splice(index, 1);
  }
  handleFiles(files: FileList) {
    this.selectedFiles = Array.from(files);
    this.imagePreviews = [];
    Array.from(files).forEach(file => {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreviews.push(e.target.result);
      };
      reader.readAsDataURL(file);
    });
  }
  ngOnInit() {
    const savedId = localStorage.getItem('Id');
    if (!savedId) {
      this.toastService.error("לא נמצא מזהה בעלים. התחברי שנית.");
      this.router.navigate(['/login']);
      return;
    }
    this.ownerIdFromLocalStorage = Number(savedId);

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode = true;
      this.propertyId = Number(idParam);
      this.loadPropertyToForm(this.propertyId);
    }
  }
  onSubmit() {
    const f = this.propertyForm.get('Property')?.value;

    if (this.isEditMode && this.propertyId) {
      const payload = {
        Title: f.Title || "",
        City: f.City || "",
        Address: f.Address || "",
        PricePerNight: Number(f.PricePerNight) || 0,
        Capacity: Number(f.Capacity) || 0,
        Description: f.Description || "ללא תיאור",
        OwnerID: this.ownerIdFromLocalStorage
      };
      this.propertyService.updateProperty(this.propertyId, payload).subscribe({
        next: () => {
          this.uploadNewImagesIfAny(this.propertyId!);
        },
        error: (err) => {
          console.error("שגיאה בעדכון הנכס:", err);
          this.toastService.error("שגיאה בעדכון הנכס.");
        }
      });
      return;
    }

    const formData = new FormData();
    formData.append('Title', f.Title || "");
    formData.append('City', f.City || "");
    formData.append('Address', f.Address || "");
    formData.append('PricePerNight', f.PricePerNight?.toString() || "0");
    formData.append('Capacity', f.Capacity?.toString() || "0");
    formData.append('Description', f.Description || "ללא תיאור");
    formData.append('Id', this.ownerIdFromLocalStorage!.toString());

    this.selectedFiles.forEach(file => {
      formData.append('images', file);
    });
    this.propertyService.addPropertyWithImage(this.ownerIdFromLocalStorage!, formData)
      .subscribe({
        next: (response) => {
          this.toastService.success("הנכס נוסף בהצלחה!");
          this.router.navigate(['/my-properties']);
        },
        error: (err) => {
          console.error("Server Error Details:", err.error);
          this.toastService.error("שגיאה בהוספת נכס. בדקי את ה-Console.");
        }
      });
  }
  onCityBlur() {
    const val = this.cityInput.nativeElement.value;
    if (val) this.propertyForm.patchValue({ Property: { City: val } });
  }
  onAddressBlur() {
    const val = this.addressInput.nativeElement.value;
    if (val) this.propertyForm.patchValue({ Property: { Address: val } });
  }
  loadPropertyToForm(id: number) {
    this.propertyService.getByID(id).subscribe({
      next: (data: any) => {
        const propertyData = data.Property || data;
        if (this.ownerIdFromLocalStorage && propertyData.OwnerID && propertyData.OwnerID !== this.ownerIdFromLocalStorage) {
          this.toastService.error('אין לך הרשאה לערוך נכס זה.');
          this.router.navigate(['/my-properties']);
          return;
        }
        this.propertyForm.patchValue({ Property: propertyData });
        this.existingImages = propertyData.Images || [];
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('שגיאה בטעינת הנכס לעריכה:', err);
        this.toastService.error('שגיאה בטעינת פרטי הנכס.');
        this.router.navigate(['/my-properties']);
      }
    });
  }
  async removeExistingImage(image: any) {
    const confirmed = await this.confirmService.confirm('האם למחוק את התמונה הזו?', 'מחיקה', 'ביטול');
    if (!confirmed) return;

    this.propertyService.deleteImage(image.Id).subscribe({
      next: () => {
        this.existingImages = this.existingImages.filter(img => img.Id !== image.Id);
        this.toastService.success('התמונה נמחקה בהצלחה.');
      },
      error: (err) => {
        console.error('שגיאה במחיקת תמונה:', err);
        this.toastService.error('שגיאה במחיקת התמונה.');
      }
    });
  }
  uploadNewImagesIfAny(propertyId: number) {
    if (this.selectedFiles.length === 0) {
      this.toastService.success("הנכס עודכן בהצלחה!");
      this.router.navigate(['/my-properties']);
      return;
    }

    const uploads = this.selectedFiles.map(file => this.propertyService.uploadImage(file, propertyId));
    forkJoin(uploads).subscribe({
      next: () => {
        this.toastService.success("הנכס והתמונות עודכנו בהצלחה!");
        this.router.navigate(['/my-properties']);
      },
      error: (err) => {
        console.error("שגיאה בהעלאת תמונות חדשות:", err);
        this.toastService.error("פרטי הנכס נשמרו, אך הייתה שגיאה בהעלאת התמונות החדשות.");
        this.router.navigate(['/my-properties']);
      }
    });
  }
  ngAfterViewInit() {
    this.initGoogleAutocompletes();
  }
  initGoogleAutocompletes() {
    if (!this.cityInput || !this.addressInput) return;

    this.cityAutocompleteInstance = new google.maps.places.Autocomplete(this.cityInput.nativeElement, {
      types: ['(cities)'],
      componentRestrictions: { country: 'il' }
    });
    this.cityAutocompleteInstance.addListener('place_changed', () => {
      const place = this.cityAutocompleteInstance.getPlace();
      if (place) this.updateCitySelection(place);
    });
    this.addressAutocompleteInstance = new google.maps.places.Autocomplete(this.addressInput.nativeElement, {
      types: ['address'],
      componentRestrictions: { country: 'il' },
      strictBounds: true
    });
    this.addressAutocompleteInstance.addListener('place_changed', () => {
      const place = this.addressAutocompleteInstance.getPlace();
      if (place) this.validateAndSetAddress(place);
    });
  }
  updateCitySelection(place: any) {
    const cityName = place.name;
    this.propertyForm.patchValue({ Property: { City: cityName, Address: '' } });
    this.cityInput.nativeElement.value = cityName;
    this.addressInput.nativeElement.value = '';
    if (place.geometry && place.geometry.viewport) {
      this.cityBounds = place.geometry.viewport;
      this.addressAutocompleteInstance.setBounds(this.cityBounds);
    }
    this.cdr.detectChanges();
  }
  validateAndSetAddress(place: any) {
    const fullAddress = place.formatted_address || place.name || '';
    if (fullAddress) {
      this.propertyForm.patchValue({ Property: { Address: fullAddress } });
      this.addressInput.nativeElement.value = fullAddress;
    }
    this.cdr.detectChanges();
  }
  onFileSelected(event: any) {
    this.selectedFiles = Array.from(event.target.files);
  }
  onDrop(event: DragEvent) {
    event.preventDefault();
    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      this.selectedFiles = Array.from(event.dataTransfer.files);
    }
  }
}
